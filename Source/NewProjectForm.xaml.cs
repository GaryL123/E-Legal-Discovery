using System;
using System.Collections.Generic;
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
    /// Interaction logic for NewProjectForm.xaml
    /// </summary>
    public partial class NewProjectPage : Page
    {
        private int OwnerId { get; set; }

        public NewProjectPage(int ownerId)
        {
            InitializeComponent();
            OwnerId = ownerId;
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            var projectName = ProjectNameTextBox.Text;
            var projectDescription = ProjectDescriptionTextBox.Text;

            if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(projectDescription))
            {
                MessageBox.Show("Project Name and Description are required.");
                return;
            }

            var project = new Project
            {
                ProjectName = projectName,
                Description = projectDescription,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                OwnerId = this.OwnerId
            };

            ProjectsDbService projectsDbService = new ProjectsDbService();
            if (projectsDbService.Insert(project))
            {
                MessageBox.Show("Project created successfully.");
                // Optionally navigate back to the main menu or clear the form for a new entry
            }
            else
            {
                MessageBox.Show("There was an error creating the project.");
            }

            Window.GetWindow(this).Content = new MainMenu(OwnerId);
        }
    }

}
