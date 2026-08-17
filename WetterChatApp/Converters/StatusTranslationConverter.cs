using System;
using System.Globalization;
using System.Windows.Data;

namespace NimbusChat
{
    public class StatusTranslationConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            string status = value as string;

            if (string.IsNullOrEmpty(status))
                return string.Empty;

            switch (status)
            {
                case "Online":
                    return LanguageManager.Get("Online");

                case "Away":
                    return LanguageManager.Get("Away");

                case "Busy":
                    return LanguageManager.Get("Busy");

                case "Offline":
                    return LanguageManager.Get("Offline");

                case "Everyone":
                    return LanguageManager.Get("Everyone");

                default:
                    return status;
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}