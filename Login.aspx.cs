using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_201128S
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string finalHash;
        static string salt;
        static string logInfo;
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
                string currentAttempt = Int32.Parse(counter(emailid)).ToString();
                if (Int32.Parse(currentAttempt) > 0)
                {
                    if (ValidateCaptcha())
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

                                //Reset the account attempt
                                updateattempt(emailid, "3");

                                //Log successful login
                                logged(emailid, "success");

                                Response.Redirect("Homepage.aspx", false);
                            }


                            else
                            {
                                error.Text = "Email or Password is not valid. Please try again.";
                                string subtractAttempt = (Int32.Parse(counter(emailid)) - 1).ToString();
                                updateattempt(emailid, subtractAttempt);

                                //Log fail login
                                logged(emailid, "fail");
                            }
                        }
                        else
                        {
                            error.Text = "Email or Password is not valid. Please try again.";

                            //Log fail login
                            logged(emailid, "fail");
                        }
                    }
                    else
                    {
                        error.Text = "Invalid CAPTCHA";
                    }
                }
                else
                {
                    error.Text = "Your account has been locked for 15 minutes for 3 invalid attempts.";
                    string locktimedate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    if (lockStatus(emailid) == null)
                    {
                        locked(emailid, locktimedate);

                        //Log lockout account
                        logged(emailid, "locked");
                    }
                    else
                    {
                        DateTime cTimedate = Convert.ToDateTime(locktimedate);
                        DateTime lTimedate = Convert.ToDateTime(locktime(emailid));
                        TimeSpan ts = cTimedate.Subtract(lTimedate);
                        Int32 minLocked = Convert.ToInt32(ts.TotalMinutes);
                        Int32 pendingMin = 15 - minLocked;
                        if (pendingMin <= 0)
                        {
                            updateattempt(emailid, "3");
                            updatelock(emailid);

                            error.Text = "Your account has been recovered. Please relogin.";

                            //Log account recovered
                            logged(emailid, "unlock");
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

        protected string getDBHash(string emailid)
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

        protected string getDBSalt(string emailid)
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

        protected string counter(string emailid)
        {
            string c = "1";
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Attempt FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Attempt"] != null)
                        {
                            c = reader["Attempt"].ToString();
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

        protected string updateattempt(string emailid, string count)
        {
            string a = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET Attempt=@counter WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@counter", count);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Attempt"] != null)
                        {
                            a = reader["Attempt"].ToString();
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

        protected string lockStatus(string emailid)
        {
            string ls = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Lockdatetime FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Lockdatetime"] != null)
                        {
                            if (reader["Lockdatetime"] != DBNull.Value)
                            {
                                ls = reader["Lockdatetime"].ToString();
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
            return ls;
        }

        protected string locked(string emailid, string locktiming)
        {
            string a = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET Lockdatetime=@locktimedate WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@locktimedate", locktiming);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Lockdatetime"] != null)
                        {
                            a = reader["Lockdatetime"].ToString();
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

        protected string locktime(string emailid)
        {
            string lt = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Lockdatetime FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Lockdatetime"] != null)
                        {
                            lt = reader["Lockdatetime"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return lt;
        }

        protected string updatelock(string emailid)
        {
            string a = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET Lockdatetime=@datetime WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@datetime", DBNull.Value);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Lockdatetime"] != null)
                        {
                            a = reader["Lockdatetime"].ToString();
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

        protected void logged(string emailid, string status)
        {
            if (status == "success")
            {
                logInfo = emailid + " has successfully login.";
            }
            else if (status == "locked")
            {
                logInfo = emailid + " account has been locked.";
            }
            else if (status == "unlock")
            {
                logInfo = emailid + " account has been recovered.";
            }
            else
            {
                logInfo = emailid + " has login failed.";
            }
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Logs VALUES(@DateTime, @Log)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Log", logInfo);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public class MyObject
        {
            public string success { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            // When user submits the recaptcha form, the user gets a response POST parameter
            // CaptchaRespponse consist of the user click pattern. Behaviour analytics!
            string captchaResponse = Request.Form["g-recaptcha-response"];

            //To send a GET request to Google along with the response and Secret Key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Lf1rEoeAAAAAEH9p8kqaEYyTxqoRFYyU6nXGCqo &response=" + captchaResponse);

            try
            {
                //Codes to recieve the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        // Create jsonObject to handle the response e.g. success or error
                        // Deserialize
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}
