namespace RaceWriterBot.Domain.Exceptions
{
    [Serializable]
    public class DublicatHashtagException : Exception
    {
        public DublicatHashtagException()
        {
        }

        public DublicatHashtagException(string? message) : base(message)
        {
        }

        public DublicatHashtagException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}