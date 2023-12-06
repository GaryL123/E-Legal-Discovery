class Startup
{
    static void Main()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=Toor11!!;Database=postgres";
        User currentUser = null; // Initialize currentUser

        // Keep the application running until the user decides to log out or close the program
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Log In / Log Out");
            Console.WriteLine("2. Sign Up");
            Console.WriteLine("3. Create Project");
            Console.WriteLine("4. Exit");

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
                            currentUser.CreateProject(connectionString);
                        
                        else
                            Console.WriteLine("You need to log in first.");
                        
                        break;
                    case 4:
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
