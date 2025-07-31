namespace RaceWriterBot.Domain.Exceptions
{
    [Serializable]
    internal class HashtagNotFoundException : Exception
    {
        public HashtagNotFoundException()
        {
        }

        public HashtagNotFoundException(string? message) : base(message)
        {
        }

        public HashtagNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}