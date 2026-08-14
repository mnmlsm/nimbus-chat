using System;
using System.Collections.Generic;

namespace NimbusChat
{
    public enum AppLanguage
    {
        English,
        German,
        Russian
    }

    public static class LanguageManager
    {

        public static string GetDayName(DateTime date)
        {
            switch (CurrentLanguage)
            {
                case AppLanguage.German:
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday: return "Mo";
                        case DayOfWeek.Tuesday: return "Di";
                        case DayOfWeek.Wednesday: return "Mi";
                        case DayOfWeek.Thursday: return "Do";
                        case DayOfWeek.Friday: return "Fr";
                        case DayOfWeek.Saturday: return "Sa";
                        case DayOfWeek.Sunday: return "So";
                    }
                    break;

                case AppLanguage.Russian:
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday: return "Пн";
                        case DayOfWeek.Tuesday: return "Вт";
                        case DayOfWeek.Wednesday: return "Ср";
                        case DayOfWeek.Thursday: return "Чт";
                        case DayOfWeek.Friday: return "Пт";
                        case DayOfWeek.Saturday: return "Сб";
                        case DayOfWeek.Sunday: return "Вс";
                    }
                    break;

                default:
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday: return "Mon";
                        case DayOfWeek.Tuesday: return "Tue";
                        case DayOfWeek.Wednesday: return "Wed";
                        case DayOfWeek.Thursday: return "Thu";
                        case DayOfWeek.Friday: return "Fri";
                        case DayOfWeek.Saturday: return "Sat";
                        case DayOfWeek.Sunday: return "Sun";
                    }
                    break;
            }

            return string.Empty;
        }
        public static AppLanguage CurrentLanguage { get; private set; }
            = AppLanguage.English;

        public static event EventHandler LanguageChanged;

        public static void SetLanguage(AppLanguage language)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;

            if (LanguageChanged != null)
                LanguageChanged(null, EventArgs.Empty);
        }

        public static string Get(string key)
        {
            Dictionary<string, string> dictionary;

            switch (CurrentLanguage)
            {
                case AppLanguage.German:
                    dictionary = German;
                    break;

                case AppLanguage.Russian:
                    dictionary = Russian;
                    break;

                default:
                    dictionary = English;
                    break;
            }

            string value;

            if (dictionary.TryGetValue(key, out value))
                return value;

            return key;
        }

        private static readonly Dictionary<string, string> English =
            new Dictionary<string, string>
            {
                { "EditProfile", "Edit Profile" },
                { "UpdateAccountDetails", "Update your account details" },
                { "Username", "USERNAME" },
                { "Email", "EMAIL" },
                { "Status", "STATUS" },
                { "FavoriteCityLabel", "FAVORITE CITY" },
                { "SaveChanges", "Save Changes" },
                { "ProfileSaved", "Profile saved!" },
                { "Success", "Success" },
                { "Offline", "Offline" },
                { "Online", "Online" },
                { "Busy", "Busy" },
                { "Away", "Away" },
                { "GlobalChat", "Global Chat" },
                { "MessagesTitle", "Messages" },
                { "YourConversations", "Your conversations" },
                { "Search", "Search..." },
                { "SelectChat", "Select a chat" },
                { "Location", "LOCATION" },
                { "Everyone", "Everyone" },
                { "MessageSendError", "Message could not be sent." },
                { "Error", "Error" },
                { "WeatherSearch", "Weather Search" },
                { "SearchWeatherAnywhere", "Search weather anywhere" },
                { "City", "CITY" },
                { "SearchWeather", "Search Weather" },
                { "Humidity", "Humidity" },
                { "Wind", "Wind" },
                { "FeelsLike", "Feels Like" },
                { "UseForDashboard", "Use for Dashboard" },
                { "Open", "Open" },
                { "ViewForecast", "View forecast" },
                { "OpenChats", "Open chats" },
                { "EditAccount", "Edit account" },
                { "YourProfile", "Your profile" },
                { "WeatherDashboard", "Weather dashboard" },
                { "CurrentWeather", "CURRENT WEATHER" },
                { "Condition", "CONDITION" },
                { "WeatherRightNow", "Weather right now" },
                { "GoodMorning", "Good Morning" },
                { "GoodAfternoon", "Good Afternoon" },
                { "GoodEvening", "Good Evening" },
                { "GoodNight", "Good Night" },
                { "SelectCity", "Select your city." },
                { "WeatherUnavailable", "Weather unavailable" },
                { "FavoriteCity", "FAVORITE CITY" },
                { "SavedToProfile", "Saved to your profile" },
                { "WeatherNotConnected", "WEATHER · NOT CONNECTED" },
                { "Menu", "MENU" },
                { "WeatherConnected", "WEATHER · CONNECTED" },
                { "Settings", "Settings" },
                { "SettingsDescription", "Customize your NimbusChat experience." },
                { "Appearance", "Appearance" },
                { "AppearanceDescription", "Choose how NimbusChat looks." },
                { "Language", "Language" },
                { "LanguageDescription", "Choose the language of NimbusChat." },
                { "Application", "Application" },
                { "WeatherMessenger", "Weather & Messenger" },
                { "Close", "Close" },

                { "Dashboard", "Dashboard" },
                { "Weather", "Weather" },
                { "Messages", "Messages" },
                { "Profile", "Profile" },
            };

        private static readonly Dictionary<string, string> German =
            new Dictionary<string, string>
            {
                { "EditProfile", "Profil bearbeiten" },
                { "UpdateAccountDetails", "Aktualisiere deine Kontodaten" },
                { "Username", "BENUTZERNAME" },
                { "Email", "E-MAIL" },
                { "Status", "STATUS" },
                { "FavoriteCityLabel", "LIEBLINGSSTADT" },
                { "SaveChanges", "Änderungen speichern" },
                { "ProfileSaved", "Profil gespeichert!" },
                { "Success", "Erfolg" },
                { "Offline", "Offline" },
                { "Online", "Online" },
                { "Busy", "Beschäftigt" },
                { "Away", "Abwesend" },
                { "GlobalChat", "Globaler Chat" },
                { "MessagesTitle", "Nachrichten" },
                { "YourConversations", "Deine Unterhaltungen" },
                { "Search", "Suchen..." },
                { "SelectChat", "Chat auswählen" },
                { "Everyone", "Alle" },
                { "MessageSendError", "Nachricht konnte nicht gesendet werden." },
                { "Error", "Fehler" },
                { "WeatherSearch", "Wettersuche" },
                { "SearchWeatherAnywhere", "Wetter überall suchen" },
                { "City", "STADT" },
                { "SearchWeather", "Wetter suchen" },
                { "Humidity", "Luftfeuchtigkeit" },
                { "Wind", "Wind" },
                { "FeelsLike", "Gefühlt wie" },
                { "UseForDashboard", "Für Dashboard verwenden" },
                { "Open", "Öffnen" },
                { "ViewForecast", "Vorhersage anzeigen" },
                { "OpenChats", "Chats öffnen" },
                { "EditAccount", "Konto bearbeiten" },
                { "YourProfile", "Dein Profil" },
                { "WeatherDashboard", "Wetter-Dashboard" },
                { "CurrentWeather", "AKTUELLES WETTER" },
                { "Condition", "ZUSTAND" },
                { "WeatherRightNow", "Wetter aktuell" },
                { "GoodMorning", "Guten Morgen" },
                { "GoodAfternoon", "Guten Tag" },
                { "GoodEvening", "Guten Abend" },
                { "GoodNight", "Gute Nacht" },
                { "SelectCity", "Wähle deine Stadt." },
                { "Location", "ORT" },
                { "FavoriteCity", "FAVORITENSTADT" },
                { "SavedToProfile", "In deinem Profil gespeichert" },
                { "WeatherNotConnected", "WETTER · NICHT VERBUNDEN" },
                { "Menu", "MENÜ" },
                { "WeatherConnected", "WETTER · VERBUNDEN" },
                { "Settings", "Einstellungen" },
                { "SettingsDescription", "Passe dein NimbusChat-Erlebnis an." },
                { "Appearance", "Darstellung" },
                { "AppearanceDescription", "Wähle das Erscheinungsbild von NimbusChat." },
                { "Language", "Sprache" },
                { "LanguageDescription", "Wähle die Sprache von NimbusChat." },
                { "Application", "Anwendung" },
                { "WeatherMessenger", "Wetter & Messenger" },
                { "Close", "Schließen" },

                { "Dashboard", "Dashboard" },
                { "Weather", "Wetter" },
                { "Messages", "Nachrichten" },
                { "Profile", "Profil" },
                { "Logout", "Abmelden" }
            };

        private static readonly Dictionary<string, string> Russian =
            new Dictionary<string, string>
            {
                { "EditProfile", "Редактировать профиль" },
                { "UpdateAccountDetails", "Обновите данные своего аккаунта" },
                { "Username", "ИМЯ ПОЛЬЗОВАТЕЛЯ" },
                { "Email", "ЭЛЕКТРОННАЯ ПОЧТА" },
                { "Status", "СТАТУС" },
                { "FavoriteCityLabel", "ЛЮБИМЫЙ ГОРОД" },
                { "SaveChanges", "Сохранить изменения" },
                { "ProfileSaved", "Профиль сохранён!" },
                { "Success", "Успешно" },
                { "Offline", "Не в сети" },
                { "Online", "В сети" },
                { "Busy", "Занят" },
                { "Away", "Отошёл" },
                { "GlobalChat", "Общий чат" },
                { "Everyone", "Все" },
                { "MessagesTitle", "Сообщения" },
                { "YourConversations", "Ваши разговоры" },
                { "Search", "Поиск..." },
                { "SelectChat", "Выберите чат" },
                { "MessageSendError", "Не удалось отправить сообщение." },
                { "Error", "Ошибка" },
                { "WeatherSearch", "Поиск погоды" },
                { "SearchWeatherAnywhere", "Найдите погоду в любом городе" },
                { "City", "ГОРОД" },
                { "SearchWeather", "Найти погоду" },
                { "Humidity", "Влажность" },
                { "Wind", "Ветер" },
                { "FeelsLike", "Ощущается как" },
                { "UseForDashboard", "Использовать на главной" },
                { "Open", "Открыть" },
                { "ViewForecast", "Посмотреть прогноз" },
                { "OpenChats", "Открыть чаты" },
                { "EditAccount", "Редактировать аккаунт" },
                { "YourProfile", "Ваш профиль" },
                { "WeatherDashboard", "Погодная панель" },
                { "CurrentWeather", "ТЕКУЩАЯ ПОГОДА" },
                { "Condition", "СОСТОЯНИЕ" },
                { "WeatherRightNow", "Погода сейчас" },
                { "GoodMorning", "Доброе утро" },
                { "GoodAfternoon", "Добрый день" },
                { "GoodEvening", "Добрый вечер" },
                { "GoodNight", "Доброй ночи" },
                { "SelectCity", "Выберите город." },
                { "WeatherUnavailable", "Погода недоступна" },
                { "Location", "МЕСТОПОЛОЖЕНИЕ" },
                { "FavoriteCity", "ЛЮБИМЫЙ ГОРОД" },
                { "SavedToProfile", "Сохранено в вашем профиле" },
                { "WeatherNotConnected", "ПОГОДА · НЕ ПОДКЛЮЧЕНО" },
                { "Menu", "МЕНЮ" },
                { "WeatherConnected", "ПОГОДА · ПОДКЛЮЧЕНО" },
                { "Settings", "Настройки" },
                { "SettingsDescription", "Настройте работу NimbusChat." },
                { "Appearance", "Внешний вид" },
                { "AppearanceDescription", "Выберите внешний вид NimbusChat." },
                { "Language", "Язык" },
                { "LanguageDescription", "Выберите язык NimbusChat." },
                { "Application", "Приложение" },
                { "WeatherMessenger", "Погода и сообщения" },
                { "Close", "Закрыть" },

                { "Dashboard", "Главная" },
                { "Weather", "Погода" },
                { "Messages", "Сообщения" },
                { "Profile", "Профиль" },
                { "Logout", "Выйти" }
            };
    }
}