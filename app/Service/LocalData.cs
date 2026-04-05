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
    }
}
