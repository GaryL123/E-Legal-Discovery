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
    public Project CreateProject(string connectionString)
    {
        Project newProject = new Project();

        Console.Write("Enter project name: ");
        newProject.ProjectName = Console.ReadLine();

        Console.Write("Enter project description: ");
        newProject.Description = Console.ReadLine();

        Console.Write("Enter project start date (yyyy-MM-dd): ");
        if (DateOnly.TryParse(Console.ReadLine(), out DateOnly startDate))
        {
            newProject.StartDate = startDate;
        }
        else
        {
            // Handle parsing error or set a default value
            Console.WriteLine("Invalid date format. Using default value.");
            newProject.StartDate = DateOnly.MinValue;
        }

        Console.Write("Enter project end date (yyyy-MM-dd): ");
        if (DateOnly.TryParse(Console.ReadLine(), out DateOnly endDate))
        {
            newProject.EndDate = endDate;
        }
        else
        {
            // Handle parsing error or set a default value
            Console.WriteLine("Invalid date format. Using default value.");
            newProject.EndDate = DateOnly.MinValue;
        }

        newProject.OwnerId = user_id; // Assign the owner ID

        // Save the project to the database
        newProject.SaveToDatabase(connectionString);

        Console.WriteLine($"Project '{newProject.ProjectName}' created successfully!");

        return newProject;
    }

}
