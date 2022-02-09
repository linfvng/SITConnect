using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SITConnect_201128S
{
    public class Password
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        public int checkPassword(string password)
        {
            int score = 0;

            // Score 1 very weak
            // if length of password is less than 8 chars
            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            // Score 2 weak
            // if password contains only lowercase letter(s)
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            // Score 3 medium
            // if password contains only lowercase and uppercase letter(s)
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            // Score 4 strong
            // if password contains numeral(s)
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            // Score 5 excellent
            // if password contains special character(s)
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }

            return score;
        }

        public string getDBHash(string emailid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        public string getDBSalt(string emailid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }


    }
}