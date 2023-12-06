using System;
using System.Threading;

class Program
{
    static User currentUser; // Variable to store the currently logged-in user

    static void Main()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Toor11!!;Database=postgres";

        // Keep the application running until the user decides to log out or close the program
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Log Out / Log In");
            Console.WriteLine("2. Close the Program");
            Console.WriteLine("3. Create Project");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        LogInOut(connectionString);
                        break;
                    case 2:
                        Console.WriteLine("Goodbye!");
                        return;
                    case 3:
                        if (currentUser != null)
                        {
                            CreateProject(connectionString);
                        }
                        else
                        {
                            Console.WriteLine("You need to log in first.");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }

    // Method to log out the current user or log in a new user
    static void LogInOut(string connectionString)
    {
        if (currentUser != null)
        {
            currentUser = null;
            Console.WriteLine("You have been logged out.");

            // Add a delay and clear the console screen
            Thread.Sleep(2000); // Sleep for 2 seconds
            Console.Clear();
        }
        else
        {
            Console.Write("Enter your username: ");
            string enteredUsername = Console.ReadLine();

            Console.Write("Enter your password: ");
            string enteredPassword = Console.ReadLine();

            // Authenticate the user
            User authenticatedUser;
            bool isAuthenticated = User.AuthenticateUser(enteredUsername, enteredPassword, connectionString, out authenticatedUser);

            if (isAuthenticated)
            {
                // Store the currently logged-in user
                currentUser = authenticatedUser;

                Console.WriteLine($"Welcome, {currentUser.UserName}!");

                // Add a delay and clear the console screen
                Thread.Sleep(2000); // Sleep for 2 seconds
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Authentication failed. Returning to the main menu.");
            }
        }
    }

    // Method to create a new project
    static void CreateProject(string connectionString)
    {
        Project newProject = currentUser.CreateProject(connectionString);

        // Ask the user if they want to add emails to the project
        Console.Write("Do you want to add emails to this project? (yes/no): ");
        string addEmailsChoice = Console.ReadLine();

        if (addEmailsChoice.ToLower() == "yes")
        {
            // call a method to handle the email addition process here
            Console.WriteLine("Emails can be added to the project here (not implemented yet).");
        }

        // Add a delay and clear the console screen
        Thread.Sleep(2000); // Sleep for 2 seconds
        Console.Clear();
    }
}
