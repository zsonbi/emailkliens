using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace emailkliens
{
    internal class DataBase
    {
        private const string domain = "dusza.hu"; //The default domain
        private List<Account> accounts = new List<Account>(); //The accounts stored in a List
        private Account currentlyLoggedIn; //The currently logged in user all the parameters are equal to "" if noone is logged in

        //----------------------------------------------------------------------
        /// <summary>
        /// Returns the currently Logged in user's username
        /// </summary>
        public string CurrentlyLoggedUser { get => currentlyLoggedIn.username; }

        //Constructor
        public DataBase(string file = "adatok.txt")
        {
            string[] lines = System.IO.File.ReadAllLines(file);
            foreach (var item in lines)
            {
                accounts.Add(new Account(item));
            }
        }

        //******************************************************************
        //Private Methods
        //---------------------------------------------------------------
        /// <summary>
        /// Saves the content of accounts into a adatok.txt file
        /// </summary>
        private void SaveAccounts()
        {
            string[] output = new string[accounts.Count];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = accounts[i].username + " " + accounts[i].password + " " + accounts[i].reminder;
            }
            System.IO.File.WriteAllLines("adatok.txt", output);
        }

        //----------------------------------------------------------------
        /// <summary>
        /// Saves the emails into a txt file with the username as the txt-s name
        /// </summary>
        /// <param name="user"></param>
        private void SaveEmails(string user)
        {
            Account acc = accounts.Find(x => x.username == user);
            string[] output = new string[acc.recievedMails.Count];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = acc.recievedMails[i].sender + " " + acc.recievedMails[i].subject + " " + acc.recievedMails[i].recievedate + " " + (acc.recievedMails[i].IsItRead ? "olvasott" : "olvasatlan") + " "
                    + "{" + acc.recievedMails[i].content + "}";
            }

            System.IO.File.WriteAllLines(user + ".txt", output);
        }

        //------------------------------------------------------------------
        /// <summary>
        /// Encode the password to Hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Hash Code</returns>
        private int EncodeToHash(string input)
        {
            input = input.PadRight(10, 'd');
            int output = 0;
            foreach (var item in input)
            {
                output += Convert.ToInt32(item);
            }
            return output;
        }

        /// <summary>
        ///Checks if the Content is a valid for an email
        /// </summary>
        /// <returns></returns>
        private bool ValidContent(string content)
        {
            foreach (var item in content.ToLower())
            {
                if (!(item >= 'a' && item <= 'z' || item >= '0' && item <= '9' || item == ' ' || item == '.' || item == '?' ||
                    item == '!' || item == ';' || item == ':' || item == ',' || item == '-'))
                {
                    return false;
                }
            }
            return true;
        }

        //-----------------------------------------------------
        /// <summary>
        /// Puts the email into the recipient's mailbox
        /// </summary>
        /// <param name="email">the email itself</param>
        /// <param name="recipient">Who will recieve the letter</param>
        private void SendEmail(email email, string recipient)
        {
            string temp = recipient.Split('[')[0];

            accounts.Find(x => x.username == temp).recievedMails.Add(email);
            SaveEmails(temp);
        }

        //---------------------------------------------------------
        //Checks if the word matches the criterias
        private bool Legal(string word, byte maxSize, byte minSize = 0)
        {
            if (word.Split(' ').Length != 1)
                return false;
            else if (word.Length > maxSize)
                return false;
            else if (word.Length < minSize)
                return false;
            return true;
        }

        //*********************************************************
        //Public Methods
        /// <summary>
        /// Checks if the database contains this user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool Contains(string username)
        {
            return accounts.FindIndex(x => x.username == username) != -1;
        }

        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Compares the password with the user's and if it's a match login to that account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ComparePass(string username, string password)
        {
            bool matches = accounts.Find(x => x.username == username).password == EncodeToHash(password).ToString();
            if (matches)
                currentlyLoggedIn = accounts.Find(x => x.username == username);

            return matches;
        }

        //-----------------------------------------------------------------
        /// <summary>
        /// Returns that user's reminder
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetReminder(string username)
        {
            if (!this.Contains(username))
                return "Nincs ilyen Felhasználó";
            return accounts.Find(x => x.username == username).reminder;
        }

        //--------------------------------------------------------------------
        /// <summary>
        /// Returns all the incoming mails for the currently logged in user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<email> GetEmails(byte orderType)
        {
            if (currentlyLoggedIn.username == "")
                throw new Exception("Not Logged in");
            switch (orderType)
            {
                case 0:
                    currentlyLoggedIn.recievedMails = currentlyLoggedIn.recievedMails.OrderBy(x => x.sender).ToList();
                    break;

                case 1:
                    currentlyLoggedIn.recievedMails = currentlyLoggedIn.recievedMails.OrderBy(x => x.subject).ToList();
                    break;

                case 2:
                    currentlyLoggedIn.recievedMails = currentlyLoggedIn.recievedMails.OrderBy(x => x.recievedate).ToList();
                    break;

                default:
                    break;
            }

            return currentlyLoggedIn.recievedMails;
        }

        //---------------------------------------------------------
        /// <summary>
        /// Adds a new account to the database
        /// </summary>
        /// <param name="newAcctount"></param>
        public void Add(string username, string password, string reminder)
        {
            accounts.Add(new Account(username, EncodeToHash(password).ToString(), reminder));
            SaveAccounts();
        }

        //-------------------------------------------------------------
        /// <summary>
        /// Logs the user out
        /// </summary>
        public void Logout()
        {
            currentlyLoggedIn = new Account();
        }

        //---------------------------------------------------------
        /// <summary>
        /// Checks if the database contains that recipient
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ValidRecipient(string email)
        {
            return email.Contains(domain);
        }

        //--------------------------------------------
        /// <summary>
        /// Puts the letter into the recipient's mailbox
        /// </summary>
        /// <param name="email"></param>
        /// <param name="recipient"></param>
        public byte WriteLetter(string recipient, string subject, string content)
        {
            if (!this.Contains(recipient.Split('[')[0]))
                return 1;
            else if (subject.Length > 15)

                return 1;
            else if (content.Length > 100 && ValidContent(content))
                return 2;
            SendEmail(new email(CurrentlyLoggedUser + "[kukac]" + domain, subject, content), recipient);
            return 0;
        }

        //-----------------------------------------------------
        /// <summary>
        /// Change the inputs is
        /// </summary>
        /// <param name="email"></param>
        public void ReadLetter(int emailIndex)
        {
            currentlyLoggedIn.recievedMails[emailIndex].IsItRead = true;
            SaveEmails(CurrentlyLoggedUser);
        }

        //-----------------------------------------------------
        /// <summary>
        /// Deletes the letter at the specified index
        /// </summary>
        public void DeleteLetter(int index)
        {
            currentlyLoggedIn.recievedMails.RemoveAt(index);
            accounts[accounts.FindIndex(x => x.username == CurrentlyLoggedUser)] = currentlyLoggedIn;

            SaveEmails(CurrentlyLoggedUser);
        }

        //-------------------------------------------------
        /// <summary>
        /// Checks if the user exists in the database
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsUserExists(string username)
        {
            return Contains(username);
        }

        //------------------------------------------------------------
        /// <summary>
        /// Returns the reminder of the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string ShowReminder(string username)
        {
            string reminder = GetReminder(username);
            return reminder == "" ? "No reminder is set" : reminder;
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Tries to log into an account
        /// </summary>
        /// <param name="username"></param>
        /// <returns>0 if succesful , returns 1 if there is no such user, returns 2 if the password was incorrect</returns>
        public byte Login(string username, string password)
        {
            if (!Contains(username))
                return 1;
            if (ComparePass(username, password))
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }

        //--------------------------------------------------------------
        /// <summary>
        /// Creates an account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="reminder"></param>
        /// <returns> 0 if the creation was succesful 1 if the username was incorrect 2 if the password was incorrect
        /// 3 if the reminder was bad or 4 if there is already a user with that name</returns>
        public byte CreateNewAccount(string username, string password, string reminder)
        {
            if (!Legal(username, 15))
            {
                return 1;
            }
            if (!Legal(password, 10, 8))
            {
                return 2;
            }
            if (!Legal(reminder, 15))
            {
                return 3;
            }
            if (Contains(username))
            {
                return 4;
            }
            Add(username, password, reminder);
            return 0;
        }
    }
}