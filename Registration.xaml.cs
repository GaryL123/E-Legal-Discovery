using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Npgsql;
using System.Windows.Controls.Primitives;

namespace Login_WPF
{
    /// <summary>  
    /// Interaction logic for Registration.xaml  
    /// </summary>  
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }

        public void loadLoginPage(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new Login();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            textBoxUsername.Text = "";
            textBoxEmail.Text = "";
            passwordBox1.Password = "";
            passwordBoxConfirm.Password = "";
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            // Ensure correct user input

            if (textBoxEmail.Text.Length == 0)
            {
                errormessage.Text = "Enter an email.";
                textBoxEmail.Focus();
            }
            else if (!Regex.IsMatch(textBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                errormessage.Text = "Enter a valid email.";
                textBoxEmail.Select(0, textBoxEmail.Text.Length);
                textBoxEmail.Focus();
            }
            else
            {
                string username = textBoxUsername.Text;
                string email = textBoxEmail.Text;
                string password = passwordBox1.Password;

                if (passwordBox1.Password.Length == 0)
                {
                    errormessage.Text = "Enter password.";
                    passwordBox1.Focus();
                }
                else if (passwordBoxConfirm.Password.Length == 0)
                {
                    errormessage.Text = "Enter Confirm password.";
                    passwordBoxConfirm.Focus();
                }
                else if (passwordBox1.Password != passwordBoxConfirm.Password)
                {
                    errormessage.Text = "Password and confirmed password do not match!";
                    passwordBoxConfirm.Focus();
                }
                else
                {
                    errormessage.Text = "";

                    // Database Connection
                    var connectionString = "Host=localhost;Username=postgres;Password=12345678;Database=postgres";

                    // Attempt to insert the user and navigate to the main menu if successful
                    try
                    {
                        int newUserId = User.InsertUser(connectionString, username, password, email);

                        // If the user is successfully created, navigate to MainMenu with the new user ID
                        if (newUserId > 0)
                        {
                            Window.GetWindow(this).Content = new MainMenu(newUserId);
                        }
                        else
                        {
                            errormessage.Text = "Registration failed. User could not be created.";
                        }
                    }
                    catch (Exception ex)
                    {
                        errormessage.Text = "Error: " + ex.Message;
                    }
                }
            }
        }
    }
}
