using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using Telegram.Bot.Types;

namespace RaceWriterBot.Handlers
{
    public class CallbackQueryHandler
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly MenuManager _menuManager;

        public CallbackQueryHandler(IUserDataStorage userDataStorage)
        {
            _userDataStorage = userDataStorage;
        }

        public Task ProcessCallbackQuery(CallbackQuery query)
        {
            if (query.Data.StartsWith("EditTemplateMessageText_"))
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
                    AddBotToTargetChatSettings(query.From.Id);
                    break;
                case Constants.CommandNames.ACTION_CONFIRMATION_ADDING_BOT:
                    RequestForwardedMessage(query.From.Id);
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

        private void AddNewHashtag(long userId, int channelHash, int messageId)
        {
            var userSession = _userDataStorage.GetUserSession(userId);
            var channelSession = _userDataStorage.GetTargetChatSessions(userId)
                .Where(s => s.GetHashCode() == channelHash)
                .FirstOrDefault();

            if (channelSession != null)
            {
                _dialogManager.SetExpectedAction(userId, Constants.CommandNames.ACTION_ADD_HASHTAG, channelSession);

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

            _dialogManager.SetExpectedAction(userId, Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE, hashtag);

            _botMessenger.SendMessage(
                userId,
                "Будь ласка, введіть новий текст шаблону для хештега #" + hashtagName);
        }
    }
}