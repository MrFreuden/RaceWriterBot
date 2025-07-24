using RaceWriterBot.Infrastructure;
using RaceWriterBot.Models;

namespace RaceWriterBot.Interfaces
{
    public interface IUserDataStorage
    {
        void AddUser(long userId);
        User GetUser(long userId);
        void RemoveUser(long userId);
    }
}
