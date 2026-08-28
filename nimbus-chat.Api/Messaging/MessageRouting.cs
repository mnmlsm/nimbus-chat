namespace NimbusChat.Api.Messaging
{
    // Decides whether an incoming message belongs to the global chat or to a
    // private 1:1 conversation.
    public static class MessageRouting
    {
        public static bool IsGlobal(int? receiverId) =>
            !receiverId.HasValue || receiverId.Value <= 0;
    }
}
