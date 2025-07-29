using RaceWriterBot.Models;

namespace RaceWriterBot.Interfaces
{
    public interface IViewManager
    {
        Task Settings(long chatId);
        Task ShowHashtags(long userId, TargetChatSession channel);
        Task ShowMessageDetails(long userId, PostMessagePair pair);
        Task ShowTemplateMessage(long userId, HashtagSession hashtag);
        Task AddBotToTargetChatSettings(long chatId);
        Task RequestForwardedMessage(long chatId);
        Task ReturnToPreviousMenu(long chatId);
        Task ShowErrorMessage(long chatId);
        Task AddNewHashtag(long userId, long channelId);
        Task StartEditHashtagTemplate(long userId, string hashtagName);
    }
}