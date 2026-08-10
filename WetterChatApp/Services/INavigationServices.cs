using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NimbusChat.WetterChatApp.Services
{
    // Abstraction for showing/closing windows, intended to keep view models
    // decoupled from concrete Window types.
    public interface INavigationServices
    {
        void ShowWindow(Window window);
        void CloseWindow(Window window);
    }
}