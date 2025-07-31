namespace RaceWriterBot.Domain.Exceptions
{
    [Serializable]
    public class ChatNotFoundException : Exception
    {
        public ChatNotFoundException()
        {
        }

        public ChatNotFoundException(string? message) : base(message)
        {
        }

        public ChatNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}