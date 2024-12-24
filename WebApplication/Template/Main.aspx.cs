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

namespace WebApplication.Template
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kullanıcı login olmadıysa Login sayfasına yönlendirme
            if (Session["User"] == null)
            {
                Response.Redirect("LoginPage.aspx");

            }
            if (!IsPostBack)
            {
                lblWeatherInfo.Text = "Hava durumu bilgisi almak için bir şehir adı girin.";
            }
            string apiKey = "451ea1379d2c469747b294bf43a5462c"; // OpenWeather API anahtarınızı buraya ekleyin
            string city = "Istanbul"; // Varsayılan şehir
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=tr";
        }
        protected async void btnSearch_Click(object sender, EventArgs e)
        {
            string city = txtCity.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                lblWeatherInfo.Text = "Lütfen bir şehir adı girin.";
                return;
            }

            string apiKey = "YOUR_API_KEY"; // OpenWeather API anahtarınızı buraya ekleyin
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
            string comment = txtComment.Text.Trim();
            if (string.IsNullOrEmpty(comment))
            {
                lblWeatherInfo.Text = "Lütfen bir yorum yazın.";
                return;
            }

            // Yorumları saklamak için bir Session kullanıyoruz
            List<string> comments = (List<string>)Session["Comments"] ?? new List<string>();

            // Yeni yorumu listeye ekle
            comments.Add(comment);

            // Session'da sakla
            Session["Comments"] = comments;

            // Yorumları yeniden yükle
            LoadComments();
        }

        private void LoadComments()
        {
            List<string> comments = (List<string>)Session["Comments"] ?? new List<string>();

            // commentsContainer'ın içeriğini temizle
            commentsContainer.InnerHtml = "";

            foreach (var comment in comments)
            {
                commentsContainer.InnerHtml += $"<p>{comment}</p>";
            }
        }


    }
}