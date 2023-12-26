using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Converter
{
    public class Exchanger : BindableObject
    {
        private DateTime _date;

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                DatePicked?.Invoke();
                DateText = _date.ToString();
                OnPropertyChanged();
            }
        }

        public event Action DatePicked;

        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, ValuteItem> Valute { get; set; }

        private string _dataText;
        public string DateText
        {
            get
            {
                return "Показан курс на: " + _dataText;
            }
            set
            {
                _dataText = value;
                OnPropertyChanged();
            }
        }
    }

    public class ValuteItem
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }

        public ValuteItem(string charCode, int nominal, string name, double value)
        {
            CharCode = charCode;
            Nominal = nominal;
            Name = name;
            Value = value;
        }
    }
}