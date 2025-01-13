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
using System.Collections;

namespace WebApplication.Template
{
    public partial class Main : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {

            // Kullanıcı login olmadıysa Login sayfasına yönlendirme
            //if (Session["User"] == null)
            //{
            //    Response.Redirect("LoginPage.aspx", false);
            //    return;
            //}
            if (Session["User"] == null)
            {
                Response.Redirect("LoginPage.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                // Giriş yapan kullanıcının e-postasını al
                string userEmail = Session["User"] as string;

                // Kullanıcının şehir bilgisi veritabanından alınır
                string userCity = GetCityFromDatabase(userEmail);
                if (string.IsNullOrEmpty(userCity))
                {
                    userCity = "Istanbul"; // Varsayılan şehir
                }

                // Şehir ID'si alınıyor
                string cityId = await GetCityIdFromAPI(userCity);
                if (!string.IsNullOrEmpty(cityId))
                {
                    // Widget için JavaScript'i çalıştır
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "UpdateWidget", $"updateWeatherWidget('{cityId}');", true);
                }
            }



            if (!IsPostBack)
            {
                // Giriş yapan kullanıcının e-posta adresini al
                string userEmail = Session["User"] as string;

                // Kullanıcının şehir bilgisi
                string userCity = GetCityFromDatabase(userEmail);
                if (string.IsNullOrEmpty(userCity))
                {
                    userCity = "Istanbul"; // Varsayılan şehir
                }



                // Şehre göre yorumları yükle
                txtCity.Text = userCity; // Varsayılan şehri TextBox'a yaz
                LoadComments();
            }


            if (!IsPostBack)
            {
                // Öğe sayısını kontrol edin
                if (ddlCities.Items.Count == 1)
                {

                    txtCity.Text = ddlCities.Items[0].ToString();
                }


                LoadComments();
            }


            if (!IsPostBack)
            {


                // Session'dan email bilgisini al




                string userEmail = Session["user"] as string;

                // Kullanıcının şehir bilgisini al
                string city = GetCityFromDatabase(userEmail);
                if (string.IsNullOrEmpty(city))
                {
                    city = "Istanbul"; // Varsayılan şehir
                }


                if (!string.IsNullOrEmpty(city))
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync(Connection.ApiConnection(city));
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

        private async Task<string> GetCityIdFromAPI(string cityName)
        {
            string cityId = null;
            string apiKey = "451ea1379d2c469747b294bf43a5462c";
            string apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject weatherData = JObject.Parse(responseBody);
                        cityId = weatherData["id"]?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Şehir ID'si alınırken hata oluştu: {ex.Message}";
            }

            return cityId;
        }


        protected void txtInput_TextChanged(object sender, EventArgs e)
        {
            string cityName = txtCity.Text.Trim();





            ddlCities.Items.Clear();
            ddlCities.SelectedIndex = -1;
            if (txtCity.Text != null)
            {
                foreach (string city in Connection.GetCity(txtCity.Text))
                {
                    ddlCities.Items.Add(new ListItem(city));
                }
            }



        }
        protected void ddlCities_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCity.Text = ddlCities.SelectedItem.ToString();
        }


        private string GetCityFromDatabase(string userEmail)
        {

            string city = null;

            try
            {
                using (var connection = Connection.GetConnection())
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

            ddlCities.Items.Clear();


            if (string.IsNullOrEmpty(city))
            {
                lblWeatherInfo.Text = "Lütfen bir şehir adı girin.";
                return;
            }




            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(Connection.ApiConnection(city));
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


            if (!string.IsNullOrEmpty(city))
            {
                // Şehir ID'si alınıyor
                string cityId = await GetCityIdFromAPI(city);
                if (!string.IsNullOrEmpty(cityId))
                {
                    // Widget için JavaScript'i çalıştır
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "UpdateWidget", $"updateWeatherWidget('{cityId}');", true);
                }
                else
                {
                    lblMessage.Text = "Girilen şehir için ID bulunamadı.";
                }
            }
            else
            {
                lblMessage.Text = "Lütfen bir şehir adı girin.";
            }


        }
        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            // Session'dan email bilgisini al
            string userEmail = Session["User"] as string;

            string city = null;
            string commentText = txtComment.Text.Trim();

            if (string.IsNullOrEmpty(commentText))
            {
                lblMessage.Text = "Lütfen tüm alanları doldurun.";
                return;
            }

