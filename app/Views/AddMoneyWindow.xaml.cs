using app.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace app
{
    public partial class AddMoney : Window
    {
        private static readonly Regex _regex = new Regex(@"^[0-9]*[.,]?[0-9]*$");

        public AddMoney()
        {
            InitializeComponent();
        }
        private bool IsTextAllow(string text)
        {
            return _regex.IsMatch(text);
        }

        private void Value_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.Text == "." || e.Text == ",")
            {
                if (textBox.Text.Contains(".") || textBox.Text.Contains(","))
                {
                    e.Handled = true; 
                }
                else
                {
                    e.Handled = false; 
                }
                return;
            }
            string currentText = textBox.Text;
            if (textBox.SelectionLength > 0)
                currentText = currentText.Remove(textBox.SelectionStart, textBox.SelectionLength);

            string fullText = currentText.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !IsTextAllow(fullText);
        }
        private void Value_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pasteText = (string)e.DataObject.GetData(typeof(string));
                var textBox = sender as TextBox;

                string currentText = textBox.Text;
                if (textBox.SelectionLength > 0)
                {
                    currentText = currentText.Remove(textBox.SelectionStart, textBox.SelectionLength);
                }

                string fullText = currentText.Insert(textBox.SelectionStart, pasteText);

                if (!IsTextAllow(fullText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}