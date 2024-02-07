using System;
using Npgsql;
using System.Text;
using System.Runtime.CompilerServices;
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
    public static int InsertUser(string connectionString, string username, string password, string email)
    {
        int userId = 0;

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            string hashedPassword = HashPassword(password);

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                // Return the id of the inserted user
                cmd.CommandText = (@"INSERT INTO users (username, password, email, role) VALUES (@username, @password, @email, 'User')
                                RETURNING user_id;");

                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", hashedPassword);
                cmd.Parameters.AddWithValue("email", email);

                // Execute the command and get the user ID
                userId = (int)cmd.ExecuteScalar();
            }

            conn.Close();
        }

        return userId;
    }



    // Method to authenticate a user and return a boolean indicating success or failure
    public static User AuthenticateUser(string connectionString, string enteredUsername, string enteredPassword)
    {
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
                        User authenticatedUser = new User
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
                        if (string.Equals(authenticatedUser.Password, enteredHashedPassword, StringComparison.OrdinalIgnoreCase))
                        {
                            return authenticatedUser; // Return the authenticated User object
                        }
                        else
                        {
                            return null; // Return null if authentication fails
                        }
                    }
                    else
                    {
                        // User not found
                        return null;
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

    public static string GetUsernameById(int userId)
    {
        string username = null;

        using (var conn = new NpgsqlConnection("Host=localhost;Username=postgres;Password=12345678;Database=postgres"))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT username FROM users WHERE user_id = @UserId";
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        username = reader["username"].ToString();
                    }
                }
            }

            conn.Close();
        }

        return username;
    }
}