using Login_WPF;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;

public class EmailDbService
{
    private NpgsqlConnection connection;

    public EmailDbService()
    {
        connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=12345678;Database=postgres");
    }

    private bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (NpgsqlException ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

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

    public List<Email> GetEmailsByProjectId(int projectId)
    {
        List<Email> emailList = new List<Email>();

        string query = "SELECT email_id, project_id, sender_address, receiver_address, subject, date_sent, email_content, file_path FROM emails WHERE project_id = @ProjectId";

        if (this.OpenConnection())
        {
            using (var cmd = new NpgsqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Email email = new Email
                        {
                            EmailId = Convert.ToInt32(dataReader["email_id"]),
                            ProjectId = Convert.ToInt32(dataReader["project_id"]),
                            SenderAddress = Convert.ToString(dataReader["sender_address"]),
                            ReceiverAddress = Convert.ToString(dataReader["receiver_address"]),
                            Subject = Convert.ToString(dataReader["subject"]),
                            DateSent = Convert.ToDateTime(dataReader["date_sent"]),
                            EmailContent = Convert.ToString(dataReader["email_content"]),
                            FilePath = Convert.ToString(dataReader["file_path"])
                        };
                        emailList.Add(email);
                    }
                }
            }
            this.CloseConnection();
        }



        return emailList;
    }

    public bool InsertEmail(Email email)
    {
        try
        {
            if (this.OpenConnection())
            {
                var cmd = new NpgsqlCommand("INSERT INTO emails (project_id, sender_address, receiver_address, subject, date_sent, email_content, file_path) VALUES (@ProjectId, @SenderAddress, @ReceiverAddress, @Subject, @DateSent, @EmailContent, @FilePath)", connection);
                cmd.Parameters.AddWithValue("@ProjectId", email.ProjectId);
                cmd.Parameters.AddWithValue("@SenderAddress", email.SenderAddress);
                cmd.Parameters.AddWithValue("@ReceiverAddress", email.ReceiverAddress);
                cmd.Parameters.AddWithValue("@Subject", email.Subject);
                cmd.Parameters.AddWithValue("@DateSent", email.DateSent);
                cmd.Parameters.AddWithValue("@EmailContent", email.EmailContent);
                cmd.Parameters.AddWithValue("@FilePath", email.FilePath);
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
            MessageBox.Show("An error occurred when inserting the email: " + ex.Message);
            return false;
        }
        finally
        {
            this.CloseConnection();
        }
    }
}
