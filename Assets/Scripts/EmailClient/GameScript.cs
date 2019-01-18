﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour {

    public TimerScript timer;
    public ExplanationScript explanations;
    public EmailScript emailScript;
    public ScoreScript score;
    public GameObject finishedPanel;
    public GameObject goBack;
    public GameObject pauseMenu; 
    // Audio sources
    public AudioSource whistleSound;
    public AudioSource backgroundMusic;
    public AudioSource lightClick;
    public AudioSource meanClick;
    public AudioSource scoreTally;
    // Variables
    private bool pauseEnabled; // Game can be paused
    private bool gameIsPaused; // Game is currently paused

    /*
     * initialisation
     */
    private void Awake()
    {
        // Make pause disabled
        pauseEnabled = false;
        pauseMenu.SetActive(false);
        gameIsPaused = false;
        // Make some objects inactive
        score.gameObject.SetActive(false);
        finishedPanel.SetActive(false);
        goBack.gameObject.SetActive(false);
        // Give your reference to other objects
        score       .SetGameScript(this);
        timer       .SetGameScript(this);
        explanations.SetGameScript(this);
        emailScript .SetGameScript(this);
    }

    /*
     * Called every frame after everything else
     */
    private void Update()
    {
        // Check if pause button pressed
        if (pauseEnabled && Input.GetKeyUp(KeyCode.Escape))
        {
            if (!gameIsPaused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        gameIsPaused = true;
        // Stop timer
        timer.PauseTimer();
        // Pause music
        backgroundMusic.Pause();
    }

    public void UnPause()
    {
        pauseMenu.SetActive(false);
        gameIsPaused = true;
        // Unpause music
        backgroundMusic.Play();
        // Unpause timer
        timer.UnPauseTimer();
    }

    /*
     * The timer reached 0
     */
    public void TimerEnded()
    {
        whistleSound.Play();
        StartCoroutine(EndGame());
    }

    /*
     * Check button was clicked after every mail was sorted
     */
     public void FinishedSortingEmails()
    {
        whistleSound.Play();
        timer.StopTimer();
        StartCoroutine(EndGame());
    }

    /*
     * Looked at all the explanation panels
     */
    public void ExplanationsDone()
    {
        StartCoroutine(StartGame());
    }

    /*
     * Start the game
     */
     IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.2f);
        // Make pause enabled
        pauseEnabled = true;
        // Start music
        backgroundMusic.Play();
        // Start timer
        timer.StartTimer();
    }

    /*
     * Game ended
     */
    IEnumerator EndGame()
    {
        // Disable pause
        pauseEnabled = false;
        // Stop the stuff
        backgroundMusic.Stop();
        finishedPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        int[] results = emailScript.CheckEmails();
        // (int totalEmailsInt, int phishingEmailsInt, int sortedEmailsInt, int correctlyIdentifiedInt, int wronglyTrashedInt)
        // Show score panel
        score.gameObject.SetActive(true);
        // Deactivate now useless panels
        finishedPanel.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
        // Show score
        score.ShowScore(results[0], results[1], results[2], results[3], results[4]);
    }

    /*
     * After the score has been shown
     */
     public void FinishedShowingScore()
    {
        // Tag emails
        emailScript.TagEmails();
        // Remove score panel and finish panel
        score.gameObject.SetActive(false);
        // Show try again and whatnot
        goBack.gameObject.SetActive(true);
    }

    public void PlayLightClick()
    {
        lightClick.Play();
    }

    public void PlayMeanClick()
    {
        meanClick.Play();
    }

    public void ToggleScoreTally(bool toggle)
    {
        if (toggle)
        {
            scoreTally.Play();
        }
        else
        {
            scoreTally.Pause();
        }
    }

    public void TryAgainButtonPressed()
    {
        // play click sound
        PlayMeanClick();
        // Start game over
        StartOver();
    }

    public void ContinueButtonPressed()
    {
        // play click sound
        PlayMeanClick();
        // Go back to office scene
        SceneManager.LoadScene("Office");
    }

    public void StartOver()
    {
        // Make some objects inactive
        score.Reset();
        finishedPanel.SetActive(false);
        goBack.gameObject.SetActive(false);
        // Reset timer
        timer.gameObject.SetActive(true);
        timer.ResetTimer();
        // Reset email positions and color
        emailScript.ResetEmails();
        // Start game
        StartCoroutine(StartGame());
    }
}
