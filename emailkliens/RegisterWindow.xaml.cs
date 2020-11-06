using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace emailkliens
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        //***************************************************************
        //Private Methods
        //Makes sure that only one word is inputted
        private bool OnlyOneWord(string input)
        {
            return input.Split(' ').Length > 1;
        }

        //*********************************************************
        //Handlers
        //Checks if the Reminder is correct paint it red if it's incorrect green if it's correct
        private void Remindertbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox.Text.Length == 0)
            {
                tbox.Background = Brushes.Green;
                return;
            }

            if (!OnlyOneWord(tbox.Text) && tbox.Text.Length <= 15)
                tbox.Background = Brushes.Green;
            else
                tbox.Background = Brushes.Red;
        }

        //-------------------------------------------------------------------------------
        //Checks if the Password is correct paint it red if it's incorrect green if it's correct
        //Also it checks if the passwords are a match
        private void Passwordtbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbox = sender as TextBox;

            if (tbox.Text.Length <= 10 && tbox.Text.Length >= 8 && !OnlyOneWord(tbox.Text))
            {
                tbox.Background = Brushes.Green;
                if (Passwordtbox.Text == RepeatPasswordtbox.Text)
                {
                    Passwordtbox.Background = Brushes.Green;
                    RepeatPasswordtbox.Background = Brushes.Green;
                }
            }
            else
                tbox.Background = Brushes.Red;
            if (Passwordtbox.Text != RepeatPasswordtbox.Text)
            {
                Passwordtbox.Background = Brushes.Red;
                RepeatPasswordtbox.Background = Brushes.Red;
            }
        }

        //------------------------------------------------------------------
        //Checks if the Reminder is correct paint it red if it's incorrect green if it's correct
        private void Usernametbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox.Text.Length == 0)
            {
                tbox.Background = Brushes.Red;
                return;
            }
            if (!OnlyOneWord(tbox.Text) && tbox.Text.Length <= 15)
                tbox.Background = Brushes.Green;
            else
                tbox.Background = Brushes.Red;
        }

        //-------------------------------------------------------
        //Makes sure that only numbers and the abc-s characters are enabled
        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string input = e.Key.ToString();

            if (input.Length == 1 && !input.StartsWith("D"))
            {
                input = input.ToLower();
                if (Convert.ToChar(input) >= 'a' && Convert.ToChar(input) <= 'z')
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            else
                e.Handled = false;
        }
    }
}