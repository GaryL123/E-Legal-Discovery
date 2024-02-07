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
        public Email SelectedEmail { get; set; }
        public int loggedInUserId { get; set; }

        private void EmailsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEmail = EmailsListView.SelectedItem as Email;
            if (selectedEmail != null)
            {
                SenderTextBlock.Text = selectedEmail.SenderAddress;
                ReceiverTextBlock.Text = selectedEmail.ReceiverAddress;
                DateSentTextBlock.Text = selectedEmail.DateSent.ToString();
                SubjectTextBlock.Text = selectedEmail.Subject;
                ContentTextBlock.Text = selectedEmail.EmailContent;
            }
        }

        public EmailManagementPage(int projectId, int loggedInUserId)
        {
            InitializeComponent();
            CurrentProjectId = projectId;
            emailManager = new EmailDbService();
            Emails = new ObservableCollection<Email>(emailManager.GetEmailsByProjectId(projectId));
            EmailsListView.ItemsSource = Emails;
            this.loggedInUserId = loggedInUserId;
        }

        public void Back_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Content = new MainMenu(loggedInUserId);
        }

        private void UploadEmails_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Email files (*.eml)|*.eml" 
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {
                        Email email = Email.ExtractEmailMetadata(filename);
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
