using RaceWriterBot.Models;

namespace RaceWriterBot.Interfaces
{
    public interface IViewManager
    {
        void Settings(long chatId);
        void ShowHashtags(long userId, TargetChatSession channel, int messageId);
        void ShowMessageDetails(long userId, PostMessagePair pair, int messageId);
        void ShowTemplateMessage(long userId, HashtagSession hashtag, int messageId);
    }
}