using app.models;
using System;
using System.Collections.Generic;
using System.IO;
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

        private string jsonString = "";
        private string LOCAL_FILE_PATH = "localData.json";
        public async Task<List<Money>> LoadData()
        {

            if (File.Exists(LOCAL_FILE_PATH))
            {
                jsonString = File.ReadAllText(LOCAL_FILE_PATH);
            }
            if (string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    jsonString = await client.GetStringAsync(Url);

                    _localData.SaveToJson(jsonString);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка загрузки данных: {e.Message}");
                    return new List<Money>();
                }
            }

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, option);

            var valute = dictionary["Valute"].ToString();

            var dict = JsonSerializer.Deserialize<Dictionary<string, Money>>(valute, option);

            return dict.Values.ToList();

        }
    }
}
