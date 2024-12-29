using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
namespace WebApplication.Template
{
    public partial class LoginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
        }

        // MySQL bağlantı dizesi
        string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = HashPassword(txtPassword.Text.Trim());

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var loginResult = AuthenticateUser(email, password);
                if (loginResult == "success")
                {
                    // Kullanıcı giriş yaptıktan sonra Session oluştur
                    Session["User"] = email;
                    

                    lblMessage.Text = "Login successful! Redirecting...";
                    Response.Redirect("Main.aspx");
                }
                else if (loginResult == "adminIsHere")
                {
                    Session["User"] = email;

                    lblMessage.Text = "Login successful! Redirecting...";
                    Response.Redirect("AdminPanel.aspx");
                }
                else
                {
                    lblMessage.Text = loginResult;
                }
            }
            else
            {
                lblMessage.Text = "Please enter both User ID and Password.";
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Hash'i bir string olarak döndür
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Hexadecimal formatta döndür
                }
                return builder.ToString();
            }
        }
        protected void btnPasswordChange_Click(object sender, EventArgs e)
        {
            Response.Redirect("changePasswordPage.aspx");
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
        }

        private string AuthenticateUser(string email, string password)
        {
            try
            {
                // MySQL bağlantısı oluştur
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT email, password, isActive, isAdmin FROM user WHERE email = @email AND password = @password";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Parametreleri ekle
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);
                    
                    
                    // Bağlantıyı aç ve sorguyu çalıştır
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // isActive kontrolü (isteğe bağlı)
                            bool isActive = reader.GetBoolean("isActive");
                            if (!isActive)
                            {
                                return "Your account is inactive. Please contact admin.";
                            }
                            bool isAdmin = reader.GetBoolean("isAdmin");
                            if (!isAdmin)
                            {
                                return "success";
                            }
                            else
                            {
                                return "adminIsHere";
                            }
                            
                        }
                        else
                        {
                            return "Invalid User ID or Password.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
