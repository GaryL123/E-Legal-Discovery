using Npgsql;
using System;
using System.Collections.Generic;

namespace Login_WPF
{
    public class Project
    {
        // Public properties
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OwnerId { get; set; }

        // Constructor
        public Project()
        {
            // Initialize default values
            ProjectName = string.Empty;
            Description = string.Empty;
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
                    cmd.CommandText = "INSERT INTO projects (project_name, description, created_at, updated_at, owner_id) " +
                                      "VALUES (@project_name, @description, @created_at, @updated_at, @owner_id) RETURNING project_id";

                    cmd.Parameters.AddWithValue("project_name", ProjectName);
                    cmd.Parameters.AddWithValue("description", Description);
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
                            project.CreatedAt = (DateTime)reader["created_at"];
                            project.UpdatedAt = (DateTime)reader["updated_at"];
                        }
                    }
                }

                conn.Close();
            }

            return project;
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
    }
}
