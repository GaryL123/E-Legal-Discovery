using System;

class Program
{
    static void Main()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Toor11!!;Database=postgres";

        // Create an email object and set the necessary properties
        Email email = new Email();
        Console.Write("Enter the path to the email file: ");
        string emailFilePath = Console.ReadLine();
        email.file_path = emailFilePath;

        // Extract metadata from the email file
        email.ExtractMetadata();

        // Save the email to the database
        email.project_id = 1; // Set the project ID as needed
        email.SaveToDatabase(connectionString);

        // Duplicate the email file to a specified location
        email.DuplicateFile("D:\\EmailDuplicatesTest"); // Set the desired destination path

        // Retrieve and print the email information from the database
        email.RetrieveAndPrintFromDatabase(connectionString);
    }
}
