using NimbusChat.Api.Messaging;
using Xunit;

namespace NimbusChat.Api.Tests
{
    public class MessageRoutingTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void IsGlobal_ReturnsTrue_WhenReceiverIsMissingOrNonPositive(int? receiverId)
        {
            Assert.True(MessageRouting.IsGlobal(receiverId));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(int.MaxValue)]
        public void IsGlobal_ReturnsFalse_ForRealReceiverIds(int? receiverId)
        {
            Assert.False(MessageRouting.IsGlobal(receiverId));
        }
    }
}
