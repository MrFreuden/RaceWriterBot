namespace RaceWriterBot.Temp
{
    public class UserDataStorage : IUserDataStorage
    {
        private Dictionary<long, UserSession> _usersSessions = new Dictionary<long, UserSession>();
        public UserDataStorage()
        {
            _usersSessions.Add(190866300, 
                new UserSession
                {
                    UserChatId = 190866300,
                    TargetChats = new List<TargetChatSession>
                    {
                        new TargetChatSession("Test1", 123),
                        new TargetChatSession("Test2", 124),
                        new TargetChatSession("Test3", 125),
                        new TargetChatSession("Test4", 126),
                        new TargetChatSession("Test5", 127),
                        new TargetChatSession("Test6", 128),
                        new TargetChatSession("Test7", 129)
                    }
                });
        }

        public void AddNewUser(long userId)
        {
            if (!_usersSessions.ContainsKey(userId))
            {
                _usersSessions.Add(userId, new UserSession());
            }
        }

        public UserSession GetUserSession(long userId)
        {
            return _usersSessions[userId];
        }

        public IReadOnlyCollection<TargetChatSession> GetTargetChatSessions(long userId)
        {
            return _usersSessions[userId].TargetChats;
        }

        public IReadOnlyCollection<HashtagSession> GetHashtagSessions(TargetChatSession targetChatSession)
        {
            return targetChatSession.Hashtags;
        }

        public void AddTargetChatSession(long userId, TargetChatSession targetChatSession)
        {
            _usersSessions[userId].TargetChats.Add(targetChatSession);
        }

        public void UpdateHashtagTemplate(long userId, HashtagSession hashtag)
        {
            var targetChat = _usersSessions[userId].TargetChats
                .FirstOrDefault(chat => chat.Hashtags
                    .Any(h => h.Hashtag == hashtag.Hashtag));

            if (targetChat != null)
            {
                var oldHashtag = targetChat.Hashtags
                    .FirstOrDefault(h => h.Hashtag == hashtag.Hashtag);

                if (oldHashtag != null)
                {
                    oldHashtag.TextTemplate = hashtag.TextTemplate;
                }
            }
        }

    }

    public interface IUserDataStorage
    {
        void UpdateHashtagTemplate(long userId, HashtagSession hashtag);
        void AddNewUser(long userId);
        UserSession GetUserSession(long userId);
        IReadOnlyCollection<TargetChatSession> GetTargetChatSessions(long userId);
        void AddTargetChatSession(long userId, TargetChatSession targetChatSession);
        IReadOnlyCollection<HashtagSession> GetHashtagSessions(TargetChatSession targetChatSession);
    }
}
