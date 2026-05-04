using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NimbusChat.WetterChatApp.Data;

namespace NimbusChat
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // DB beim Start initialisieren (Woche 1)
            DatabaseInitializer.Initialize();
        }
    }
}