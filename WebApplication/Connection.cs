using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication
{

    public static class Connection
    {
        // Bağlantı dizesi (Connection String)
        private static readonly string connectionString = "Server=localhost;Database=mydatabase;User=root;Password=12345;";

        // MySQL Bağlantısını döndüren metot
        public static MySqlConnection GetConnection()
        {
            try
            {
                var connection = new MySqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Veritabanına bağlanırken bir hata oluştu.", ex);
            }
        }

        public static string ApiConnection(string city)
        {
            string apiKey = "451ea1379d2c469747b294bf43a5462c"; // OpenWeather API anahtarınızı buraya ekleyin
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=tr";
            return apiUrl;
        }

        public static ArrayList GetCity(string city)
        {

            ArrayList lines = new ArrayList(File.ReadAllLines("C:\\Users\\efeka\\source\\repos\\WebApplication\\WebApplication\\city_names.txt"););
            foreach (string line in lines)
            {
                if (!line.ToLower().StartsWith(city.ToLower())){
                    lines.Remove(line);
                    
                }
            }


            return lines;


        }
    }
}

