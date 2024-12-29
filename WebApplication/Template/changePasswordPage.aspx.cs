using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.Template
{
    public partial class changePasswordPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                lblMessage.Text = "Lütfen tüm alanları doldurun.";
                return;
            }

            try
            {
                if (VerifyCurrentPassword(email, currentPassword))
                {
                    if (UpdatePassword(email, newPassword))
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Password change successful! Redirecting to login page...";
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        Response.AddHeader("REFRESH", "2;URL=LoginPage.aspx");
                    }
                    else
                    {
                        lblMessage.Text = "Şifre değiştirilemedi. Lütfen tekrar deneyin.";
                    }
                }
                else
                {
                    lblMessage.Text = "Mevcut şifre yanlış.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Hata: {ex.Message}";
            }
        }

        private bool VerifyCurrentPassword(string email, string currentPassword)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            string hashedPassword = HashPassword(currentPassword);

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM User WHERE email = @Email AND password = @Password";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0; // Şifre doğruysa true döner
                }
            }
            catch
            {
                throw;
            }
        }

        private bool UpdatePassword(string email, string newPassword)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            string hashedPassword = HashPassword(newPassword);

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "UPDATE User SET password = @NewPassword WHERE email = @Email";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NewPassword", hashedPassword);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0; // Şifre değiştirildiyse true döner
                }
            }
            catch
            {
                throw;
            }
        }
        protected void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}