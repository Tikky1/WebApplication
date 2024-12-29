using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data;

namespace WebApplication.Template
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {

            // Kullanıcı login olmadıysa Login sayfasına yönlendirme
            if (Session["User"] == null)
            {
                Response.Redirect("LoginPage.aspx", false);
                return;
            }

            if (!IsPostBack)
            {

                LoadComments();
                // Session'dan email bilgisini al
                string userEmail = Session["User"] as string;


                // Kullanıcının şehir bilgisini al
                string city = GetCityFromDatabase(userEmail);
                if (string.IsNullOrEmpty(city))
                {
                    city = "Istanbul"; // Varsayılan şehir
                }

                string apiKey = "451ea1379d2c469747b294bf43a5462c"; // OpenWeather API anahtarınızı buraya ekleyin
                string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=tr";
                if (!string.IsNullOrEmpty(city))
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync(apiUrl);
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();

                            JObject weatherData = JObject.Parse(responseBody);
                            string cityName = weatherData["name"].ToString();
                            string temperature = weatherData["main"]["temp"].ToString();
                            string weatherDescription = weatherData["weather"][0]["description"].ToString();

                            lblWeatherInfo.Text = $"Şehir: {cityName}<br />" +
                                                  $"Sıcaklık: {temperature}°C<br />" +
                                                  $"Durum: {weatherDescription}";
                        }
                    }
                    catch (Exception ex)
                    {
                        lblWeatherInfo.Text = $"Hata: {ex.Message}";
                    }
                }
                else
                {
                    lblWeatherInfo.Text = "Kullanıcının şehir bilgisi bulunamadı.";
                }
            }

        }




        private string GetCityFromDatabase(string userEmail)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            string city = null;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT city FROM user WHERE email = @Email";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Doğru parametreyi ekliyoruz
                    command.Parameters.AddWithValue("@Email", userEmail);

                    connection.Open();
                    object result = command.ExecuteScalar(); // Sorgunun sonucunu alıyoruz

                    if (result != null)
                    {
                        city = result.ToString(); // Şehir bilgisini döndürüyoruz
                    }
                }
            }
            catch (Exception ex)
            {
                lblWeatherInfo.Text = $"Hata: {ex.Message}";
            }

            return city; // Şehir bilgisi veya null döndürülür

        }
        protected async void btnSearch_Click(object sender, EventArgs e)
        {
            string city = txtCity.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                lblWeatherInfo.Text = "Lütfen bir şehir adı girin.";
                return;
            }

            string apiKey = "451ea1379d2c469747b294bf43a5462c"; // OpenWeather API anahtarınızı buraya ekleyin
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=tr";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject weatherData = JObject.Parse(responseBody);
                    string cityName = weatherData["name"].ToString();
                    string temperature = weatherData["main"]["temp"].ToString();
                    string weatherDescription = weatherData["weather"][0]["description"].ToString();

                    lblWeatherInfo.Text = $"Şehir: {cityName}<br />" +
                                          $"Sıcaklık: {temperature}°C<br />" +
                                          $"Durum: {weatherDescription}";
                }
            }
            catch (Exception ex)
            {
                lblWeatherInfo.Text = $"Hata: {ex.Message}";
            }
        }
        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            // Session'dan email bilgisini al
            string userEmail = Session["User"] as string;
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            string city = null;
            string commentText = txtComment.Text.Trim();

            if (string.IsNullOrEmpty(commentText))
            {
                lblMessage.Text = "Lütfen tüm alanları doldurun.";
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    // Kullanıcının şehir bilgisi için SELECT sorgusu
                    string selectQuery = "SELECT City FROM user WHERE email = @Email";

                    // Yorum ekleme için INSERT sorgusu
                    string insertQuery = "INSERT INTO Comments (email, CommentText, City) " +
                                         "VALUES (@Email, @CommentText, @City)";

                    // Şehir bilgisini almak için MySqlCommand
                    MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@Email", userEmail);

                    // Bağlantıyı aç
                    connection.Open();

                    // SELECT sorgusundan şehir bilgisini al
                    object result = selectCommand.ExecuteScalar();
                    if (result != null)
                    {
                        city = result.ToString();
                    }
                    else
                    {
                        lblMessage.Text = "Kullanıcının şehir bilgisi bulunamadı.";
                        return;
                    }

                    // INSERT sorgusu için MySqlCommand
                    MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Email", userEmail);
                    insertCommand.Parameters.AddWithValue("@CommentText", commentText);
                    insertCommand.Parameters.AddWithValue("@City", city);

                    // Yorum ekleme işlemini gerçekleştir
                    insertCommand.ExecuteNonQuery();

                    lblMessage.Text = "Yorum başarıyla kaydedildi.";
                    txtComment.Text = ""; // TextBox'ı temizle

                    // Yorumları yeniden yükle
                    LoadComments();
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = $"Hata: {ex.Message}";
            }

        }
        private void LoadComments()
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT id,email, City, CommentText, CreatedAt " +
                                   "FROM Comments " +
                                   "WHERE isActive = 1 " +
                                   "ORDER BY CreatedAt DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        rptComments.DataSource = dt;
                        rptComments.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = $"Yorumlar yüklenirken hata oluştu: {ex.Message}";
            }
        }

        protected void rptComments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditComment")
            {
                // Düzenlenecek yorumun ID'sini al
                int commentId = Convert.ToInt32(e.CommandArgument);
                lblMessage.Text = commentId.ToString();
                // Yorum bilgilerini getir
                LoadCommentForEditing(commentId);
            }
            if (e.CommandName == "DeActiveComment")
            {
                // Düzenlenecek yorumun ID'sini al
                int commentId = Convert.ToInt32(e.CommandArgument);

                // Yorum bilgilerini getir
                DeactivateComment(commentId);

            }

        }
        private void LoadCommentForEditing(int commentId)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            string currentUserEmail = Session["User"] as string;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT CommentText FROM Comments WHERE id = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", commentId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            string commentText = reader["CommentText"].ToString();

                            // Eğer yorum sahibi değilse ve admin değilse işlem yapılmaz
                            if (!IsAdmin(currentUserEmail))
                            {
                                lblMessage.Text = "Bu yorumu düzenleme yetkiniz yok.";
                                return;
                            }

                            // Düzenleme alanını doldur
                            txtEditComment.Text = commentText;
                            hfCommentId.Value = commentId.ToString();
                            pnlEditComment.Visible = true;
                        }
                        else
                        {
                            lblMessage.Text = "Yorum bulunamadı.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = $"Yorum düzenlenirken hata oluştu: {ex.Message}";
            }
        }

        protected void btnSaveComment_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            int commentId = Convert.ToInt32(hfCommentId.Value);
            string updatedComment = txtEditComment.Text.Trim();

            if (string.IsNullOrEmpty(updatedComment))
            {
                lblMessage.Text = "Yorum boş bırakılamaz.";
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    string query = "UPDATE Comments SET CommentText = @CommentText WHERE id = @id ";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CommentText", updatedComment);
                    command.Parameters.AddWithValue("@id", commentId);


                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();


                    lblMessage.Text = "Yorum başarıyla güncellendi.";
                    pnlEditComment.Visible = false;
                    LoadComments(); // Güncel yorumları yeniden yükle

                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = $"Yorum güncellenirken hata oluştu: {ex.Message}";
            }
        }
        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEditComment.Visible = false; // Düzenleme panelini gizle
            hfCommentId.Value = string.Empty; // HiddenField'i temizle
            txtEditComment.Text = string.Empty; // TextBox'u temizle
        }







        private void DeactivateComment(int commentId)
        {
            string connectionString = "Server=localhost;Port=3306;Database=proje;User=root;Password=12345;";
            string currentUserEmail = Session["User"] as string;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    // Silme işlemi için önce yorumu kontrol et
                    string checkQuery = "SELECT email FROM Comments WHERE id = @ID AND isActive = TRUE";
                    MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@ID", commentId);

                    connection.Open();
                    object result = checkCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string commentOwnerEmail = result.ToString();

                        // Eğer yorum sahibi değilse ve admin değilse işlem yapılmaz
                        if (commentOwnerEmail != currentUserEmail && !IsAdmin(currentUserEmail))
                        {
                            lblMessage.Text = "Bu yorumu silme yetkiniz yok.";
                            return;
                        }

                        // Yorumu pasif hale getir
                        string updateQuery = "UPDATE Comments SET isActive = FALSE WHERE id = @ID";
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@ID", commentId);
                        updateCommand.ExecuteNonQuery();

                        lblMessage.Text = "Yorum başarıyla silindi.";
                        LoadComments();
                    }
                    else
                    {
                        lblMessage.Text = "Yorum bulunamadı veya zaten silinmiş.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = $"Yorum silinirken hata oluştu: {ex.Message}";
            }
        }
        public bool IsAdmin(string email)
        {
            return true;
        }








    }
}