using RaceWriterBot.Domain.Models.Old;
using RaceWriterBot.Interfaces;

namespace RaceWriterBot.Infrastructure
{
    public class UserDataStorage : IUserDataStorage
    {
        private readonly Dictionary<long, User3> _users;

        public UserDataStorage()
        {
            _users = new Dictionary<long, User3>();
            var defaultUser = new User3(190866300);
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
                _users.Add(userId, new User3(userId));
            }
        }

        public User3 GetUser(long userId)
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

        public bool TryGetUser(long userId, out User3? user)
        {
            if (userId == 0)
            {
                user = default;
                return false;
            }

            if (_users.TryGetValue(userId, out var user1))
            {
                user = user1;
                return true;
            }

            user = default;
            return false;
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
