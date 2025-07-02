namespace RaceWriterBot.Temp
{
    public class UserSession
    {
        private readonly List<TargetChatSession> _targetChats = [];
        public IReadOnlyList<TargetChatSession> TargetChats { get { return _targetChats.AsReadOnly(); } }
        public long UserChatId { get; set; }

        public void AddTargetChatSession(TargetChatSession chat)
        {
            if (_targetChats.Any(c => c.TargetChatId == chat.TargetChatId))
                throw new InvalidOperationException("Chat already exists");

            _targetChats.Add(chat);
        }

        public void UpdateHashtagTextTemplate(string hashtagName, string newTemplate)
        {
            var chat = _targetChats
                .FirstOrDefault(c => c.Hashtags.Any(h => h.HashtagName == hashtagName));

            var tag = (chat?.Hashtags.FirstOrDefault(h => h.HashtagName == hashtagName)) ?? throw new InvalidOperationException("Hashtag not found");

            tag.TextTemplate = newTemplate;
        }

        public void UpdateHashtagName(string hashtagName, string newHashtagName)
        {
            var chat = _targetChats
                .FirstOrDefault(c => c.Hashtags.Any(h => h.HashtagName == hashtagName));

            var tag = (chat?.Hashtags.FirstOrDefault(h => h.HashtagName == hashtagName)) ?? throw new InvalidOperationException("Hashtag not found");

            tag.HashtagName = newHashtagName;
        }

        public void AddHashtag(long targetChatId, HashtagSession hashtagSession)
        {
            var chat = _targetChats.FirstOrDefault(c => c.TargetChatId == targetChatId) ?? throw new InvalidOperationException("Chat not found");

            chat.AddHashtag(hashtagSession);
        }
    }

    public class TargetChatSession
    {
        private readonly List<HashtagSession> _hashtags = [];
        public long TargetChatId { get; set; }
        public string Name { get; private set; }
        public IReadOnlyList<HashtagSession> Hashtags { get { return _hashtags.AsReadOnly(); } }

        public TargetChatSession(string name, long chatId)
        {
            Name = name;
            TargetChatId = chatId;
        }

        public void AddHashtag(HashtagSession hashtag)
        {
            if (_hashtags.Any(h => h.HashtagName == hashtag.HashtagName))
                throw new InvalidOperationException("Hashtag already exists");

            _hashtags.Add(hashtag);
        }

        public override string ToString()
        {
            return Name;
        }
    }

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

    public class PostMessagePair
    {
        public int OriginalPostId { get; set; }
        public int BotMessageId { get; set; }
    }
}
