using Converter;

namespace Converter;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ConverterViewModel();
    }
}