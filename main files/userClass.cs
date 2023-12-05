using System;
using Npgsql;
using System.Text;

class userClass
{
    // Helper method to retrieve a user by username from the database
    private static User GetUserByUsername(string username, string connectionString)
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
}

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
        user_id = 0;
        Password = string.Empty;
        Email = string.Empty;
        role = string.Empty;
    }

    // Constructor with parameters
    public User(string userName, int user_id, string password, string email, string role)
    {
        this.UserName = userName;
        this.user_id = user_id;
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
                cmd.CommandText = "INSERT INTO users (username, user_id, password, email, role) VALUES (@username, @user_id, @password, @email, @role)";

                cmd.Parameters.AddWithValue("username", UserName);
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("password", hashedPassword); // Store the hashed password
                cmd.Parameters.AddWithValue("email", Email);
                cmd.Parameters.AddWithValue("role", role);

                cmd.ExecuteNonQuery();
            }

            conn.Close();
        }
    }

    // Method to authenticate a user
    public bool AuthenticateUser(string enteredPassword, string connectionString)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT password FROM users WHERE lower(username) = lower(@username)";
                cmd.Parameters.AddWithValue("username", UserName);

                // Retrieve hashed password from the database
                string storedHashedPassword = cmd.ExecuteScalar() as string;

                if (storedHashedPassword != null)
                {
                    // Hash the entered password for comparison
                    string enteredHashedPassword = HashPassword(enteredPassword);

                    // Compare hashed passwords
                    return string.Equals(storedHashedPassword, enteredHashedPassword, StringComparison.OrdinalIgnoreCase);
                }
            }

            conn.Close();
        }

        return false; // Authentication failed
    }

    // Helper method to hash a password
    private string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            // Compute hash from password bytes
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert hashed bytes to string (hex format)
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
