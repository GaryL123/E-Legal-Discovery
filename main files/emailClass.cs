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
    public string email_content { get; set; }
    public string file_path { get; set; }

    // Method to save the email to the database
public void SaveToDatabase(string connectionString)
{
    using (var conn = new NpgsqlConnection(connectionString))
    {
        conn.Open();

        using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO emails (project_id, sender_address, receiver_address, subject, date_sent, email_content, file_path) " +
                              "VALUES (@project_id, @sender_address, @receiver_address, @subject, @date_sent, @email_content, @file_path) RETURNING email_id";

            cmd.Parameters.AddWithValue("project_id", project_id);
            cmd.Parameters.AddWithValue("sender_address", sender_address);
            cmd.Parameters.AddWithValue("receiver_address", receiver_address);
            cmd.Parameters.AddWithValue("subject", subject);
            cmd.Parameters.AddWithValue("date_sent", date_sent);
            cmd.Parameters.AddWithValue("email_content", email_content);
            cmd.Parameters.AddWithValue("file_path", file_path);

            // Retrieve the generated email_id after insertion
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
        email_content = message.TextBody;

        // Print extracted metadata
        //Console.WriteLine("Sender Address: " + sender_address);
        //Console.WriteLine("Receiver Address: " + receiver_address);
        //Console.WriteLine("Subject: " + subject);
        //Console.WriteLine("Date Sent: " + date_sent);
        //Console.WriteLine("Email Content:\n" + email_content);
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
                        Console.WriteLine($"Project ID: {reader["project_id"]}");
                        Console.WriteLine($"Sender Address: {reader["sender_address"]}");
                        Console.WriteLine($"Receiver Address: {reader["receiver_address"]}");
                        Console.WriteLine($"Subject: {reader["subject"]}");
                        Console.WriteLine($"Date Sent: {reader["date_sent"]}");
                        Console.WriteLine($"Email Content: {reader["email_content"]}");
                        Console.WriteLine($"File Path: {reader["file_path"]}");
                    }
                }
            }

            conn.Close();
        }
    }
}
