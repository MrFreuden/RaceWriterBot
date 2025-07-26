using RaceWriterBot.Enums;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using RaceWriterBot.Models;
using Telegram.Bot.Types;

namespace RaceWriterBot.Handlers
{
    public class CallbackQueryHandler
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly IBotDataStorage _botStorage;
        private readonly IBotMessenger _botMessenger;
        private readonly IDialogProcessor _dialogProcessor;
        private readonly IViewManager _viewManager;
        private readonly MenuManager _menuManager;

        public CallbackQueryHandler(IUserDataStorage userDataStorage, IBotDataStorage botStorage, IBotMessenger botMessenger, IDialogProcessor dialogProcessor, IViewManager viewManager, MenuManager menuManager)
        {
            _userDataStorage = userDataStorage;
            _botStorage = botStorage;
            _botMessenger = botMessenger;
            _dialogProcessor = dialogProcessor;
            _viewManager = viewManager;
            _menuManager = menuManager;
        }

        public Task ProcessCallbackQuery(CallbackQuery query)
        {
            if (query.Data.StartsWith(Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE))
            {
                var hashtagName = query.Data.Split('_').Last();
                _viewManager.StartEditHashtagTemplate(query.From.Id, hashtagName);
                return Task.CompletedTask;
            }

            if (query.Data.StartsWith(Constants.CommandNames.ACTION_ADD_HASHTAG))
            {
                if (int.TryParse(query.Data.Split("_").Last(), out var channelHash))
                {
                    _viewManager.AddNewHashtag(query.From.Id, channelHash);
                }
                return Task.CompletedTask;
            }

            var segments = query.Data.Split('_', 3);
            if (segments.Length >= 2)
            {
                var pageType = (PageType)Enum.Parse(typeof(PageType), segments[0]);
                var action = segments[1];
                HandlePagination(query, pageType, action, segments.Length > 2 ? segments[2] : null);
                return Task.CompletedTask;
            }
            switch (query.Data)
            {
                case Constants.CommandNames.ACTION_CREATE_TARGET_CHAT:
                    _viewManager.AddBotToTargetChatSettings(query.From.Id);
                    break;
                case Constants.CommandNames.ACTION_CONFIRMATION_ADDING_BOT:
                    _viewManager.RequestForwardedMessage(query.From.Id);
                    break;
                case Constants.CommandNames.ACTION_BACK:
                    _menuManager.NavigateBack(query.From.Id);
                    break;
                case "4":
                    break;
                case "":
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        private async Task HandlePagination(CallbackQuery query, PageType pageType, string action, string data)
        {
            var userId = query.From.Id;
            var chatId = query.Message.Chat.Id;

            switch (pageType)
            {
                case PageType.Channels:
                    await _menuManager.HandlePaginationAction<TargetChatSession>(
                        userId, chatId, pageType, action, data,
                        (session) => _viewManager.ShowHashtags(userId, session));
                    break;

                case PageType.Hashtags:
                    await _menuManager.HandlePaginationAction<HashtagSession>(
                        userId, chatId, pageType, action, data,
                        (hashtag) => _viewManager.ShowTemplateMessage(userId, hashtag));
                    break;

                case PageType.Messages:
                    await _menuManager.HandlePaginationAction<PostMessagePair>(
                        userId, chatId, pageType, action, data,
                        (pair) => _viewManager.ShowMessageDetails(userId, pair));
                    break;
            }
        } 
    }
}