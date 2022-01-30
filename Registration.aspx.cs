using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SITConnect_201128S
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int checkPassword(string password)
        {
            int score = 0;

            // Score 1 very weak
            // if length of password is less than 8 chars
            if (password.Length < 8)
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
        protected void Chkpwd(object sender, EventArgs e)
        {
            // Extract data from textbox
            int scores = checkPassword(passwordTB.Text);
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
            pwdstatus.Text = "Status : " + status;
            if (scores < 4)
            {
                pwdstatus.ForeColor = Color.Red;
                return;
            }
            pwdstatus.ForeColor = Color.Green;
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            //string pwd = get value from your Textbox
            string pwd = passwordTB.Text.ToString().Trim(); ;
            //Generate random "salt"
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];
            //Fills array of bytes with a cryptographically strong sequence of random values.
            rng.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);
            SHA512Managed hashing = new SHA512Managed();
            string pwdWithSalt = pwd + salt;
            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            finalHash = Convert.ToBase64String(hashWithSalt);
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;
            createAccount();

            Response.Redirect("Login.aspx", false);
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Registration VALUES(@Fname, @Lname, @CreditCard, @Email, @PasswordHash, @PasswordSalt, @IV, @Key, @Count, @Lockdatetime)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Fname", fnameTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@Lname", lnameTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", Convert.ToBase64String(encryptData(ccardTB.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", emailTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@Count", 3);
                            cmd.Parameters.AddWithValue("@Lockdatetime", DBNull.Value);
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
    }
}