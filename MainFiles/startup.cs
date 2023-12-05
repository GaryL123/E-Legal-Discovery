using System;

class Program
{
    static void Main()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Toor11!!;Database=postgres";

        // Create an email object and set the necessary properties
        Email email = new Email();
        // Console.Write("Enter the path to the email file: ");
        // string emailFilePath = Console.ReadLine();
        // email.file_path = emailFilePath;

        // Extract metadata from the email file
        // email.ExtractMetadata();

        // Save the email to the database
        // email.SaveToDatabase(connectionString);

        // Duplicate the email file to a specified location
        // email.DuplicateFile("D:\\EmailDuplicatesTest"); // Set the desired destination path

        // Retrieve and print the email information from the database
        // email.RetrieveAndPrintFromDatabase(connectionString);

        // Create a user object and set generic information
        User newUser = new User
        {
            UserName = "TestUser",
            Password = "TestPassword",
            Email = "testuser@example.com",
            role = "User"
        };

        // Insert the user into the database
        //newUser.InsertUser(connectionString);

        // Test the user authentication
        string enteredUsername = "TestUser";
        string enteredPassword = "TestPassword";

        // Authenticate the user
        bool isAuthenticated = newUser.AuthenticateUser(enteredUsername, enteredPassword, connectionString);

        // Display authentication result
        Console.WriteLine($"Authentication Result: {isAuthenticated}");
    }

    // Helper method to print the contents of the user table
    private static void PrintUserTable(string connectionString)
    {
        using (var conn = new Npgsql.NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new Npgsql.NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM users";

                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("User Table Contents:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"User ID: {reader["user_id"]}");
                        Console.WriteLine($"Username: {reader["username"]}");
                        Console.WriteLine($"Password: {reader["password"]}");
                        Console.WriteLine($"Email: {reader["email"]}");
                        Console.WriteLine($"Role: {reader["role"]}");
                        Console.WriteLine("---------------------");
                    }
                }
            }

            conn.Close();
        }
    }
}
