namespace RaceWriterBot.Domain.Models.Old
{
    public class UserSession
    {
        private readonly List<TargetChatSession> _targetChats = [];
        public IReadOnlyList<TargetChatSession> TargetChats { get { return _targetChats.AsReadOnly(); } }
        public long UserChatId { get; set; }

        public UserSession(long userChatId)
        {
            UserChatId = userChatId;
        }

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
}
