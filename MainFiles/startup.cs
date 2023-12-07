class Startup
{
    static void Main()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Toor11!!;Database=postgres";
        User currentUser = null; // Initialize currentUser
        Project currentProject = null; // Initialize currentProject

        // Keep the application running until the user decides to log out or close the program
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Log In / Log Out");
            Console.WriteLine("2. Sign Up");

            // Updated menu options
            if (currentUser != null)
            {
                Console.WriteLine("3. Create Project");
                Console.WriteLine("4. Open Project");

                // Display additional options when a project is open
                if (currentProject != null)
                {
                    Console.WriteLine("5. List Emails");
                   // Console.WriteLine("6. Edit Email");
                }
            }

            Console.WriteLine("6. Exit");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        User.LogInOut(connectionString, ref currentUser);
                        break;
                    case 2:
                        User.SignUp(connectionString, currentUser);
                        break;
                    case 3:
                        if (currentUser != null)
                        {
                            currentUser.CreateProject(connectionString);
                        }
                        else
                        {
                            Console.WriteLine("You need to log in first.");
                        }
                        break;
                    case 4:
                        if (currentUser != null)
                        {
                            // Assign the opened project to currentProject
                            currentProject = Project.OpenProject(connectionString, currentUser);
                        }
                        else
                        {
                            Console.WriteLine("You need to log in first.");
                        }
                        break;
                    case 5:
                        if (currentUser != null && currentProject != null)
                        {
                            // Call a method to list emails for the current project
                            currentProject.ListEmails(connectionString);
                        }
                        else
                        {
                            Console.WriteLine("You need to log in and open a project first.");
                        }
                        break;
                    //case 6:
                    //    if (currentUser != null && currentProject != null)
                    //    {
                    //        // Call a method to edit an email for the current project
                    //        currentProject.EditEmail(connectionString);
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("You need to log in and open a project first.");
                    //    }
                    //    break;
                    case 6:
                        Console.WriteLine("Goodbye!");
                        return;
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
}
