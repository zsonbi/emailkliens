using System;

namespace emailkliens
{
    internal class email
    {
        public string sender;//The one who sent the email
        public string subject;//The subject of the email
        public string recievedate;//When the email was written
        public bool IsItRead;//Is it read -,-
        public string content;//The content of the email

        /// <summary>
        /// When Reading in from a file
        /// </summary>
        /// <param name="raw">a row of a file</param>
        public email(string raw)
        {
            string[] splitted = raw.Split(' ');
            this.sender = splitted[0];
            subject = splitted[1];
            recievedate = splitted[2];
            IsItRead = splitted[3] == "olvasott";
            content = raw.Split('{')[1];
            content = content.TrimEnd('}');
        }

        //---------------------------------------------------
        /// <summary>
        /// When the user makes a new email
        /// </summary>
        /// <param name="username"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        public email(string username, string subject, string content)
        {
            this.sender = username;
            this.subject = subject;
            this.content = content;
            this.recievedate = DateTime.Now.Month + "." + DateTime.Now.Day;
            this.IsItRead = false;
        }
    }
}