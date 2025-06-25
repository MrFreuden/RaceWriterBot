using Moq;
using RaceWriterBot.asdfadgfh;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Temp
{
    public partial class UpdateProcessor
    {
        private const int CountObjectsPerPage = 3;
        private const string CHANNELS_PAGE = "channels";
        private const string HASHTAGS_PAGE = "hashtags";
        private const string MESSAGES_PAGE = "messages";

        private readonly IUserDataStorage _userDataStorage;
        private readonly IBotDataStorage _botStorage;
        private readonly IBotMessenger _botMessenger;
        private readonly PaginationState _paginationState = new();
        
        public UpdateProcessor(IBotMessenger botMessenger) : this(
            botMessenger, 
            new UserDataStorage(), 
            new BotDataStorage()) { }
        
        public UpdateProcessor(
            IBotMessenger botMessenger, 
            IUserDataStorage userDataStorage, 
            IBotDataStorage botDataStorage)
        {
            _userDataStorage = userDataStorage;
            _botStorage = botDataStorage;
            _botMessenger = botMessenger;
        }

        public Task ProcessMessage(Message message)
        {
            if (IsPrivateMessage(message))
            {
                if (IsForwardedMessage(message))
                {
                    CheckAccess(message);
                    return Task.CompletedTask;
                }
                ProcessPrivateMessage(message);
                return Task.CompletedTask;
            }
            if (IsReplyToPostMessage(message))
            {
                var parsed = ParseMessage(message);
            }
            return Task.CompletedTask;
        }

        private bool IsPrivateMessage(Message message)
        {
            return message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private;
        }

        private bool IsForwardedMessage(Message message)
        {
            return message.ForwardFromChat != null;
        }

        private bool IsReplyToPostMessage(Message message)
        {
            return message.ReplyToMessage != null;
        }

        

        private void ProcessPrivateMessage(Message message)
        {
            switch (message.Text)
            {
                case "/start": 
                    _userDataStorage.AddNewUser(message.Chat.Id);
                    _botMessenger.SendMessage(message.Chat.Id, "Ласкаво просимо");
                    break;
                case "/settings": Settings(message.Chat.Id);
                    
                    break;
                default:
                    break;
            }
        }

        private void Settings(long chatId)
        {
            var targetChatSessions = _userDataStorage.GetTargetChatSessions(chatId);
            if (targetChatSessions != null)
            {
                if (targetChatSessions.Count == 0)
                {
                    _botMessenger.SendMessage(chatId, "У вас немає активних каналів",
                        replyMarkup: new InlineKeyboardButton[][]
                        {
                            [("Створити", "CreateTargetChatSession")]
                        });
                }
                else
                {
                    var paging = new Paging<TargetChatSession>(
                        targetChatSessions.ToList(),
                        session => session.Name,
                        $"{CHANNELS_PAGE}_",
                        CountObjectsPerPage);

                    _paginationState.SavePagination(chatId, CHANNELS_PAGE, paging);

                    var keyboard = paging.GetPageMarkup(0);
                    _botMessenger.SendMessage(chatId, "Активні канали", keyboard);
                }
            }
        }

        private void RegisterNewTargetChat(Message message)
        {
            _userDataStorage.AddTargetChatSession(message.From.Id, new TargetChatSession(message.ForwardFromChat.Title, message.ForwardFromChat.Id));
        }

        

        private void CheckAccess(Message forwardedMessage)
        {
            if (_botStorage.AddOwner(forwardedMessage.From.Id, forwardedMessage.Chat.Id))
            {
                RegisterNewTargetChat(forwardedMessage);
                _botMessenger.SendMessage(forwardedMessage.From.Id, "Успiх");
                Settings(forwardedMessage.From.Id);
            }
            else
            {
                _botMessenger.SendMessage(forwardedMessage.From.Id, "Помилка");
            }
           
        }

        
        public Task ProcessCallbackQuery(CallbackQuery query)
        {
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
                case "CreateTargetChatSession": AddBotToTargetChatSettings(query.From.Id);
                    break;
                case "UserConfirmAddingBotToTargetChat": RequestForwardedMessage(query.From.Id);
                    break;
                //case "PickedChannel": ShowHashTags(query.From.Id, query);
                //    break;
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
                case CHANNELS_PAGE:
                    HandlePaginationAction<TargetChatSession>(
                        userId, chatId, messageId, pageType, action, data,
                        (session) => ShowHashtags(userId, session, messageId));
                    break;

                case HASHTAGS_PAGE:
                    HandlePaginationAction<HashtagSession>(
                        userId, chatId, messageId, pageType, action, data,
                        (hashtag) => ShowMessages(userId, hashtag, messageId));
                    break;

                case MESSAGES_PAGE:
                    HandlePaginationAction<PostMessagePair>(
                        userId, chatId, messageId, pageType, action, data,
                        (pair) => ShowMessageDetails(userId, pair, messageId));
                    break;
            }
        }

        private void ShowMessageDetails(long userId, PostMessagePair pair, int messageId)
        {
            throw new NotImplementedException();
        }

        private void ShowMessages(long userId, HashtagSession hashtag, int messageId)
        {
            throw new NotImplementedException();
        }

        private void ShowHashtags(long userId, TargetChatSession channel, int messageId)
        {
            var hashtags = _userDataStorage.GetHashtagSessions(channel);

            if (hashtags == null || hashtags.Count == 0)
            {
                _botMessenger.EditMessageText(
                    userId,
                    messageId,
                    $"Канал {channel.Name} не має хештегів");
                //create
            }

            var paging = new Paging<HashtagSession>(
                hashtags.ToList(),
                hashtag => hashtag.Hashtag,
                $"{HASHTAGS_PAGE}_",
                CountObjectsPerPage);

            _paginationState.SavePagination(userId, HASHTAGS_PAGE, paging);

            var markup = paging.GetPageMarkup(0);
            _botMessenger.EditMessageText(userId, messageId, $"Хештеги для каналу {channel.Name}:", markup);
        }

        private void HandlePaginationAction<T>(
            long userId, long chatId, int messageId, 
            string pageType, string action, string data, 
            Action<T> onItemSelected)
        {
            var paging = _paginationState.GetPagination<T>(userId, pageType);
            if (paging == null) return;

            switch (action)
            {
                case "page":
                    if (Int32.TryParse(data, out var pageNumber))
                    {
                        var markup = paging.GetPageMarkup(pageNumber);
                        _botMessenger.EditMessageReplyMarkup(chatId, messageId, markup);
                    }
                    break;

                case "item":
                    var selectedItem = paging.GetItem(data);
                    if (selectedItem != null)
                    {
                        onItemSelected(selectedItem);
                    }
                    break;
            }
        }

        private void AddBotToTargetChatSettings(long chatId)
        {
            _botMessenger.SendMessage(chatId, "Додайте бота в чат обговорень каналу та дайте йому права адміністратора",
                replyMarkup: new InlineKeyboardButton[][]
                    {
                        [("Зроблено", "UserConfirmAddingBotToTargetChat")]
                    });
        }

        private void RequestForwardedMessage(long chatId)
        {
            _botMessenger.SendMessage(chatId, "Надішліть будь-яке повідомлення з чату обговорень");
        }


        private async Task ParseMessage(Message message)
        {
            throw new NotImplementedException();
        }

        private (string Name, List<string> Slots)? ParseSmart(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            // Ищем все слоты — время или число
            var matches = Regex.Matches(input, @"\b(\d{1,2}:\d{2}|\d{1,2})\b");

            if (matches.Count == 0) return null;

            // Считаем, что имя — всё до первого совпадения
            var firstSlotIndex = matches[0].Index;
            var name = input.Substring(0, firstSlotIndex).Trim();
            if (string.IsNullOrWhiteSpace(name)) return null;

            var slots = new List<string>();

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var value = match.Value;
                if (TimeOnly.TryParse(value, out var time))
                    slots.Add(time.ToString("HH:mm"));
                else if (int.TryParse(value, out var numSlot))
                    slots.Add($"Slot #{numSlot}");
            }

            return (name, slots);
        }

        public async Task ProcessChatMember(ChatMemberUpdated myChatMember)
        {
            if (myChatMember.NewChatMember.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Administrator)
            {
                _botStorage.AddTargetChatId(myChatMember.Chat.Id);
            }
        }
    }
}