using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SITConnect_201128S
{
    public class Log
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string logInfo;
        public void logged(string emailid, string status)
        {
            if (status == "login success")
            {
                logInfo = emailid + " has successfully login.";
            }
            else if (status == "locked")
            {
                logInfo = emailid + " account has been locked.";
            }
            else if (status == "recover")
            {
                logInfo = emailid + " account has been recovered.";
            }
            else if (status == "fail")
            {
                logInfo = emailid + " has login failed.";
            }
            else if (status == "logout")
            {
                logInfo = emailid + " has logout successfully.";
            }
            else if (status == "register success")
            {
                logInfo = emailid + " has register successfully.";
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
    }
}