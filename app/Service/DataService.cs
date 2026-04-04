using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace app.Service
{
    class DataService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task LoadData()
        {
            string url = "https://www.cbr-xml-daily.ru/daily_json.js";

            using (client)
            {
                try
                {
                    string jsonString = await client.GetStringAsync(url);
                    var option = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                }
                catch(Exception e)
                {
                    MessageBox.Show("Ошибка загрузки данных: {e.Message}");
                }
            }
        }



    }
}
