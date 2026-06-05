using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusChat.WetterChatApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public Task<User> LoginAsync(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null)
                return Task.FromResult<User>(null);

            if (user.PasswordHash != password)
                return Task.FromResult<User>(null);

            return Task.FromResult(user);
        }
    }
}