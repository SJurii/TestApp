using app.models;
using app.Services;
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
        private readonly DatabaseService _dbService = new DatabaseService();

        private string LOCAL_FILE_PATH = "localData.json";
        private string LOCAL_CUSTOM_PATH = "customMoney.json";

        public async Task<List<Money>> LoadData()
        {
            string jsonString = "";
            List<Money> listMoney = new List<Money>();

            if (File.Exists(LOCAL_FILE_PATH))
            {
                jsonString = _localData.ReadLocalFile();

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
            if (listMoney != null && listMoney.Count > 0)
            {
                var apiOnly = listMoney.Where(m => !m.isCustom).ToList();
                _dbService.SaveDataDb(apiOnly);
            }
            return listMoney;

        }



        public async Task<List<Money>> UpdateData(string jsonString)
        {
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if(jsonString == null)
            {
                return new List<Money>();
            }

            if (jsonString.Trim().StartsWith("["))
            {
                var apiList = JsonSerializer.Deserialize<List<Money>>(jsonString, option) ?? new List<Money>();
                var customList = _localData.LoadCustomMoney();

                if(customList.Count > 0)
                {
                    apiList.AddRange(customList);
                }

                return apiList;
            }
            try
            {
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, option);

                var valute = dictionary["Valute"].ToString();

                var dict = JsonSerializer.Deserialize<Dictionary<string, Money>>(valute, option);

                var listMoney = dict.Values.ToList();
                var customMoney = _localData.LoadCustomMoney();

                if (customMoney.Count > 0)
                {
                    listMoney.AddRange(customMoney);
                   
                }
                return listMoney;
            }
            catch
            {
                return new List<Money>();
            }

        }



        public async Task<string> DownloadDataRaw()
        {
            try
            {
                return await client.GetStringAsync(Url);

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task DeleteMoney(Money moneyToDelete)
        {
            if (moneyToDelete == null) return;


            if (moneyToDelete.isCustom == true)
            {
                var customlist = _localData.LoadCustomMoney();

                var item = customlist.FirstOrDefault(m => m.ID.ToString() == moneyToDelete.ID.ToString());
                if (item != null)
                {
                    customlist.Remove(item);
                    _dbService.DeleteCurrency(moneyToDelete);
                    var jsonString = JsonSerializer.Serialize(customlist, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(LOCAL_CUSTOM_PATH, jsonString);
                }
            }
            else
            {
                string jsonstring = _localData.ReadLocalFile();
                var localApiList = await UpdateData(jsonstring);
                var item = localApiList.FirstOrDefault(m => m.ID == moneyToDelete.ID);
                if (item != null)
                {
                    localApiList.Remove(item);
                    _dbService.DeleteCurrency(moneyToDelete);

                    var jsonString = JsonSerializer.Serialize(localApiList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(LOCAL_FILE_PATH, jsonString);
                }
            }

        }
    }
}
