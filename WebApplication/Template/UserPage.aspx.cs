using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication.Template
{
    public partial class UserPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kullanıcı login olmadıysa Login sayfasına yönlendirme
            if (Session["User"] == null)
            {
                Response.Redirect("LoginPage.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                LoadUsers();
                // Session'dan email bilgisini al
                string userEmail = Session["User"] as string;
            }
        }
        public bool IsAdmin(string email)
        {
            return true;
        }
        
        
        private void LoadUsers()
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM User ORDER BY Id DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        gvUsers.DataSource = dt;
                        gvUsers.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Veriler yüklenirken hata oluştu: {ex.Message}";
            }
        }
        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            LoadUsers();
        }
        protected void gvUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            LoadUsers();
        }
        protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);

            GridViewRow row = gvUsers.Rows[e.RowIndex];
            string name = (row.Cells[1].Controls[0] as TextBox).Text;
            string surname = (row.Cells[2].Controls[0] as TextBox).Text;
            string email = (row.Cells[3].Controls[0] as TextBox).Text;
            bool isActive = (row.Cells[4].Controls[0] as CheckBox).Checked;
            bool isAdmin = (row.Cells[5].Controls[0] as CheckBox).Checked;
            string phoneNumber = (row.Cells[6].Controls[0] as TextBox).Text;
            string city = (row.Cells[7].Controls[0] as TextBox).Text;
            

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "UPDATE User SET name = @name, surname = @surname, email = @Email," +
                        " isActive = @isActive, isAdmin = @isAdmin, phoneNumber = @phoneNumber, city = @city" +
                        " WHERE Id = @Id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@surname", surname);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@isActive", isActive);                   
                    command.Parameters.AddWithValue("@isAdmin", isAdmin);
                    command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@city", city);
                    command.Parameters.AddWithValue("@Id", userId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                lblMessage.Text = "Kayıt başarıyla güncellendi.";
                gvUsers.EditIndex = -1;
                LoadUsers();
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Güncelleme sırasında hata oluştu: {ex.Message}";
            }
        }
        








    }
}