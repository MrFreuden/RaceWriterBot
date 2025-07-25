using RaceWriterBot.Models;

namespace RaceWriterBot.Interfaces
{
    public interface IViewManager
    {
        void Settings(long chatId);
        void ShowHashtags(long userId, TargetChatSession channel);
        void ShowMessageDetails(long userId, PostMessagePair pair);
        void ShowTemplateMessage(long userId, HashtagSession hashtag);
        void AddBotToTargetChatSettings(long chatId);
        void RequestForwardedMessage(long chatId);
        void ReturnToPreviousMenu(long chatId);
        void ShowErrorMessage(long chatId);
    }
}