namespace RaceWriterBot.Temp
{
    public class UserDataStorage
    {
        private Dictionary<long, UserSession> _usersSessions = new Dictionary<long, UserSession>();

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

        }
    }
}
