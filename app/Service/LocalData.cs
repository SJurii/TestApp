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
        public void SaveToJson(string data)
        {
            File.WriteAllText("localData.json", data);
        }
        public void SaveCustomMoney(Money newMoney)
        {
            List<Money> currentList = LoadCustomMoney();
            currentList.Add(newMoney);
            var jsonString = JsonSerializer.Serialize(currentList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("customMoney.json", jsonString);
        }

        public List<Money> LoadCustomMoney()
        {
            if (File.Exists("customMoney.json"))
            {
                try
                {
                    var jsonString = File.ReadAllText("customMoney.json");
                    
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

        public void SaveLastSession()
        {
            var lastSession = new AppSettings
            {
                LastSession = DateTime.Now
            };
            var jsonString = JsonSerializer.Serialize(lastSession, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("appSettings.json", jsonString);
        }

        public DateTime? LoadLastSession()
        {
            if (File.Exists("appSettings.json"))
            {
                try
                {
                    var jsonString = File.ReadAllText("appSettings.json");
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
