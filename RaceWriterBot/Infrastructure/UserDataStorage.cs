using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;

namespace RaceWriterBot.Infrastructure
{
    public class UserDataStorage : IUserDataStorage
    {
        private readonly Dictionary<long, User> _users;

        public UserDataStorage()
        {
            _users = new Dictionary<long, User>();
            var defaultUser = new User(190866300);
            defaultUser.AddTargetChatSession(-1002633936370, "Test2");
            //defaultUser.AddTargetChatSession(new TargetChatSession("Test2", 124));
            //defaultUser.AddTargetChatSession(new TargetChatSession("Test3", 125));
            //defaultUser.AddTargetChatSession(new TargetChatSession("Test4", 126));
            //defaultUser.AddTargetChatSession(new TargetChatSession("Test5", 127));
            //defaultUser.AddTargetChatSession(new TargetChatSession("Test6", 128));
            //defaultUser.AddTargetChatSession(new TargetChatSession("Test7", 129));

            _users.Add(190866300, defaultUser);
        }

        public void AddUser(long userId)
        {
            if (!_users.ContainsKey(userId))
            {
                _users.Add(userId, new User(userId));
            }
        }

        public User GetUser(long userId)
        {
            if (_users.TryGetValue(userId, out var user))
            {
                return user;
            }
            
            throw new KeyNotFoundException($"User with ID {userId} is not found");
        }

        public void RemoveUser(long userId)
        {
            if (_users.ContainsKey(userId))
            {
                _users.Remove(userId);
            }
        }
    }
}
