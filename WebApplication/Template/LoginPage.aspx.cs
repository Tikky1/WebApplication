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
            // Eğer kullanıcı zaten giriş yaptıysa, doğrudan yönlendir
            if (Session["User"] != null)
            {
                Response.Redirect("Main.aspx");
            }
        }

        // MySQL bağlantı dizesi
        string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

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
                    string query = "SELECT email, password, isActive FROM user WHERE email = @email AND password = @password";
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

                            return "success";
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
