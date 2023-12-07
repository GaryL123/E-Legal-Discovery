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
    /// Interaction logic for EmailManagementPage.xaml
    /// </summary>
    public partial class EmailManagementPage : Page
    {
        private ObservableCollection<Email> Emails { get; set; }
        private int CurrentProjectId { get; set; }
        private EmailDbService emailManager { get; set; }



        public EmailManagementPage(int projectId)
        {
            InitializeComponent();
            CurrentProjectId = projectId;
            emailManager = new EmailDbService();
            Emails = new ObservableCollection<Email>(emailManager.GetEmailsByProjectId(projectId));
            EmailsDataGrid.ItemsSource = Emails;
        }

        private void UploadEmails_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Email files (*.eml)|*.eml" // Adjust the filter according to your needs
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {
                        Email email = Email.ExtractEmailMetadata(filename); // Call the static method on the Email class
                        email.ProjectId = CurrentProjectId;
                        if (emailManager.InsertEmail(email))
                        {
                            Emails.Add(email);
                        }
                        else
                        {
                            MessageBox.Show("Failed to insert email into the database.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error processing file: " + ex.Message);
                    }
                }
            }
        }



    }

}
