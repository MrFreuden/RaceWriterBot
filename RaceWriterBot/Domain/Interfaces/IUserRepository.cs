using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Interfaces
{
    public interface IUserRepository
    {
        User GetUser(UserId id);
        void AddUser(User user);
    }
}
