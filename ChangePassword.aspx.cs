using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_201128S
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string finalHash;
        static string salt;
        Log log = new Log();
        Password pwdchk = new Password();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string cpwd = cPasswordTB.Text.ToString().Trim();
            string npwd = nPasswordTB.Text.ToString().Trim();
            string emailid = Session["LoggedIn"].ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = pwdchk.getDBHash(emailid);
            string dbSalt = pwdchk.getDBSalt(emailid);
            string dbHistory1 = getDBPwdHistory1(emailid);
            string dbHistory2 = getDBPwdHistory2(emailid);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = cpwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    // Checking Current Password
                    if (userHash != dbHash)
                    {
                        cPasswordError.Text = "*Current Password is incorrect";
                        cPasswordError.ForeColor = Color.Red;
                        pwdchecker.Text = "";
                        cnPasswordError.Text = "";
                    }
                    else
                    {
                        // Validate New Password
                        int scores = pwdchk.checkPassword(nPasswordTB.Text);
                        string status = "";
                        switch (scores)
                        {
                            case 1:
                                status = "Very Weak";
                                break;
                            case 2:
                                status = "Weak";
                                break;
                            case 3:
                                status = "Medium";
                                break;
                            case 4:
                                status = "Strong";
                                break;
                            case 5:
                                status = "Excellent";
                                break;
                            default:
                                break;
                        }
                        if (scores < 4)
                        {
                            pwdchecker.Text = "*Password Status : " + status;
                            pwdchecker.ForeColor = Color.Red;
                            pwdchecker.Attributes.Add("style", "visibility:visible");
                            cPasswordError.Text = "";
                            cnPasswordError.Text = "";
                        }
                        else
                        {
                            string npwdWithSalt = npwd + dbSalt;
                            byte[] nhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(npwdWithSalt));
                            string nuserHash = Convert.ToBase64String(nhashWithSalt);
                            // Validate Past 2 Password
                            if (nuserHash != dbHistory1 && nuserHash != dbHistory2)
                            {
                                // Validate New Password equal to Confirm Password
                                if (nPasswordTB.Text == cnPasswordTB.Text)
                                {
                                    updatePasswordHistory(emailid, nuserHash, dbHistory1);
                                    //Log for change password
                                    log.logged(Session["LoggedIn"].ToString(), "change password");
                                    Response.Redirect("Homepage.aspx", false);
                                }
                                else
                                {
                                    cnPasswordError.Text = "*Confirm Password does not match";
                                    cnPasswordError.ForeColor = Color.Red;
                                    cPasswordError.Text = "";
                                    pwdchecker.Text = "";
                                }
                            }
                            else
                            {
                                pwdchecker.Text = "*Do not reuse your password";
                                pwdchecker.ForeColor = Color.Red;
                                pwdchecker.Attributes.Add("style", "visibility:visible");
                                cPasswordError.Text = "";
                                cnPasswordError.Text = "";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
        }

        protected string getDBPwdHistory1(string emailid)
        {
            string h1 = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHistory1 FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHistory1"] != null)
                        {
                            if (reader["PasswordHistory1"] != DBNull.Value)
                            {
                                h1 = reader["PasswordHistory1"].ToString();
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
            return h1;
        }

        protected string getDBPwdHistory2(string emailid)
        {
            string h2 = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHistory2 FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHistory2"] != null)
                        {
                            if (reader["PasswordHistory2"] != DBNull.Value)
                            {
                                h2 = reader["PasswordHistory2"].ToString();
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
            return h2;
        }

        protected string updatePasswordHistory(string emailid, string history1, string history2)
        {
            string a = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET PasswordHash=@passwordhistory1, PasswordHistory1=@passwordhistory1, PasswordHistory2=@passwordhistory2 WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@passwordhistory1", history1);
            command.Parameters.AddWithValue("@passwordhistory2", history2);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHistory1"] != null && reader["PasswordHistory2"] != null)
                        {
                            a = reader["PasswordHistory1"].ToString();
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