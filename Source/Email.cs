using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_WPF
{
    public class Email
    {
        public int EmailId { get; set; }
        public int ProjectId { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public string Subject { get; set; }
        public DateTime DateSent { get; set; }
        public string EmailContent { get; set; }
        public string FilePath { get; set; }

        // Constructor for creating an Email object with no initial data
        public Email()
        {
        }

        // Constructor for creating an Email object with initial data
        public Email(int emailId, int projectId, string senderAddress, string receiverAddress,
                     string subject, DateTime dateSent, string emailContent, string filePath)
        {
            EmailId = emailId;
            ProjectId = projectId;
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            Subject = subject;
            DateSent = dateSent;
            EmailContent = emailContent;
            FilePath = filePath;
        }

        // Extract metadata from .eml file and create an Email object
        public static Email ExtractEmailMetadata(string filepath)
        {
            var email = new Email();

            try
            {
                // Load the .eml file
                var message = MimeMessage.Load(filepath);

                // Extract the metadata from the MimeMessage object
                email.SenderAddress = message.From.Mailboxes.FirstOrDefault()?.Address;
                email.ReceiverAddress = message.To.Mailboxes.FirstOrDefault()?.Address;
                email.Subject = message.Subject;
                email.DateSent = message.Date.DateTime;
                email.EmailContent = message.TextBody; // Or use message.HtmlBody if you want HTML content

                // Additional properties like FilePath can be set here
                email.FilePath = filepath;
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as if the file is not found or the email is not in a correct format
                Console.WriteLine("An error occurred while extracting email metadata: " + ex.Message);
            }

            return email;
        }
    }

}
