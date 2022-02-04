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
        Log log = new Log();
        Password pwdchk = new Password();


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            string pwd = passwordTB.Text.ToString().Trim();
            string emailid = emailTB.Text.ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = pwdchk.getDBHash(emailid);
            string dbSalt = pwdchk.getDBSalt(emailid);
            try
            {
                string currentAttempt = Int32.Parse(countAttempt(emailid)).ToString();
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
                                updateAttempt(emailid, "3");

                                //Log for successful login
                                log.logged(emailid, "login success");

                                Response.Redirect("Homepage.aspx", false);
                            }


                            else
                            {
                                error.Text = "Email or Password is not valid. Please try again.";
                                string subtractAttempt = (Int32.Parse(countAttempt(emailid)) - 1).ToString();
                                updateAttempt(emailid, subtractAttempt);

                                //Log for failed login
                                log.logged(emailid, "fail");
                            }
                        }
                        else
                        {
                            error.Text = "Email or Password is not valid. Please try again.";

                            //Log for failed login
                            log.logged(emailid, "fail");
                        }
                    }
                }
                else
                {
                    error.Text = "Your account has been locked for 15 minutes for 3 invalid attempts.";
                    string locktimedate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    if (lockStatus(emailid) == null)
                    {
                        locked(emailid, locktimedate);

                        //Log for account lockout
                        log.logged(emailid, "locked");
                    }
                    else
                    {
                        DateTime cTimedate = Convert.ToDateTime(locktimedate);
                        DateTime lTimedate = Convert.ToDateTime(lockedTime(emailid));
                        TimeSpan ts = cTimedate.Subtract(lTimedate);
                        Int32 minLocked = Convert.ToInt32(ts.TotalMinutes);
                        Int32 pendingMin = 15 - minLocked;
                        if (pendingMin <= 0)
                        {
                            updateAttempt(emailid, "3");
                            updateLock(emailid);

                            error.Text = "Your account has been recovered. Please relogin.";

                            //Log for account recovery
                            log.logged(emailid, "recover");
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

        protected string countAttempt(string emailid)
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

        protected string updateAttempt(string emailid, string count)
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

        protected string lockedTime(string emailid)
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

        protected string updateLock(string emailid)
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

        // Google reCaptcha v3
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
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Lf291MeAAAAALk8CoUgeqa7JGNP8rpNQIUxqTyW &response=" + captchaResponse);

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
