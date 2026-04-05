using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace app.models
{
    public class Money
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Previous { get; set; }
        public bool isCustom { get; set; } = false;
    }
}
