using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_201128S
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            string pwd = passwordTB.Text.ToString().Trim();
            string emailid = emailTB.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(emailid);
            string dbSalt = getDBSalt(emailid);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);

                        if (userHash.Equals(dbHash))
                        {
                            Session["LoggedIn"] = emailTB.Text.Trim();

                            // creates  a new GUID and save into the session
                            string guid = Guid.NewGuid().ToString();
                            Session["AuthToken"] = guid;

                            //now create a new cookie with this guid value
                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                            string c = counter(emailid).ToString();
                            if (Int32.Parse(c) > 0)
                            {
                                updateattempt(emailid, "3");
                                Response.Redirect("Homepage.aspx", false);
                            }
                            else
                            {
                                attempt.Text = "Your account have been locked. Please contact the administrator.";
                            }
                        }


                        else
                        {
                            error.Text = "Email or Password is not valid. Please try again.";
                            string c = (Int32.Parse(counter(emailid)) - 1).ToString();
                            if (Int32.Parse(c) > 0)
                            {
                                attempt.Text = "You have " + c + " attempt left.";
                                updateattempt(emailid, c);
                            }
                            else
                            {
                                attempt.Text = "Your account have been locked. Please contact the administrator.";
                            }
                        }

                }
                else
                {
                    error.Text = "Email or Password is not valid. Please try again.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }

        }

        protected string getDBHash(string emailid)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Registration WHERE Email=@EMAILID";
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

        protected string getDBSalt(string emailid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM Registration WHERE Email=@EMAILID";
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

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected string counter(string emailid)
        {
            string c = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Count FROM Registration WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Count"] != null)
                        {
                            c = reader["Count"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return c;
        }

        protected string updateattempt(string emailid, string counter)
        {
            string a = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Registration SET Count=@count WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@count", counter);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Count"] != null)
                        {
                            if (reader["Count"] != DBNull.Value)
                            {
                                a = reader["Count"].ToString();
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
            return a;
        }
    }
}
