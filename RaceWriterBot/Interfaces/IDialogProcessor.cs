using Telegram.Bot.Types;

namespace RaceWriterBot.Interfaces
{
    public interface IDialogProcessor
    {
        void ProcessDialogMessage(long userId, Message message);
        void ProcessHashtagAdd(Message message);
        void ProcessHashtagEdit(Message message);
        void ProcessHashtagTemplateEdit(Message message);
        bool HasActiveDialog(long userId);
    }
}