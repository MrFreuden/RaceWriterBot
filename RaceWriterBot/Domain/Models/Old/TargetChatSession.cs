namespace RaceWriterBot.Domain.Models.Old
{
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
}
