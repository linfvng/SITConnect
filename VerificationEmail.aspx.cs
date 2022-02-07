using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_201128S
{
    public partial class VerificationEmail : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        Log log = new Log();
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

        protected void verifyBtn_Click(object sender, EventArgs e)
        {
            if (otpTB.Text == emailOTP(Session["LoggedIn"].ToString()))
            {
                //Log for correct otp
                log.logged(Session["LoggedIn"].ToString(), "otp success");

                Response.Redirect("Homepage.aspx", false);
            }
            else
            {
                error.Text = "Verification Code is invalid";
            }
        }

        protected string emailOTP(string emailid)
        {
            string otp = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Verification FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Verification"] != null)
                        {
                            if (reader["Verification"] != DBNull.Value)
                            {
                                otp = reader["Verification"].ToString();
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
            return otp;
        }
    }
}