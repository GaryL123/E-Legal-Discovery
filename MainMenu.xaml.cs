using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Login_WPF
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        private ObservableCollection<Project> Projects = new ObservableCollection<Project>();
        private ProjectsDbService projectManager = new ProjectsDbService();
        private int LoggedInUserId { get; set; }
        private string Username { get; set; }

        public MainMenu(int loggedInUserId)
        {
            InitializeComponent();
            LoggedInUserId = loggedInUserId;
            
            Projects = projectManager.Select(loggedInUserId);
            ProjectsDataGrid.ItemsSource = Projects;
            string username = User.GetUsernameById(LoggedInUserId);

            // Test if the username is being retrieved correctly
            if (string.IsNullOrEmpty(username))
            {
                WelcomeTextBlock.Text = "Welcome user not found";
            }
            else
            {
                WelcomeTextBlock.Text = $"Welcome {username}";
            }
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the button that was clicked
            Button deleteButton = (Button)sender;

            // Retrieve the Project object associated with the button
            Project projectToDelete = (Project)deleteButton.DataContext;

            // Confirm delete action
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete this project?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (projectManager.Delete(projectToDelete.ProjectId))
                {
                    MessageBox.Show("Project deleted successfully.");
                    Projects.Remove(projectToDelete); // Update the ObservableCollection
                }
                else
                {
                    MessageBox.Show("There was an error deleting the project.");
                }
            }
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            Button openProjectButton = (Button)sender;
            Project projectToOpen = (Project)openProjectButton.DataContext;

            Window.GetWindow(this).Content = new EmailManagementPage(projectToOpen.ProjectId);
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            // Assuming that 'this.NavigationService' is available in the context
            Window.GetWindow(this).Content = new NewProjectPage(LoggedInUserId);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Assuming that 'this.NavigationService' is available in the context
            Window.GetWindow(this).Content = new Login();
        }
    }
}
