using Telegram.Bot.Types;

namespace RaceWriterBot.Interfaces
{
    public interface IDialogProcessor
    {
        bool ProcessDialogMessage(long userId, Message message);
        bool HasActiveDialog(long userId);
    }
}