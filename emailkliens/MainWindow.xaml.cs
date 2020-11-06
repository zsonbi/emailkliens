using System.Collections.Generic;
using System.Windows;

namespace emailkliens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataBase db;
        private RegisterWindow registerwindow;
        private List<email> emails = new List<email>();
        private byte orderType = 0;

        public MainWindow()
        {
            InitializeComponent();
            db = new DataBase();
        }

        //********************************************************
        //Private Mehods
        //Clears the out the Listbox and the TextBoxes when the user Logs out
        private void ClearOut()
        {
            emaillistbox.Items.Clear();
            Usernametbox.Text = "";
            Passwordtbox.Text = "";
            headertbox.Text = "";
            subjecttbox.Text = "";
            ContentTbox.Text = "";
        }

        //-------------------------------------------------------------
        /// <summary>
        /// Swaps the Visibilities for the buttons and readonly for the username and for the password
        /// </summary>
        /// <param name="current"></param>
        private void SwapVisibilities(bool current)
        {
            Usernametbox.IsReadOnly = current;
            Passwordtbox.IsReadOnly = current;
            Loginbtn.Visibility = current ? Visibility.Hidden : Visibility.Visible;
            Registerbtn.Visibility = current ? Visibility.Hidden : Visibility.Visible;
            Reminderbtn.Visibility = current ? Visibility.Hidden : Visibility.Visible;
            Logoutbtn.Visibility = current ? Visibility.Visible : Visibility.Hidden;
            SendNewLetterbtn.Visibility = current ? Visibility.Visible : Visibility.Hidden;
            insideMenuWrap.Visibility = current ? Visibility.Visible : Visibility.Hidden;
        }

        //--------------------------------------------------------
        //Creates the window where the user can register
        private void CreateRegisterWindow()
        {
            registerwindow = new RegisterWindow();
            registerwindow.Registerbtn.Click += FinishRegister;
            registerwindow.Show();
        }

        //--------------------------------------------------------
        /// <summary>
        /// Shows the emails and orders them according to the ordertype
        /// </summary>
        /// <param name="orderType">0: sender, 1:subject, 2:date</param>
        private void GetEmails(byte orderType = 0)
        {
            emails = db.GetEmails(orderType);
            this.orderType = orderType;
            if (emails.Count == 0)
            {
                return;
            }

            List<string> stringList = new List<string>();
            foreach (var item in emails)
            {
                stringList.Add((item.IsItRead ? "Olvasott" : "Olvasatlan") + " " + item.sender + " " + (item.subject == "nincs_megadva" ? "" : item.subject) + " " + item.recievedate);
            }
            emaillistbox.Items.Clear();
            foreach (var item in stringList)
            {
                emaillistbox.Items.Add(item);
            }
        }

        //**************************************************
        //Handlers
        //Logs the user out
        private void Logoutbtn_Click(object sender, RoutedEventArgs e)
        {
            db.Logout();
            ClearOut();
            SwapVisibilities(false);
        }

        //-------------------------------------------------------------
        //Pops a new window up for the Registration process
        private void Registerbtn_Click(object sender, RoutedEventArgs e)
        {
            CreateRegisterWindow();
        }

        //-------------------------------------------------------------
        //Pops a new window up for the Registration process
        private void FinishRegister(object sender, RoutedEventArgs e)
        {
            switch (db.CreateNewAccount(registerwindow.Usernametbox.Text, registerwindow.Passwordtbox.Text, registerwindow.Remindertbox.Text))
            {
                case 0:
                    registerwindow.Close();
                    MessageBox.Show("Felhasználó regisztrálva");
                    break;

                case 1:
                    MessageBox.Show("Nem megfelelő felhasználónév");
                    break;

                case 2:
                    MessageBox.Show("Nem megfelelő jelszó");
                    break;

                case 3:
                    MessageBox.Show("Nem megfelelő emlékeztető");
                    break;

                case 4:
                    MessageBox.Show("Ez a felhasználónév már foglalt");
                    break;

                default:
                    break;
            }
        }

        //---------------------------------------------------------------
        //Gets the user's reminder
        private void Reminderbtn_Click(object sender, RoutedEventArgs e)
        {
            if (Usernametbox.Text == "")
            {
                MessageBox.Show("Írjon valamit a Felhasználónévhez előtte");
                return;
            }
            MessageBox.Show(db.GetReminder(Usernametbox.Text));
        }

        //-------------------------------------------------------------
        //Starts the Login Process
        private void Loginbtn_Click(object sender, RoutedEventArgs e)
        {
            switch (db.Login(Usernametbox.Text, Passwordtbox.Text))
            {
                case 0:
                    Passwordtbox.Text = "";
                    SwapVisibilities(true);
                    GetEmails();
                    break;

                case 1:
                    MessageBox.Show("Nincs ilyen felhasználó");
                    break;

                case 2:
                    MessageBox.Show("Nem megfelelő jelszó");
                    break;

                default:
                    break;
            }
        }

        //-------------------------------------------------------------
        //Sends the email
        private void Sendbtn_Click(object sender, RoutedEventArgs e)
        {
            if (headertbox.Text == "")
            {
                MessageBox.Show("Nincs címzett");
                return;
            }
            switch (db.WriteLetter(headertbox.Text, subjecttbox.Text == "" ? "nincs_megadva" : subjecttbox.Text, ContentTbox.Text))
            {
                case 0:
                    MessageBox.Show("Sikeresen elküldve");
                    break;

                case 1:
                    MessageBox.Show("Nincs ilyen felhasználó a rendszerben");
                    break;

                case 2:
                    MessageBox.Show("A tárgy nem megfelelő");
                    break;

                case 3:
                    MessageBox.Show("A tartalom hosszabb mint 100 karakter");
                    break;
            }
        }

        //-------------------------------------------------------------------
        //When the Mainwindow Closes close the Registerwindow also
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (registerwindow != null)
            {
                registerwindow.Close();
            }
        }

        //----------------------------------------------------------------
        //Adds the IncomingLetters to the list
        private void ReadSelectedLetterbtn_Click(object sender, RoutedEventArgs e)
        {
            if (emaillistbox.SelectedIndex == -1)
                return;
            Sendbtn.Visibility = Visibility.Hidden;
            headertbox.IsReadOnly = true;
            subjecttbox.IsReadOnly = true;
            ContentTbox.IsReadOnly = true;

            db.ReadLetter(emaillistbox.SelectedIndex);
            headerLabel.Content = "Küldő";
            headertbox.Text = emails[emaillistbox.SelectedIndex].sender;
            subjecttbox.Text = emails[emaillistbox.SelectedIndex].subject == "nincs_megadva" ? "" : emails[emaillistbox.SelectedIndex].subject;
            ContentTbox.Text = emails[emaillistbox.SelectedIndex].content;
            int temp = emaillistbox.SelectedIndex;
            GetEmails(orderType);
            emaillistbox.SelectedIndex = temp;
        }

        //----------------------------------------------------------------
        private void DeleteSelectedLetterbtn_Click(object sender, RoutedEventArgs e)
        {
            if (emaillistbox.SelectedIndex == -1)
                return;
            db.DeleteLetter(emaillistbox.SelectedIndex);
            GetEmails(orderType);
        }

        //-------------------------------------------------------
        //This button will start the letter writing
        private void SendNewLetterbtn_Click(object sender, RoutedEventArgs e)
        {
            headertbox.IsReadOnly = false;
            subjecttbox.IsReadOnly = false;
            ContentTbox.IsReadOnly = false;
            headerLabel.Content = "Címzett";
            headertbox.Text = "";
            subjecttbox.Text = "";
            ContentTbox.Text = "";
            Sendbtn.Visibility = Visibility.Visible;
        }

        //----------------------------------------------------------
        // This button will Sort the emails according to the radio button state
        private void Sortbtn_Click(object sender, RoutedEventArgs e)
        {
            byte orderType = (byte)((bool)SenderRadiobtn.IsChecked ? 0 : (bool)SubjectRadiobtn.IsChecked ? 1 : 2);
            GetEmails(orderType);
        }
    }
}