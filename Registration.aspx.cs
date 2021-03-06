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
using System.IO;

namespace SITConnect_201128S
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SITConnect"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Photo;
        byte[] Key;
        byte[] IV;
        Log log = new Log();
        Password pwdchk = new Password();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            HttpPostedFile postedFile = photoTB.PostedFile;
            string fileName = Path.GetFileName(postedFile.FileName);
            string fileExtension = Path.GetExtension(fileName);

            bool validEmail;
            bool validFile;
            bool validPwd;

            if(checkEmail(emailTB.Text.Trim()) != null)
            {
                emailError.Text = "Email have been used";
                emailError.ForeColor = Color.Red;
                validEmail = false;
            }
            else
            {
                validEmail = true;
            }

            // File Upload Validation
            if(fileExtension.ToLower() == ".jpg" || fileExtension.ToLower() == ".bmp" || fileExtension.ToLower() == ".gif" || fileExtension.ToLower() == ".png")
            {
                Stream stream = postedFile.InputStream;
                BinaryReader binaryReader = new BinaryReader(stream);
                Photo = binaryReader.ReadBytes((int)stream.Length);

                validFile = true;
            }
            else
            {
                photoError.Text = "Only images (.jpg, .png, .gif, .bmp) can be upload";
                photoError.ForeColor = System.Drawing.Color.Red;

                validFile = false;
            }

            // Password Validation
            int scores = pwdchk.checkPassword(passwordTB.Text);
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
                validPwd = false;
            }
            else
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
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                validPwd = true;
            }

            if (validPwd && validFile && validEmail)
            {
                createAccount();

                Response.Redirect("Login.aspx", false);
            }
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Fname, @Lname, @CreditCard, @Email, @Verification, @PasswordHash, @PasswordSalt, @DoB, @Photo, @IV, @Key, @Attempt, @Lockdatetime, @PasswordHistory1, @PasswordHistory2, @MinPasswordAge,  @MaxPasswordAge, @MailEmail, @MailPwd)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Fname", fnameTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@Lname", lnameTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", Convert.ToBase64String(encryptData(ccardTB.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", emailTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@Verification", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DoB", dobTB.Text.Trim());
                            cmd.Parameters.AddWithValue("@Photo", Photo);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@Attempt", 3);
                            cmd.Parameters.AddWithValue("@Lockdatetime", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordHistory1", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordHistory2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MinPasswordAge", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MaxPasswordAge", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@MailEmail", "emailgenerator821102@gmail.com");
                            cmd.Parameters.AddWithValue("@MailPwd", "Nyp@821102");
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                            //Log for successful registration
                            log.logged(emailTB.Text, " has register successfully.");
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

        protected string checkEmail(string emailid)
        {
            string ce = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select CreditCard FROM Account WHERE Email=@EMAILID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAILID", emailid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["CreditCard"] != null)
                        {
                            ce = reader["CreditCard"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return ce;
        }
    }
}