using System.Windows;
using NimbusChat.ViewModels;

namespace NimbusChat
{
    public partial class WeatherWindow : Window
    {
        public WeatherWindow()
        {
            InitializeComponent();
            DataContext = new WeatherViewModel(); // ← ВОТ ЭТУ СТРОКУ ДОБАВИТЬ
        }
    }
}
