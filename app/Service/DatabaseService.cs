using app.models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace app.Services
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _db;

        public DatabaseService()
        {
            string dbPath = Path.Combine(Environment.CurrentDirectory, "currency.db");

            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Money>();
        }

        public List<Money> GetAllCurrencies()
        {
            return _db.Table<Money>().ToList();
        }

        public void SaveCurrency(Money money)
        {
            _db.Insert(money);
        }

        public void DeleteCurrency(Money money)
        {
            _db.Delete(money);
        }
        public void SaveDataDb(List<Money> apiData)
        {
            _db.RunInTransaction(() =>
            {
  
                _db.Table<Money>().Where(x => x.isCustom == false).Delete();

                foreach (var item in apiData)
                {
                    item.isCustom = false; 
                    _db.Insert(item);
                }
            });
        }
    }
}