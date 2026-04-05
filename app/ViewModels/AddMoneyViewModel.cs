using app.models;
using app.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace app.ViewModels
{
    public partial class AddMoneyViewModel : ObservableObject
    {
        private readonly ObservableCollection<Money> _moneyList =new();
        private LocalData _localData = new LocalData();

        [ObservableProperty]
        private string _charCode;
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private decimal _value;

        public AddMoneyViewModel(ObservableCollection<Money> moneyList)
        {
            _moneyList = moneyList;
        }

        [RelayCommand]
        public void Save()
        {
            var newMoney = new Money
            {
                ID = Guid.NewGuid().ToString(),
                CharCode = _charCode,
                Name = _name,
                Value = _value,
                isCustom = true,
                Previous = _value,
                Nominal = 1,
            };

            App.Current.Dispatcher.Invoke(() => _moneyList.Add(newMoney));
            OnPropertyChanged(nameof(_moneyList));
            _localData.SaveCustomMoney(newMoney);

            foreach (Window window in Application.Current.Windows)
            {
                if((Object)window.DataContext == (object)this)
                {
                    window.Close();
                    break;
                }
            }
        }

        

    }
}
