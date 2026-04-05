using app.models;
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
        private const string Url = "https://www.cbr-xml-daily.ru/daily_json.js";
        private LocalData _localData = new LocalData();
        public async Task<List<Money>> LoadData()
        {

            try
            {
                string jsonString = await client.GetStringAsync(Url);

                _localData.SaveToJson(jsonString);

                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, option);

                var valute = dictionary["Valute"].ToString();

                var dict = JsonSerializer.Deserialize<Dictionary<string, Money>>(valute, option);

                return dict.Values.ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка загрузки данных: {e.Message}");
                return new List<Money>();
            }


        }



    }
}
