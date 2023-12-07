using System;
using System.IO;
using Npgsql;
using MimeKit;

public class Email
{
    // Public properties
    public int email_id { get; set; }
    public int project_id { get; set; }
    public string sender_address { get; set; }
    public string receiver_address { get; set; }
    public string subject { get; set; }
    public DateTime date_sent { get; set; }
    public string file_path { get; set; }


    // Method to save the email to the database
    public void SaveToDatabase(string connectionString)
    {
        // Extract metadata from the email file
        ExtractMetadata();

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO emails (project_id, sender_address, receiver_address, subject, date_sent, file_path) " +
                                  "VALUES (@project_id, @sender_address, @receiver_address, @subject, @date_sent, @file_path) RETURNING email_id";

                cmd.Parameters.AddWithValue("project_id", project_id);
                cmd.Parameters.AddWithValue("sender_address", sender_address);
                cmd.Parameters.AddWithValue("receiver_address", receiver_address);
                cmd.Parameters.AddWithValue("subject", subject);
                cmd.Parameters.AddWithValue("date_sent", date_sent);

                // Use the original file name as part of the file path
                string fileName = Path.GetFileName(file_path);
                string destinationFilePath = Path.Combine("D:\\EmailFiles", fileName);
                string copyFilePath = Path.Combine("D:\\copyEmailFiles", fileName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                Directory.CreateDirectory(Path.GetDirectoryName(copyFilePath));

                cmd.Parameters.AddWithValue("file_path", destinationFilePath);

                // Copy the original .eml file to the destination
                File.Copy(file_path, destinationFilePath, true);
                File.Copy(file_path, copyFilePath, true);

                // Retrieve the generated email_id after insertion
                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    // Method to download the email file
    public void DownloadFile(string connectionString, int emailId, string destinationPath)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT file_path FROM emails WHERE email_id = @email_id";
                cmd.Parameters.AddWithValue("email_id", emailId);

                // ExecuteScalar returns the file path
                var filePath = cmd.ExecuteScalar() as string;

                if (!string.IsNullOrEmpty(filePath))
                {
                    // Build the source and destination file paths
                    string sourceFilePath = filePath;
                    string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(filePath));
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));

                    // Copy the file to the destination
                    File.Copy(sourceFilePath, destinationFilePath, true);

                    Console.WriteLine($"Email file downloaded to: {destinationFilePath}");
                }
                else
                {
                    Console.WriteLine("Email file not found in the database.");
                }
            }

            conn.Close();
        }
    }


    // Method to extract metadata from the email file
    public void ExtractMetadata()
    {
        // Parse the email file using MimeKit
        var message = MimeMessage.Load(file_path);

        // Extract metadata
        sender_address = message.From.ToString();
        receiver_address = message.To.ToString();
        subject = message.Subject;
        date_sent = message.Date.DateTime;
    }

    // Method to duplicate the email file to a specified location
    public void DuplicateFile(string destinationPath)
    {
        // Ensure the destination directory exists
        Directory.CreateDirectory(destinationPath);

        // Build the destination file path
        string destinationFilePath = Path.Combine(destinationPath, $"{email_id}_{Path.GetFileName(file_path)}");

        // Copy the file to the destination
        File.Copy(file_path, destinationFilePath, true);
    }

    // Method to retrieve email information from the database and print it to the console
    public void RetrieveAndPrintFromDatabase(string connectionString)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM emails WHERE email_id = @email_id";

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Retrieved Email Information from Database:");
                        Console.WriteLine($"Email ID: {reader["email_id"]}");
                        Console.WriteLine($"Sender Address: {reader["sender_address"]}");
                        Console.WriteLine($"Receiver Address: {reader["receiver_address"]}");
                        Console.WriteLine($"Subject: {reader["subject"]}");
                        Console.WriteLine($"Date Sent: {reader["date_sent"]}");
                        Console.WriteLine($"File Path: {reader["file_path"]}");
                    }
                }
            }

            conn.Close();
        }
    }

    //prints out email information to choose which one to open for editing
    public static void DisplayEmailsForSelection(List<Email> emails)
    {
        Console.WriteLine("Available Emails for Editing:");

        foreach (var email in emails)
        {
            Console.WriteLine($"Email ID: {email.email_id}, Subject: {email.subject}, Sender: {email.sender_address}");
        }

        Console.WriteLine("Enter the Email ID to edit or '0' to go back:");
    }

}
