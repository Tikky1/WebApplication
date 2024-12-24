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
    public partial class SignUp : System.Web.UI.Page
    {
        string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // Kontrollerin boş olup olmadığını kontrol et ve sıfırla
                    if (txtName != null) txtName.Text = "";
                    if (txtSurName != null) txtSurName.Text = "";
                    if (txtPassword != null) txtPassword.Text = "";
                    if (txtEmail != null) txtEmail.Text = "";
                    if (txtphoneNumber != null) txtphoneNumber.Text = "";
                    if (txtBirth != null) txtBirth.Text = "";
                    if (txtCity != null) txtCity.Text = "";
                }
            }
            catch (NullReferenceException ex)
            {
                // Hata mesajını göster
                lblMessage.Text = "Bir hata oluştu: " + ex.Message;
                // Loglama yapmak isterseniz buraya log kodu ekleyebilirsiniz.
            }
            catch (Exception ex)
            {
                // Beklenmeyen diğer hatalar için genel bir mesaj
                lblMessage.Text = "Beklenmeyen bir hata oluştu: " + ex.Message;
            }
        }
        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string surname = txtSurName.Text.Trim();
            string password = txtPassword.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtphoneNumber.Text.Trim();
            string birthDate = txtBirth.Text.Trim();
            string city = txtCity.Text.Trim();



            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(birthDate) || string.IsNullOrEmpty(city))
            {
                lblMessage.Text = "Please fill in all fields.";
                return;
            }

            var result = RegisterUser(name, surname, password, email, phone, birthDate, city);
            if (result == "success")
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Registration successful! Redirecting to login page...";
                Response.AddHeader("REFRESH", "2;URL=LoginPage.aspx");
            }
            else
            {
                lblMessage.Text = result;
            }
        }

        private string RegisterUser(string name, string surname, string password, string email, string phone, string birthDate, string city)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "INSERT INTO user (name,surname, password, email, phoneNumber, birth, city) VALUES (@name,@surname, @password, @email, @phone, @birth, @city)";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Parametreleri ekle
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@surname", surname);
                    command.Parameters.AddWithValue("@password", password); // Şifre hashlenebilir!
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@phone", phone);
                    command.Parameters.AddWithValue("@birth", birthDate);
                    command.Parameters.AddWithValue("@city", city);

                    connection.Open();
                    command.ExecuteNonQuery();
                    return "success";
                }
            }
            catch (MySqlException ex)
            {
                // Benzersiz kullanıcı kontrolü
                if (ex.Number == 1062) // Duplicate entry
                {
                    return "User Name or email already exists.";
                }

                return "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }





        //
        //
        //HASH'i hallet!!!!!!!!!!!!!
        //
        //




        //private string HashPassword(string password)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        StringBuilder builder = new StringBuilder();
        //        foreach (byte b in bytes)
        //        {
        //            builder.Append(b.ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}

    }
}