namespace RaceWriterBot.Domain.Exceptions
{
    [Serializable]
    public class DuplicateChatException : Exception
    {
        public DuplicateChatException()
        {
        }

        public DuplicateChatException(string? message) : base(message)
        {
        }

        public DuplicateChatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}