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
        public void SaveCustomMoney(Money newList)
        {
            var jsonString = JsonSerializer.Serialize(newList);
            File.WriteAllText("customMoney.json", jsonString);
        }

        public List<Money> LoadCustomMoney()
        {
            if (File.Exists("customMoney.json"))
            {
                var jsonString = File.ReadAllText("customMoney.json");
                var money = JsonSerializer.Deserialize<Money>(jsonString);
                return new List<Money> { money };
            }
            return new List<Money>();
        }
    }
}
