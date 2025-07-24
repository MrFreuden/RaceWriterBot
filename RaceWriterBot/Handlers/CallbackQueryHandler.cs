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
                StartEditHashtagTemplate(query.From.Id, hashtagName, query.Message.MessageId);
                return Task.CompletedTask;
            }

            if (query.Data.StartsWith(Constants.CommandNames.ACTION_ADD_HASHTAG))
            {
                if (int.TryParse(query.Data.Split("_").Last(), out var channelHash))
                {
                    AddNewHashtag(query.From.Id, channelHash, query.Message.MessageId);
                }
                return Task.CompletedTask;
            }

            var segments = query.Data.Split('_', 3);
            if (segments.Length >= 2)
            {
                var pageType = segments[0];
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
                case "3":
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

        private void HandlePagination(CallbackQuery query, string pageType, string action, string data)
        {
            var userId = query.From.Id;
            var chatId = query.Message.Chat.Id;
            var messageId = query.Message.MessageId;

            switch (pageType)
            {
                case Constants.CommandNames.CHANNELS_PAGE:
                    _menuManager.HandlePaginationAction<TargetChatSession>(
                        userId, chatId, messageId, pageType, action, data,
                        (session) => _viewManager.ShowHashtags(userId, session, messageId));
                    break;

                case Constants.CommandNames.HASHTAGS_PAGE:
                    _menuManager.HandlePaginationAction<HashtagSession>(
                        userId, chatId, messageId, pageType, action, data,
                        (hashtag) => _viewManager.ShowTemplateMessage(userId, hashtag, messageId));
                    break;

                case Constants.CommandNames.MESSAGES_PAGE:
                    _menuManager.HandlePaginationAction<PostMessagePair>(
                        userId, chatId, messageId, pageType, action, data,
                        (pair) => _viewManager.ShowMessageDetails(userId, pair, messageId));
                    break;
            }
        }

        private void AddNewHashtag(long userId, int channelHash, int messageId)
        {
            var userSession = _userDataStorage.GetUserSession(userId);
            var channelSession = _userDataStorage.GetTargetChatSessions(userId)
                .Where(s => s.GetHashCode() == channelHash)
                .FirstOrDefault();

            if (channelSession != null)
            {
                _userDataStorage.SetExpectedAction(userId, Constants.CommandNames.ACTION_ADD_HASHTAG, channelSession);

                _botMessenger.SendMessage(
                    userId,
                    "Введіть новий хештег");
            }
        }

        private void StartEditHashtagTemplate(long userId, string hashtagName, int messageId)
        {
            var hashtag = _userDataStorage.GetHashtagSession(userId, hashtagName);
            if (hashtag == null)
            {
                _botMessenger.SendMessage(
                    userId,
                    "Хештег не знайдено або у вас немає прав для його редагування.");
                return;
            }

            _userDataStorage.SetExpectedAction(userId, Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE, hashtag);

            _botMessenger.SendMessage(
                userId,
                "Будь ласка, введіть новий текст шаблону для хештега #" + hashtagName);
        }
    }
}