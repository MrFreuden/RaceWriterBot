using RaceWriterBot.Managers;
using RaceWriterBot.Models;

namespace RaceWriterBot.Interfaces
{
    public interface IUserDataStorage
    {
        void AddMenuHistory(long userId, Menu menu);
        void AddUserDialog(long userId, IDialogState dialogState);
        void AddUserSession(long userId);
        Stack<Menu> GetMenuHistory(long userId);
        IDialogState GetUserDialog(long userId);
        UserSession GetUserSession(long userId);
    }
}