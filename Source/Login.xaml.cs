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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
namespace Login_WPF
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>   
    public partial class Login : Page
    {
        private static User LoggedInUser { get; set; }
        public Login()
        {
            InitializeComponent();
        }

        Registration registration = new Registration();
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxUsername.Text.Length == 0)
            {
                errormessage.Text = "Enter a username.";
                textBoxUsername.Focus();
            }
            else
            {
                var connectionString = "Host=localhost;Username=postgres;Password=12345678;Database=postgres";
                string username = textBoxUsername.Text;
                string password = passwordBox1.Password;

                var user = User.AuthenticateUser(connectionString, username, password);
                if (user != null)
                {
                    LoggedInUser = user; // Store the logged-in user
                    var mainMenu = new MainMenu(user.user_id); // Pass the user ID to the MainMenu constructor
                    Window.GetWindow(this).Content = mainMenu;
                }
                else
                {
                    errormessage.Text = "Username/password was invalid.";
                }
            }
        }
        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new Registration();
        }
    }
}