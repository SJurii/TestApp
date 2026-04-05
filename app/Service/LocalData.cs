using app.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Text.Json;

namespace app.Service
{
    public class LocalData
    {
        private string LOCAL_FILE_PATH = "localData.json";
        private string LOCAL_CUSTOM_PATH = "customMoney.json";
        private string LOCAL_DATE_PATH = "appSettings.json";

        public void SaveToJson(string data)
        {
            File.WriteAllText(LOCAL_FILE_PATH, data);
        }

        public void SaveCustomMoney(Money newMoney)
        {
            List<Money> currentList = LoadCustomMoney();
            currentList.Add(newMoney);
            var jsonString = JsonSerializer.Serialize(currentList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LOCAL_CUSTOM_PATH, jsonString);
        }

        public List<Money> LoadCustomMoney()
        {
            if (File.Exists(LOCAL_CUSTOM_PATH))
            {
                try
                {
                    var jsonString = File.ReadAllText(LOCAL_CUSTOM_PATH);
                    
                    return JsonSerializer.Deserialize<List<Money>>(jsonString) ?? new List<Money>();
                } catch 
                {
                    return new List<Money>();
                }
            }
            else
            {
                return new List<Money>();
            }
        }

        public string ReadLocalFile()
        {
            return File.ReadAllText(LOCAL_FILE_PATH);
        }

        public void SaveLastSession()
        {
            var lastSession = new AppSettings
            {
                LastSession = DateTime.Now
            };
            var jsonString = JsonSerializer.Serialize(lastSession, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LOCAL_DATE_PATH, jsonString);
        }

        public DateTime? LoadLastSession()
        {
            if (File.Exists(LOCAL_DATE_PATH))
            {
                try
                {
                    var jsonString = File.ReadAllText(LOCAL_DATE_PATH);
                    var appSettings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                    return appSettings?.LastSession ?? null;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
