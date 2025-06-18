namespace RaceWriterBot.Temp
{
    public class UserSession
    {
        public long UserChatId { get; set; }
        public List<TargetChatSession> TargetChats { get; set; } = new();
    }
    public class TargetChatSession
    {
        public long TargetChatId { get; set; }
        public List<HashtagSession> Hashtags { get; set; } = new();
        public string Name { get; private set; }

        public TargetChatSession(string name, long chatId)
        {
            Name = name;
            TargetChatId = chatId;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class HashtagSession
    {
        public string Hashtag { get; set; }
        public string TextTemplate { get; set; }
        public List<PostMessagePair> Messages { get; set; } = new();
    }

    public class PostMessagePair
    {
        public int OriginalPostId { get; set; }
        public int BotMessageId { get; set; }
    }
}