            try
            {
                using (var connection = Connection.GetConnection())
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
            string City = txtCity.Text;

            try
            {
                using (var connection = Connection.GetConnection())
                {
                    string query = "SELECT id,email, City, CommentText, CreatedAt " +
                                   "FROM Comments " +
                                   "WHERE isActive = 1 and isApproved = 1 and City = @City " +
                                   "ORDER BY CreatedAt DESC";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@City", City);

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
            // Geçerli bir yorum ID'si olup olmadığını kontrol edin
            if (int.TryParse(e.CommandArgument.ToString(), out int commentId))
            {
                if (e.CommandName == "EditComment")
                {
                    // Düzenleme işlemi
                    LoadCommentForEditing(commentId);
                }
                else if (e.CommandName == "DeActiveComment")
                {
                    // Yorum pasif hale getirme işlemi
                    DeactivateComment(commentId);

                }
                else
                {
                    lblMessage.Text = "Geçersiz komut.";
                }
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Geçersiz yorum ID'si.";
            }
        }

        private void LoadCommentForEditing(int commentId)
        {
            // Kullanıcı oturumundan e-posta bilgisini alın
            string currentUserEmail = Session["User"] as string;

            // Eğer oturum bilgisi boşsa, işlem yapılmaz
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                lblMessage.Text = "Kullanıcı oturumu geçersiz.";
                return;
            }

            try
            {
                using (var connection = Connection.GetConnection())
                {
                    // Sorgu: Yorum sahibini ve metni al
                    string query = "SELECT email, CommentText FROM Comments WHERE id = @ID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", commentId);

                    // Veritabanı bağlantısını aç
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Yorum sahibinin e-posta adresi ve yorum metni
                            string commentOwnerEmail = reader["email"].ToString();
                            string commentText = reader["CommentText"].ToString();

                            // Kullanıcı yetkilendirme kontrolü
                            if (!string.Equals(commentOwnerEmail, currentUserEmail, StringComparison.OrdinalIgnoreCase)
                                && !IsAdmin(currentUserEmail))
                            {
                                lblMessage.Text = "Bu yorumu düzenleme yetkiniz yok.";
                                return;
                            }

                            // Düzenleme alanlarını doldur ve paneli görünür yap
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
                // Hata mesajını kullanıcıya göster
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = $"Yorum düzenlenirken hata oluştu: {ex.Message}";
            }
        }


        protected void btnSaveComment_Click(object sender, EventArgs e)
        {

            int commentId = Convert.ToInt32(hfCommentId.Value);
            string updatedComment = txtEditComment.Text.Trim();

            if (string.IsNullOrEmpty(updatedComment))
            {
                lblMessage.Text = "Yorum boş bırakılamaz.";
                return;
            }

            try
            {
                using (var connection = Connection.GetConnection())
                {
                    string query = "UPDATE Comments SET CommentText = @CommentText WHERE id = @ID AND email = @UserEmail";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CommentText", updatedComment);
                    command.Parameters.AddWithValue("@ID", commentId);
                    command.Parameters.AddWithValue("@UserEmail", Session["User"]);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        lblMessage.Text = "Yorum başarıyla güncellendi.";
                        pnlEditComment.Visible = false;
                        LoadComments(); // Güncel yorumları yeniden yükle
                    }
                    else
                    {
                        lblMessage.Text = "Yorum güncellenemedi. Yetkiniz olmayabilir.";
                    }
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
            // Kullanıcı oturumundan e-posta bilgisi alınır
            string currentUserEmail = Session["User"] as string;

            // Eğer oturum bilgisi boşsa işlem yapılmaz
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                lblMessage.Text = "Kullanıcı oturumu geçersiz.";
                return;
            }

            try
            {
                using (var connection = Connection.GetConnection())
                {
                    // Yorumun varlığını ve sahibini kontrol eden sorgu
                    string checkQuery = "SELECT email FROM Comments WHERE id = @ID AND isActive = TRUE";
                    MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@ID", commentId);

                    connection.Open();
                    object result = checkCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string commentOwnerEmail = result.ToString();

                        // Yorum sahibi mi veya admin mi kontrol edilir
                        if (!string.Equals(commentOwnerEmail, currentUserEmail, StringComparison.OrdinalIgnoreCase)
                            && !IsAdmin(currentUserEmail))
                        {
                            lblMessage.Text = "Bu yorumu silme yetkiniz yok.";
                            return;
                        }

                        // Yorumu pasif hale getiren sorgu
                        string updateQuery = "UPDATE Comments SET isActive = FALSE WHERE id = @ID";
                        MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@ID", commentId);
                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblMessage.Text = "Yorum başarıyla silindi.";
                            LoadComments(); // Yorumlar yeniden yüklenir
                        }
                        else
                        {
                            lblMessage.Text = "Yorum silinemedi. Bir sorun oluştu.";
                        }
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

            try
            {
                using (var connection = Connection.GetConnection())
                {
                    string query = "SELECT isAdmin FROM user WHERE email = @Email";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToBoolean(result);
                    }
                }
            }
            catch (Exception)
            {
                // Hata durumunda admin olmayan bir kullanıcı varsayılır
            }

            return false;
        }










    }
}