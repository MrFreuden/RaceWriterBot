using RaceWriterBot.Domain.Models.Old;
using RaceWriterBot.Infrastructure;

namespace RaceWriterBot.Interfaces
{
    public interface IUserDataStorage
    {
        void AddUser(long userId);
        User3 GetUser(long userId);
        void RemoveUser(long userId);
        bool TryGetUser(long userId, out User3? user);
    }
}
