namespace Converter;

internal class ValuteChangerViewModel : BindableObject
{
    private string enterItem;

    private double _entryText;

    private int _selectedIndex;

    private ValuteItem _valuteItem;

    private string[] _valuteNames;

    private Exchanger _exchanger;


    public double Value => _valuteItem.Value / _valuteItem.Nominal;

    public event Action NominalChanged;

    public ValuteChangerViewModel(string[] valuteNames, Exchanger exchanger, int selectedIndex = 0)
    {
        _valuteNames = valuteNames;
        _exchanger = exchanger;
        ChangeValueIndex(selectedIndex);
    }

    public void UpdateExchanger(Exchanger exchanger)
    {
        _exchanger = exchanger;
        ChangeValueIndex(_selectedIndex);
    }

    public string SelectedItem
    {
        get => enterItem;
        set
        {
            enterItem = value;
            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    public double EntryText
    {
        get => _entryText;
        set
        {
            if (_entryText == value)
                return;

            _entryText = value;
            OnPropertyChanged(nameof(EntryText));
            NominalChanged?.Invoke();
        }
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            OnPropertyChanged(nameof(SelectedIndex));
            ChangeValueIndex(SelectedIndex);
            NominalChanged?.Invoke();
        }
    }

    private void ChangeValueIndex(int index)
    {
        _selectedIndex = index;
        enterItem = _valuteNames[_selectedIndex];
        _valuteItem = _exchanger.Valute[enterItem];
    }

    public void SetNominal(double value)
    {
        _entryText = value;
        OnPropertyChanged(nameof(EntryText));
    }
}