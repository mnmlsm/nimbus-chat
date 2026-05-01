using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusChat.WetterChatApp.Infrastructure;

namespace NimbusChat.WetterChatApp.Services
{
    public interface INavigationService
    {
        void NavigateTo(BaseViewModel viewModel);
    }
}