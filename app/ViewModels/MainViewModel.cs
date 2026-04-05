using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using app.Service;
using app.models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace app.ViewModels
{
    public partial class MainViewModel : ObservableObject 
    {
        public MainViewModel() { }
        private readonly DataService _dataService = new DataService();
        private LocalData _localData = new LocalData();

        [ObservableProperty]
        private ObservableCollection<Money> _moneyList = new();

        [ObservableProperty]
        private string _lastSessionText;

        [RelayCommand]
        private async Task RefreshData()
        {
            var data = await _dataService.LoadData();
            MoneyList.Clear();
            foreach (var item in data)
            {
                MoneyList.Add(item);
            }
        }

        [RelayCommand]
        private void OpenAddMoney()
        {
            var addMoneyWindow = new AddMoney();
            addMoneyWindow.DataContext = new AddMoneyViewModel(MoneyList);
            addMoneyWindow.ShowDialog();

        }

        [RelayCommand]
        public async Task DeleteMoney(Money moneyToDelete)
        {
            if(moneyToDelete == null)
            {
                return;
            }
            MoneyList.Remove(moneyToDelete);
            await _dataService.DeleteMoney(moneyToDelete);

        }

        [RelayCommand]
        private async Task UpdateData()
        {
            var data = await _dataService.DownloadDataRaw();
            var upadatedList = await _dataService.UpdateData(data);
            _localData.SaveToJson(data);
            MoneyList.Clear();
            foreach (var item in upadatedList)
            {
                MoneyList.Add(item);
            }
        }

        [RelayCommand]
        public async Task InitializerDate()
        {
            try
            {
                DateTime? lastdate = _localData.LoadLastSession();
                if (lastdate.HasValue)
                {
                    LastSessionText = $"Последняя сессия: {lastdate.Value.ToString("dd.MM.yyyy HH:mm:ss")}";
                }
                else
                {
                    LastSessionText = "Последняя сессия: данные не найдены";
                }
                _localData.SaveLastSession();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
    }
}
