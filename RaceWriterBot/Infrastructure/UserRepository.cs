using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<UserId, User> _users;

        public UserRepository()
        {
            _users = new Dictionary<UserId, User>();
        }

        public void AddUser(User user)
        {
            if (!_users.ContainsValue(user))
            {
                _users[user.UserId] = user;
            }
        }

        public User GetUser(UserId id)
        {
            if (_users.TryGetValue(id, out User? value))
            {
                return value;
            }
            throw new UserNotFoundException();
        }
    }
}
