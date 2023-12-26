using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Converter
{
    internal class ConverterViewModel : BindableObject
    {
        private string[] _currencyCharCodes;
        public string[] CurrencyCharCodes
        {
            get => _currencyCharCodes;
            set
            {
                _currencyCharCodes = value;
                OnPropertyChanged(nameof(CurrencyCharCodes));
            }
        }

        private string[] _curName;
        public string[] CurName
        {
            get => _curName;
            set
            {
                _curName = value;
                OnPropertyChanged(nameof(CurName));
            }
        }

        private ValuteChangerViewModel _firstValute;
        public ValuteChangerViewModel firstValute => _firstValute;

        private ValuteChangerViewModel _secondValute;
        public ValuteChangerViewModel secondValute => _secondValute;

        public Exchanger Exchange_data { get; set; }

        private DateTime _latestDate;

        private HttpClient _client;

        private ValuteItem _ruble = new("RUB", 1, "Российский рубль", 1);

        public ConverterViewModel()
        {
            _client = new HttpClient();
            ChangeDateOnLatest();
            _firstValute.EntryText = 1;

        }

        private void ChangeDate()
        {
            var date = Exchange_data.Date;
            if (date.Date == _latestDate.Date)
            {
                ChangeDateOnLatest();
                return;
            }

            Uri uri;
            HttpResponseMessage response;
            do
            {
                uri = new Uri(GetDateUri(date));
                response = _client.GetAsync(uri).Result;
                date = date.AddDays(-1);
            }
            while (!response.IsSuccessStatusCode);

            GetData(uri);
        }


        private void ChangeDateOnLatest()
        {
            var uri = new Uri($"https://www.cbr-xml-daily.ru/daily_json.js");
            GetData(uri);
            _latestDate = Exchange_data.Date;
        }


        private void GetData(Uri uri)
        {
            var response = _client.GetAsync(uri).Result;
            var result = response.Content.ReadAsStringAsync().Result;

            Exchange_data = JsonSerializer.Deserialize<Exchanger>(result);
            Exchange_data.Valute.Add(_ruble.CharCode, _ruble);
            Exchange_data.DatePicked += ChangeDate;
            Exchange_data.DatePicked += OnfirstValuteNominalChanged;

            if (firstValute == null || secondValute == null)
                takeValute();
            else
                UpdateValute();
        }

        private void takeValute()
        {
            CurrencyCharCodes = Exchange_data.Valute.Keys.ToArray();
            _curName = Exchange_data.Valute.Values
                .Select(value => value.Name)
                .Zip(_currencyCharCodes, (s, s1) => $"[{s1}] {s}").ToArray();

            _firstValute = new ValuteChangerViewModel(CurrencyCharCodes, Exchange_data, 13);
            _secondValute = new ValuteChangerViewModel(CurrencyCharCodes, Exchange_data, 43);
            _firstValute.NominalChanged += OnfirstValuteNominalChanged;
            _secondValute.NominalChanged += OnsecondValuteNominalChanged;
        }

        private void UpdateValute()
        {
            _firstValute.UpdateExchanger(Exchange_data);
            _secondValute.UpdateExchanger(Exchange_data);
        }

        private void OnfirstValuteNominalChanged()
        {
            secondValute.SetNominal(firstValute.EntryText * firstValute.Value / secondValute.Value);
        }

        private void OnsecondValuteNominalChanged()
        {
            firstValute.SetNominal(secondValute.EntryText * secondValute.Value / firstValute.Value);
        }

        private string GetDateUri(DateTime date)
        {
            return string.Format("https://www.cbr-xml-daily.ru/archive/{0:D4}/{1:D2}/{2:D2}/daily_json.js", date.Year, date.Month, date.Day);
        }
    }
}