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

        [ObservableProperty]
        private ObservableCollection<Money> _moneyList = new();

        [RelayCommand]
        private async Task RefreshData()
        {
            var data = await _dataService.LoadData();
            _moneyList.Clear();
            foreach (var item in data)
            {
                _moneyList.Add(item);
            }
        }

        [RelayCommand]
        private void OpenAddMoney()
        {
            var addMoneyWindow = new AddMoney();
            addMoneyWindow.DataContext = new AddMoneyViewModel();
            addMoneyWindow.ShowDialog();

        }
    }
}
