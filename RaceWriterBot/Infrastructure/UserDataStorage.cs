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
            defaultUser.AddTargetChatSession(-1002633936371, "Test3");
            defaultUser.AddTargetChatSession(-1002633936372, "Test4");
            defaultUser.AddTargetChatSession(-1002633936373, "Test5");
            defaultUser.AddTargetChatSession(-1002633936374, "Test6");
            defaultUser.AddTargetChatSession(-1002633936375, "Test7");
            defaultUser.AddTargetChatSession(-1002633936376, "Test8");
            defaultUser.AddTargetChatSession(-1002633936377, "Test9");
            defaultUser.AddTargetChatSession(-1002633936378, "Test10");

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

            else
            {
                AddUser(userId);
                return _users[userId];
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
