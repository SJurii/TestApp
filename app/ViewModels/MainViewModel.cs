using app.models;
using app.Service;
using app.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Text;
using System.Windows;

namespace app.ViewModels
{
    public partial class MainViewModel : ObservableObject 
    {
        public MainViewModel() { }
        private readonly DataService _dataService = new DataService();
        private LocalData _localData = new LocalData();
        private readonly DatabaseService _dbService = new DatabaseService();

        [ObservableProperty]
        private ObservableCollection<Money> _moneyList = new();

        [ObservableProperty]
        private string _lastSessionText;

        [RelayCommand]
        private async Task RefreshData()
        {
            try
            {
                var data = await _dataService.LoadData();
                if (data == null || data.Count == 0)
                {
                    MessageBox.Show("Данные о валютах не найдены. Проверьте интернет-соединение.",
                                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MoneyList.Clear();
                foreach (var item in data)
                {
                    MoneyList.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить валюту '{moneyToDelete.Name}' ({moneyToDelete.CharCode})?", 
                "Подтверждение удаления",                                                   
                MessageBoxButton.YesNo,                                                     
                MessageBoxImage.Warning);

            if(result != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                MoneyList.Remove(moneyToDelete);
                await _dataService.DeleteMoney(moneyToDelete);
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }

           

        }

        [RelayCommand]
        private async Task UpdateData()
        {
            try
            {
                var data = await _dataService.DownloadDataRaw();

                if (string.IsNullOrEmpty(data)) throw new Exception("Сервер вернул пустой ответ.");

                var upadatedList = await _dataService.UpdateData(data);

                _localData.SaveToJson(data);

                MoneyList.Clear();
                foreach (var item in upadatedList)
                {
                    MoneyList.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
