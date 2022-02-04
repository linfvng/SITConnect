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
        byte[] Key;
        byte[] IV;
        Log log = new Log();
        Password pwdchk = new Password();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string cpwd = cPasswordTB.Text.ToString().Trim();
            string emailid = Session["LoggedIn"].ToString().Trim();
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = pwdchk.getDBHash(emailid);
            string dbSalt = pwdchk.getDBSalt(emailid);
            try
            {
                // Checking Current Password
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwdWithSalt = cpwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);

                    if (userHash != dbHash)
                    {
                        cPasswordError.Text = "*Current Password is incorrect";
                    }
                }

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
                    pwdchecker.Text = "Password Status : " + status;
                    pwdchecker.ForeColor = Color.Red;
                    pwdchecker.Attributes.Add("style", "visibility:visible");
                }
                else
                {
                    //string pwd = get value from your Textbox
                    string npwd = nPasswordTB.Text.ToString().Trim(); ;
                    //Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];
                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);
                    string pwdWithSalt = npwd + salt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    finalHash = Convert.ToBase64String(hashWithSalt);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
        }

        protected void Logout(object sender, EventArgs e)
        {
            //Log for account logout
            log.logged(Session["LoggedIn"].ToString(), "logout");

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Index.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Request.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Request.Cookies["AuthToken"].Value = string.Empty;
                Request.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }



    }
}