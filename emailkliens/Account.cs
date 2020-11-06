using System;
using System.Collections.Generic;

namespace emailkliens
{
    internal struct Account
    {
        public string username;//The username of the user
        public string password;//The password encoded into hash
        public string reminder;//The reminder of the user
        public List<email> recievedMails;//The emails of the user

        //-------------------------------------------
        /// <summary>
        /// When creating a brand new Account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="reminder"></param>
        public Account(string username, string password, string reminder)
        {
            this.username = username;
            this.password = password;
            this.reminder = reminder;
            recievedMails = new List<email>();
        }

        //-----------------------------------
        /// <summary>
        /// When importing from file
        /// </summary>
        /// <param name="inputLine">leave the input empty if you want an empty Account</param>
        public Account(string inputLine = "")
        {
            this.recievedMails = new List<email>();
            if (inputLine == "")
            {
                this.password = "";
                this.username = "";
                this.reminder = "";
                return;
            }

            string[] splitted = inputLine.Split(' ');
            this.username = splitted[0];
            this.password = splitted[1];
            this.reminder = splitted.Length < 3 ? "" : splitted[2];
            try
            {
                string[] lines = System.IO.File.ReadAllLines(username + ".txt");
                foreach (var item in lines)
                {
                    recievedMails.Add(new email(item));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}