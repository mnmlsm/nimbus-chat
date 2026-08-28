using NimbusChat.Api.Security;
using Xunit;

namespace NimbusChat.Api.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_IsDeterministic()
        {
            Assert.Equal(PasswordHasher.Hash("NimbusChat2026!"), PasswordHasher.Hash("NimbusChat2026!"));
        }

        [Fact]
        public void Hash_Returns64LowercaseHexCharacters()
        {
            var hash = PasswordHasher.Hash("NimbusChat2026!");

            Assert.Equal(64, hash.Length);
            Assert.All(hash, c => Assert.True("0123456789abcdef".Contains(c), $"'{c}' is not lowercase hex."));
        }

        // Guards the stored-hash format: if this ever changes, every existing
        // account in the Users table stops being able to log in.
        [Fact]
        public void Hash_MatchesKnownSha256Vector()
        {
            Assert.Equal(
                "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                PasswordHasher.Hash("password"));
        }

        [Fact]
        public void Hash_DiffersForDifferentPasswords()
        {
            Assert.NotEqual(PasswordHasher.Hash("passwort-eins"), PasswordHasher.Hash("passwort-zwei"));
        }

        [Fact]
        public void Hash_IsCaseSensitive()
        {
            Assert.NotEqual(PasswordHasher.Hash("Nimbus"), PasswordHasher.Hash("nimbus"));
        }

        [Fact]
        public void Hash_HandlesEmptyPassword()
        {
            Assert.Equal(
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                PasswordHasher.Hash(string.Empty));
        }

        [Fact]
        public void Hash_HandlesUmlautsAsUtf8()
        {
            Assert.NotEqual(PasswordHasher.Hash("Grussformel"), PasswordHasher.Hash("Grüßformel"));
        }

        [Fact]
        public void Verify_ReturnsTrueForMatchingPassword()
        {
            var stored = PasswordHasher.Hash("NimbusChat2026!");

            Assert.True(PasswordHasher.Verify("NimbusChat2026!", stored));
        }

        [Fact]
        public void Verify_ReturnsFalseForWrongPassword()
        {
            var stored = PasswordHasher.Hash("NimbusChat2026!");

            Assert.False(PasswordHasher.Verify("nimbuschat2026!", stored));
        }

        [Fact]
        public void Verify_ReturnsFalseForGarbageStoredHash()
        {
            Assert.False(PasswordHasher.Verify("NimbusChat2026!", "not-a-hash"));
        }
    }
}
