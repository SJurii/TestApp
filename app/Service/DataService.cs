using app.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;

namespace app.Service
{
    class DataService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string Url = "https://www.cbr-xml-daily.ru/daily_json.js";
        private LocalData _localData = new LocalData();

        private string LOCAL_FILE_PATH = "localData.json";

        public async Task<List<Money>> LoadData()
        {
            string jsonString = "";
            List<Money> listMoney = new List<Money>();

            if (File.Exists(LOCAL_FILE_PATH))
            {
                jsonString = File.ReadAllText(LOCAL_FILE_PATH);
            }
            if (string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    jsonString = await DownloadDataRaw();
                    _localData.SaveToJson(jsonString);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка записи: {e.Message}");
                    return new List<Money>();
                }
            }

            listMoney = await UpdateData(jsonString);

            return listMoney;

        }



        public async Task<List<Money>> UpdateData(string jsonString)
        {
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, option);

            var valute = dictionary["Valute"].ToString();

            var dict = JsonSerializer.Deserialize<Dictionary<string, Money>>(valute, option);

            var listMoney = dict.Values.ToList();
            var customMoney = _localData.LoadCustomMoney();
            if(customMoney.Count > 0)
            {
                listMoney.AddRange(customMoney);
            }

            return listMoney;
        }



        public async Task<string> DownloadDataRaw()
        {
            try
            {
                return await client.GetStringAsync(Url);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка загрузки данных: {e.Message}");
                return null;
            }
        }
    }
}
