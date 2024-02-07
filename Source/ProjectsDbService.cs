using Login_WPF;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;

namespace Login_WPF 
{ 
    public class ProjectsDbService
    {
        private NpgsqlConnection connection;

        // Constructor
        public ProjectsDbService()
        {
            connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=12345678;Database=postgres");
        }

        // Open connection to database
        private bool OpenConnection()
        {
            try
            {
                CloseConnection();
                connection.Open();
                return true;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}");
                return false;
            }
        }

        // Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        // Insert statement
        public bool Insert(Project project)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var cmd = new NpgsqlCommand(@"INSERT INTO projects 
                (project_name, description, created_at, updated_at, owner_id) 
                VALUES (@project_name, @description, @created_at, @updated_at, @owner_id)", connection);

                    // Don't include project_id in the parameters or values

                    cmd.Parameters.AddWithValue("@project_name", project.ProjectName);
                    cmd.Parameters.AddWithValue("@description", project.Description);
                    cmd.Parameters.AddWithValue("@created_at", project.CreatedAt);
                    cmd.Parameters.AddWithValue("@updated_at", project.UpdatedAt);
                    cmd.Parameters.AddWithValue("@owner_id", project.OwnerId);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        // Update statement
        public bool Update(Project project)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var cmd = new NpgsqlCommand("UPDATE projects SET project_name = @project_name, description = @description, created_at = @created_at, updated_at = @updated_at, owner_id = @owner_id WHERE project_id = @project_id", connection);
                    cmd.Parameters.AddWithValue("@project_id", project.ProjectId);
                    cmd.Parameters.AddWithValue("@project_name", project.ProjectName);
                    cmd.Parameters.AddWithValue("@description", project.Description);
                    cmd.Parameters.AddWithValue("@created_at", project.CreatedAt);
                    cmd.Parameters.AddWithValue("@updated_at", project.UpdatedAt);
                    cmd.Parameters.AddWithValue("@owner_id", project.OwnerId);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        // Delete statement
        public bool Delete(int projectId)
        {
            try
            {
                if (this.OpenConnection())
                {
                    var cmd = new NpgsqlCommand("DELETE FROM projects WHERE project_id = @project_id", connection);
                    cmd.Parameters.AddWithValue("@project_id", projectId);
                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        // Select statement
        public ObservableCollection<Project> Select(int loggedInUserId)
        {
            string query = "SELECT project_id, project_name, description, created_at, updated_at, owner_id FROM projects WHERE owner_id = @OwnerId";
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            if (this.OpenConnection())
            {
                var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@OwnerId", loggedInUserId);
                var dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    var project = new Project();
                    project.ProjectId = Convert.ToInt32(dataReader["project_id"]);
                    project.ProjectName = Convert.ToString(dataReader["project_name"]);
                    project.Description = Convert.ToString(dataReader["description"]);
                    project.CreatedAt = (DateTime)dataReader["created_at"];
                    project.UpdatedAt = (DateTime)dataReader["updated_at"];
                    project.OwnerId = Convert.ToInt32(dataReader["owner_id"]);
                    projects.Add(project);
                }
                dataReader.Close();
                this.CloseConnection();
                return projects;
            }
            else
            {
                return null;
            }
        }
    }
}

