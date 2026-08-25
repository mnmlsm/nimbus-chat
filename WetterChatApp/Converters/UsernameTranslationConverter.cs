using NimbusChat.WetterChatApp.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NimbusChat
{
    public class UsernameTranslationConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            User user = value as User;

            if (user == null)
                return string.Empty;

            if (user.Id == -1)
                return LanguageManager.Get("GlobalChat");

            return user.Username;
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