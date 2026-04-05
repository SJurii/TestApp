using app.models;
using app.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Packaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            if(string.IsNullOrWhiteSpace(CharCode) || CharCode.Length != 3 || !CharCode.All(char.IsLetter))
            {
                MessageBox.Show("Код валюты должен состоять ровно из 3 букв (например, USD)",
                        "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(string.IsNullOrWhiteSpace(Name) || Name.Any(char.IsDigit))
            {
                MessageBox.Show("Название не может быть пустым или содержать цифры",
                        "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal? normValue = NormalizeValue(Value.ToString());

            if (normValue == null)
            {
                MessageBox.Show("Пожалуйста, введите корректный курс (число больше 0).",
                                "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal finalValue = normValue.Value;
            


            var newMoney = new Money
            {
                ID = Guid.NewGuid().ToString(),
                CharCode = _charCode,
                Name = _name,
                Value = finalValue,
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

        private decimal? NormalizeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            string normValue = value.Replace(',', '.').Trim();
            if (decimal.TryParse(normValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                if (result > 0) return result;
            }
            return null;
        }



    }
}
