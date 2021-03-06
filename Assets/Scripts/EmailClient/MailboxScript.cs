﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Script representing a folder
 * stores emails, shuffles them, retrieves them when selected and places the preview correctly on screen
 * shows the current number of emails in the corner
 */
namespace EmailClient
{
    public class MailboxScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // File constants
        private Color32 mailBoxHoverColor = new Color32(139, 139, 139, 71);
        private Color32 mailBoxNormalColor = new Color32(139, 139, 139, 0);

        private Color32 mailBoxSelectedColor = new Color32(255, 255, 255, 255);

        // Variables
        private int counter = 0;
        public Text counterText;

        private bool isActive = false;

        // The emails inside this mailbox
        private List<Email> emails;

        // The client window
        private EmailScript emailScript;

        /*
         * Initialisation
         * This is not done in start, the email list is instead assigned in the emailScript
         * this is because otherwise the emailscript would interact with the email list before it is set
         */
        public void InitialiseEmailList()
        {
            emails = new List<Email>();
            counter = 0;
            counterText.text = "" + counter;
            Unselect();
        }

        /*
         * Set email script
         */
        public void SetEmailScript(EmailScript emailScript)
        {
            this.emailScript = emailScript;
        }

        /*
         * Returns emails
         */
        public List<Email> GetEmails()
        {
            return emails;
        }

        /*
         * Add an email to the list
         */
        public void AddEmail(Email email)
        {
            emails.Add(email);
            IncrementCounter();
        }

        /*
         * Remove an email from the list of emails
         */
        public void RemoveEmail(Email emailToRemove)
        {
            DecrementCounter();
            // Remove email from array
            emails.Remove(emailToRemove);
            // Remove email from view
            emailToRemove.emailPreview.gameObject.SetActive(false);
            // Re-assign indexes
            AssignEmailIndexes();
            // Re-position the emails on screen
            RepositionEmailPreviews(emailToRemove.index);
        }

        /*
         * Increment email counter
         */
        public void IncrementCounter()
        {
            counter++;
            counterText.text = counter.ToString();
        }

        /*
         * Decrement email counter
         */
        public void DecrementCounter()
        {
            counter--;
            counterText.text = counter.ToString();
        }

        /*
         * Hover methods
         */
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Change color
            gameObject.GetComponent<Image>().color = mailBoxHoverColor;
            // Set hovered on mailbox
            emailScript.SetHoveredOn(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Change color back
            gameObject.GetComponent<Image>().color = (isActive ? mailBoxSelectedColor : mailBoxNormalColor);
            // Set hovered on mailbox
            emailScript.SetHoveredOn(null);
        }

        /*
         * Select this mailbox
         */
        public void Select()
        {
            isActive = true;
            // Change color
            this.gameObject.GetComponent<Image>().color = mailBoxSelectedColor;
            // Set email previews as active
            foreach (Email email in emails)
            {
                email.emailPreview.gameObject.SetActive(true);
            }

            // Re-assign indexes
            AssignEmailIndexes();
            // Position the emails on screenZ
            PositionEmailPreviews();
        }

        /*
         * Unselect this mailbox
         */
        public void Unselect()
        {
            isActive = false;
            // Change color
            this.gameObject.GetComponent<Image>().color = mailBoxNormalColor;
            // Set email previews as inactive
            foreach (Email email in emails)
            {
                email.emailPreview.gameObject.SetActive(false);
            }
        }

        /*
         * Positions the emails on top of eachother in the window
         */
        private void PositionEmailPreviews()
        {
            float distanceToTop = 0;
            foreach (Email email in emails)
            {
                email.emailPreview.gameObject.transform.localPosition = new Vector3(
                    email.originalPreviewPosition.x,
                    email.originalPreviewPosition.y - distanceToTop,
                    email.originalPreviewPosition.z
                );
                distanceToTop += email.emailPreview.gameObject.GetComponent<RectTransform>().rect.height;
            }
        }

        /*
         * Reposition emails when one preview has been removed
         */
        private void RepositionEmailPreviews(int index)
        {
            for (int i = index; i < emails.Count; i++)
            {
                emails[i].emailPreview.gameObject.transform.localPosition = new Vector3(
                    emails[i].emailPreview.gameObject.transform.localPosition.x,
                    emails[i].emailPreview.gameObject.transform.localPosition.y + emails[i].emailPreview.gameObject
                        .GetComponent<RectTransform>().rect.height,
                    emails[i].emailPreview.gameObject.transform.localPosition.z
                );
            }
        }

        /*
         * Shuffle emails
         */
        public void ShuffleEmails()
        {
            emails = Shuffle(emails);
            AssignEmailIndexes();
        }

        /*
         * Assigns indexes to the emails in the order in which they appear on screen
         */
        private void AssignEmailIndexes()
        {
            for (int i = 0; i < emails.Count; i++)
            {
                emails[i].index = i;
            }
        }

        /*
         * Called when you click on the mailbox
         */
        public void OnClick()
        {
            if (!isActive)
            {
                emailScript.SetCurrentMailbox(this);
                Select();
            }
        }

        private List<T> Shuffle<T>(List<T> list)
        {
            System.Random rnd = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}