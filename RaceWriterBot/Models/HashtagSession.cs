namespace RaceWriterBot.Models
{
    public class HashtagSession
    {
        private const string DefaultTextTemplate = "18:20 - 0 вільних місць\r\n18:35 - 0 вільних місць\r\n18:55 - 0 вільних місць";
        private readonly List<PostMessagePair> _messages = [];
        public string HashtagName { get; set; }
        public string TextTemplate { get; set; }
        public IReadOnlyList<PostMessagePair> Messages { get { return _messages.AsReadOnly(); } }

        public HashtagSession()
        {
            TextTemplate = DefaultTextTemplate;
        }

        public void AddPostMessagePair(PostMessagePair postMessagePair)
        {
            if (_messages.Any(m => m.BotMessageId == postMessagePair.BotMessageId))
                throw new InvalidOperationException("PostMessagePair already exists");

            _messages.Add(postMessagePair);
        }

        public override string ToString()
        {
            return HashtagName;
        }
    }
}
