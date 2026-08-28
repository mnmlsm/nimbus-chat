using System.Security.Cryptography;
using System.Text;

namespace NimbusChat.Api.Security
{
    // Turns a plaintext password into the hex string stored in Users.PasswordHash.
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);

            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        public static bool Verify(string password, string storedHash) =>
            string.Equals(Hash(password), storedHash, StringComparison.Ordinal);
    }
}
