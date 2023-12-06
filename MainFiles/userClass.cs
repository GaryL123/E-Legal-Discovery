using System;
using Npgsql;
using System.Text;

public class User
{
    // Public properties
    public string UserName { get; set; }
    public int user_id { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string role { get; set; }


    // Default constructor
    public User()
    {
        UserName = string.Empty;
        Password = string.Empty;
        Email = string.Empty;
        role = string.Empty;
    }


    // Constructor with parameters
    public User(string userName, string password, string email, string role)
    {
        this.UserName = userName;
        this.Password = password;
        this.Email = email;
        this.role = role;
    }

    // Method to sign up a new user
    public static void SignUp(string connectionString, User currentUser)
    {
        if (currentUser == null)
        {
            User newUser = new User();

            Console.Write("Enter your username: ");
            newUser.UserName = Console.ReadLine();

            Console.Write("Enter your password: ");
            newUser.Password = Console.ReadLine();

            Console.Write("Enter your email: ");
            newUser.Email = Console.ReadLine();

            // Set a default role for the new user
            newUser.role = "User";

            // Insert the new user into the database
            newUser.InsertUser(connectionString);

            Console.WriteLine("User registration successful!");
        }
        else
        {
            Console.WriteLine("You are already logged in. Please log out first.");
        }
    }
    // Method to insert a user into the database
    public void InsertUser(string connectionString)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            // Hash the password using a strong hashing algorithm (e.g., SHA-256)
            string hashedPassword = HashPassword(Password);

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO users (username, password, email, role) VALUES (@username, @password, @email, @role)";

                cmd.Parameters.AddWithValue("username", UserName);
                cmd.Parameters.AddWithValue("password", hashedPassword); // Store the hashed password
                cmd.Parameters.AddWithValue("email", Email);
                cmd.Parameters.AddWithValue("role", role);

                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }
    }


    // Method to authenticate a user and return a boolean indicating success or failure
    public static bool AuthenticateUser(string enteredUsername, string enteredPassword, string connectionString, out User authenticatedUser)
    {
        authenticatedUser = null;

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT user_id, username, password, email, role FROM users WHERE lower(username) = lower(@username)";
                cmd.Parameters.AddWithValue("username", enteredUsername);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // User found, retrieve information from the database
                        authenticatedUser = new User
                        {
                            user_id = (int)reader["user_id"],
                            UserName = reader["username"].ToString(),
                            Password = reader["password"].ToString(),
                            Email = reader["email"].ToString(),
                            role = reader["role"].ToString()
                        };

                        // Hash the entered password for comparison
                        string enteredHashedPassword = HashPassword(enteredPassword);

                        // Compare hashed passwords
                        return string.Equals(authenticatedUser.Password, enteredHashedPassword, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // User not found
                        return false;
                    }
                }
            }
        }
    }


    // Helper method to hash a password
    private static string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            // Compute hash from password bytes
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert hashed bytes to string (hex format)
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    // Method to log out the current user or log in a new user
    public static void LogInOut(string connectionString, ref User currentUser)
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
            bool isAuthenticated = AuthenticateUser(enteredUsername, enteredPassword, connectionString, out authenticatedUser);

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

    // Helper method to retrieve a user by username from the database
    public static User GetUserByUsername(string username, string connectionString)
    {
        User user = new User();
        user.UserName = username;

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM users WHERE lower(username) = lower(@username)";
                cmd.Parameters.AddWithValue("username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user.user_id = (int)reader["user_id"];
                        user.Password = reader["password"].ToString();
                        user.Email = reader["email"].ToString();
                        user.role = reader["role"].ToString();
                    }
                }
            }

            conn.Close();
        }

        return user;
    }

    // Method to create a new project
    public void CreateProject(string connectionString)
    {
        Project newProject = new Project();

        Console.Write("Enter project name: ");
        newProject.ProjectName = Console.ReadLine();

        Console.Write("Enter project description: ");
        newProject.Description = Console.ReadLine();

        // Loop until a valid start date is entered
        while (true)
        {
            Console.Write("Enter project start date (yyyy-MM-dd): ");
            if (DateOnly.TryParse(Console.ReadLine(), out DateOnly startDate))
            {
                newProject.StartDate = startDate;
                break; // Exit the loop if a valid date is entered
            }
            else
            {
                // Handle parsing error or set a default value
                Console.WriteLine("Invalid date format. Please enter a valid date (yyyy-MM-dd).");
            }
        }

        // Loop until a valid end date is entered
        while (true)
        {
            Console.Write("Enter project end date (yyyy-MM-dd): ");
            if (DateOnly.TryParse(Console.ReadLine(), out DateOnly endDate))
            {
                newProject.EndDate = endDate;
                break; // Exit the loop if a valid date is entered
            }
            else
            {
                // Handle parsing error or set a default value
                Console.WriteLine("Invalid date format. Please enter a valid date (yyyy-MM-dd).");
            }
        }

        newProject.OwnerId = user_id; // Assign the owner ID

        // Save the project to the database
        newProject.SaveToDatabase(connectionString);

        Console.WriteLine($"Project '{newProject.ProjectName}' created successfully!");
       
        // Get the project_id of the newly created project
        int projectId = Project.GetProjectIdByName(newProject.ProjectName, connectionString);

        // Ask the user if they want to add emails to the project
        Console.Write("Do you want to add emails to this project? (yes/no): ");
        string addEmailsChoice = Console.ReadLine();

        if (addEmailsChoice.ToLower() == "yes")
        {
            // Prompt the user for the path to the email file with validation
            string emailFilePath;
            do
            {
                Console.Write("Enter the path to the email file: ");
                emailFilePath = Console.ReadLine();

                if (!File.Exists(emailFilePath))
                {
                    Console.WriteLine("Invalid file path. Please enter a valid path to an existing file.");
                }

            } while (!File.Exists(emailFilePath));

            // Create a new Email object and extract metadata
            Email newEmail = new Email();
            newEmail.file_path = emailFilePath;
            newEmail.project_id = projectId;
            newEmail.ExtractMetadata();

            // Save the email to the database
            newEmail.SaveToDatabase(connectionString);

            Console.WriteLine("Email added to the project successfully.");
        }

        // Add a delay and clear the console screen
        Thread.Sleep(2000); // Sleep for 2 seconds
        Console.Clear();
    }


}
