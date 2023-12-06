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
}
