namespace RaceWriterBot.Temp
{
    public class UserDataStorage : IUserDataStorage
    {
        private Dictionary<long, UserSession> _usersSessions = [];
        public UserDataStorage()
        {
            var defaultUser = new UserSession { UserChatId = 190866300 };
            defaultUser.AddTargetChatSession(new TargetChatSession("Test1", 123));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test2", 124));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test3", 125));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test4", 126));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test5", 127));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test6", 128));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test7", 129));

            _usersSessions.Add(190866300, defaultUser);
        }

        public void AddNewUser(long userId)
        {
            if (!_usersSessions.ContainsKey(userId))
                _usersSessions.Add(userId, new UserSession() { UserChatId = userId });
        }

        public UserSession GetUserSession(long userId)
        {
            return _usersSessions.TryGetValue(userId, out var session)
            ? session
            : throw new InvalidOperationException("User not found");
        }

        public IReadOnlyList<TargetChatSession> GetTargetChatSessions(long userId)
        {
            return GetUserSession(userId).TargetChats;
        }

        public IReadOnlyList<HashtagSession> GetHashtagSessions(long userId, long targetChatId)
        {
            var user = GetUserSession(userId);
            var chat = user.TargetChats.FirstOrDefault(c => c.TargetChatId == targetChatId)
                       ?? throw new InvalidOperationException("Target chat not found");

            return chat.Hashtags;
        }

        public void AddTargetChatSession(long userId, TargetChatSession targetChatSession)
        {
            var user = GetUserSession(userId);
            user.AddTargetChatSession(targetChatSession);
        }

        public void AddHashtagSession(long userId, long targetChatId, HashtagSession hashtag)
        {
            var user = GetUserSession(userId);
            user.AddHashtag(targetChatId, hashtag);
        }

        public void UpdateHashtagTemplate(long userId, string hashtagName, string newTemplate)
        {
            var user = GetUserSession(userId);
            user.UpdateHashtagTextTemplate(hashtagName, newTemplate);
        }

        public void UpdateHashtagName(long userId, string hashtagName, string newName)
        {
            var user = GetUserSession(userId);
            user.UpdateHashtagName(hashtagName, newName);
        }

    }

    public interface IUserDataStorage
    {
        void AddNewUser(long userId);
        UserSession GetUserSession(long userId);
        IReadOnlyList<TargetChatSession> GetTargetChatSessions(long userId);
        IReadOnlyList<HashtagSession> GetHashtagSessions(long userId, long targetChatId);
        void AddTargetChatSession(long userId, TargetChatSession targetChatSession);
        void AddHashtagSession(long userId, long targetChatId, HashtagSession hashtag);
        void UpdateHashtagTemplate(long userId, string hashtagName, string newTemplate);
        void UpdateHashtagName(long userId, string hashtagName, string newName);
    }
}
