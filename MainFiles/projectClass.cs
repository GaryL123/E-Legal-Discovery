using Npgsql;
using System;
using System.Collections.Generic;

public class Project
{
    // Public properties
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int OwnerId { get; set; }

    // Constructor
    public Project()
    {
        // Initialize default values
        ProjectName = string.Empty;
        Description = string.Empty;
        StartDate = DateOnly.MinValue;
        EndDate = DateOnly.MinValue;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
        OwnerId = 0;
    }

    // Method to save the project to the database
    public void SaveToDatabase(string connectionString)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO projects (project_name, description, start_date, end_date, created_at, updated_at, owner_id) " +
                                  "VALUES (@project_name, @description, @start_date, @end_date, @created_at, @updated_at, @owner_id) RETURNING project_id";

                cmd.Parameters.AddWithValue("project_name", ProjectName);
                cmd.Parameters.AddWithValue("description", Description);
                cmd.Parameters.AddWithValue("start_date", StartDate);
                cmd.Parameters.AddWithValue("end_date", EndDate);
                cmd.Parameters.AddWithValue("created_at", CreatedAt);
                cmd.Parameters.AddWithValue("updated_at", UpdatedAt);
                cmd.Parameters.AddWithValue("owner_id", OwnerId);

                // Retrieve the generated project_id after insertion
                ProjectId = (int)cmd.ExecuteScalar();
            }

            conn.Close();
        }
    }

    // Static method to get project_id by project name
    public static int GetProjectIdByName(string projectName, string connectionString)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT project_id FROM projects WHERE project_name = @project_name";
                cmd.Parameters.AddWithValue("project_name", projectName);

                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return (int)result;
                }
            }

            conn.Close();
        }
        return -1;
    }

    //method to get a project by their id to be opened
    public static Project GetProjectById(int id, string connectionString)
    {
        Project project = new Project();
        project.ProjectId = id;

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM projects WHERE project_id = @project_id";
                cmd.Parameters.AddWithValue("project_id", project.ProjectId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        project.ProjectId = (int)reader["project_id"];
                        project.ProjectName = reader["project_name"].ToString();
                        project.Description = reader["description"].ToString();
                        project.StartDate = DateOnly.FromDateTime((DateTime)reader["start_date"]);
                        project.EndDate = DateOnly.FromDateTime((DateTime)reader["end_date"]);
                        project.CreatedAt = (DateTime)reader["created_at"];
                        project.UpdatedAt = (DateTime)reader["updated_at"];
                    }
                }
            }

            conn.Close();
        }

        return project;
    }


    // Method to open a project
    public static Project OpenProject(string connectionString, User currentUser)
    {
        Console.WriteLine("List of Projects:");
        ListProjects(connectionString, currentUser);

        // Get the project_id from the user
        Console.Write("Enter the project_id to open: ");
        if (int.TryParse(Console.ReadLine(), out int project_id))
        {
            // Retrieve the project from the database
            Project project = GetProjectById(project_id, connectionString);

            if (project != null)
            {
                Console.WriteLine($"Opened Project: {project.ProjectName} (ID: {project.ProjectId})");
                return project;
            }
            else
            {
                Console.WriteLine("Project not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid project_id. Please enter a valid number.");
        }

        return null;
    }


    // Method to list projects for a user
    private static void ListProjects(string connectionString, User currentUser)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT project_id, project_name FROM projects WHERE owner_id = @owner_id";
                cmd.Parameters.AddWithValue("owner_id", currentUser.user_id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Project ID: {reader["project_id"]}, Project Name: {reader["project_name"]}");
                    }
                }
            }

            conn.Close();
        }
    }

    // Method to list emails for the current project by ID
    public void ListEmails(string connectionString)
    {
        List<Email> emails = GetEmailsForProject(connectionString);

        if (emails.Count > 0)
        {
            Console.WriteLine("List of Emails for the Project:");

            foreach (var email in emails)
            {
                Console.WriteLine($"Email ID: {email.email_id}, Subject: {email.subject}");
            }

            // You can now implement the logic to select and edit an email
            // based on the user's input
        }
        else
        {
            Console.WriteLine("No emails found for the current project.");
        }
    }

    // List of email Email objects for the current project by ID
    public List<Email> GetEmailsForProject(string connectionString)
    {
        List<Email> emails = new List<Email>();

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM emails WHERE project_id = @project_id";
                cmd.Parameters.AddWithValue("project_id", ProjectId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Email email = new Email
                        {
                            email_id = (int)reader["email_id"],
                            project_id = (int)reader["project_id"],
                            sender_address = reader["sender_address"].ToString(),
                            receiver_address = reader["receiver_address"].ToString(),
                            subject = reader["subject"].ToString(),
                            date_sent = (DateTime)reader["date_sent"],
                            file_path = reader["file_path"].ToString()
                        };

                        emails.Add(email);
                    }
                }
            }

            conn.Close();
        }

        return emails;
    }
}


