using System.ComponentModel;
using System.Windows;
using NimbusChat.ViewModels;

namespace NimbusChat
{
    public partial class WeatherWindow : Window
    {
        public WeatherViewModel ViewModel => (WeatherViewModel)DataContext;

        public WeatherWindow()
        {
            InitializeComponent();

            ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(WeatherViewModel.DialogResultValue))
                {
                    DialogResult = ViewModel.DialogResultValue;
                }
            };
        }
    }
}