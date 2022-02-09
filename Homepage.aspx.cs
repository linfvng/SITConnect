using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_201128S
{
    public partial class Homepage : System.Web.UI.Page
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
                else
                {
                    fullname.Text = name(Session["LoggedIn"].ToString());
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void Logout(object sender, EventArgs e)
        {
            //Log for account logout
            log.logged(Session["LoggedIn"].ToString(), " has logout successfully.");

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

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

        protected string name(string emailid)
        {
            string n = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Fname, Lname, CONCAT(Fname, ' ', Lname) AS Fullname FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Fullname"] != null)
                        {
                            if (reader["Fullname"] != DBNull.Value)
                            {
                                n = reader["Fullname"].ToString();
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
            return n;
        }
    }
}