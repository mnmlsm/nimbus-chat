using NimbusChat.WetterChatApp.Models;
using NimbusChat.WetterChatApp.Repositories;
using System;
using System.Security.Cryptography;
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

        // Async-Login mit Passwort-Hashing
        public Task<User> LoginAsync(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null)
                return Task.FromResult<User>(null);

            // Passwort hashen und mit gespeicherten Hash vergleichen
            var hash = HashPassword(password);
            if (!string.Equals(user.PasswordHash, hash, StringComparison.Ordinal))
                return Task.FromResult<User>(null);

            return Task.FromResult(user);
        }

        // Sync-Login mit Passwort-Hashing
        public User Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null)
                return null;

            var hash = HashPassword(password);
            if (!string.Equals(user.PasswordHash, hash, StringComparison.Ordinal))
                return null;

            return user;
        }

        /// <summary>
        /// Erstellt einen SHA-256-Hash des Passworts als Hex-String.
        /// </summary>
        public string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString(); // z.B. "9e107d9d372bb6826bd81d3542a419d6..."
            }
        }
    }
}