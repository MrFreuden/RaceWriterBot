using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Infrastructure
{
    public class UpdateProcessor
    {
        private const int CountObjectsPerPage = 3;
        private readonly IUserDataStorage _userDataStorage;
        private readonly IBotDataStorage _botStorage;
        private readonly IBotMessenger _botMessenger;
        private readonly IDialogProcessor _dialogProcessor;
        private readonly PaginationState _paginationState = new();
        private readonly Dictionary<long, Stack<(string pageType, object context)>> _navigationHistory = new();

        public UpdateProcessor(IBotMessenger botMessenger) : this(
            botMessenger,
            new UserDataStorage(),
            new BotDataStorage(),
            new DialogProcessor())
        { }

        public UpdateProcessor(
            IBotMessenger botMessenger,
            IUserDataStorage userDataStorage,
            IBotDataStorage botDataStorage,
            IDialogProcessor dialogProcessor)
        {
            _userDataStorage = userDataStorage;
            _botStorage = botDataStorage;
            _botMessenger = botMessenger;
            _dialogProcessor = dialogProcessor;
        }

        public Task ProcessMessage(Message message)
        {
            if (IsPrivateMessage(message))
            {
                if (_dialogProcessor.HasActiveDialog(message.From.Id))
                {
                    _dialogProcessor.ProcessDialogMessage(message.From.Id, message);
                    return Task.CompletedTask;
                }
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
                    _userDataStorage.AddUserSession(message.Chat.Id);
                    _botMessenger.SendMessage(message.Chat.Id, "Ласкаво просимо");
                    Settings(message.Chat.Id);
                    break;
                case "/settings":
                    Settings(message.Chat.Id);
                    break;
                default:
                    break;
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


        

        private void HandlePagination(CallbackQuery query, string pageType, string action, string data)
        {
            var userId = query.From.Id;
            var chatId = query.Message.Chat.Id;
            var messageId = query.Message.MessageId;

            switch (pageType)
            {
                case Constants.CommandNames.CHANNELS_PAGE:
                    HandlePaginationAction<TargetChatSession>(
                        userId, chatId, messageId, pageType, action, data,
                        (session) => ShowHashtags(userId, session, messageId));
                    break;

                case Constants.CommandNames.HASHTAGS_PAGE:
                    HandlePaginationAction<HashtagSession>(
                        userId, chatId, messageId, pageType, action, data,
                        (hashtag) => ShowTemplateMessage(userId, hashtag, messageId));
                    break;

                case Constants.CommandNames.MESSAGES_PAGE:
                    HandlePaginationAction<PostMessagePair>(
                        userId, chatId, messageId, pageType, action, data,
                        (pair) => ShowMessageDetails(userId, pair, messageId));
                    break;
            }
        }

        private void AddBotToTargetChatSettings(long chatId)
        {
            _botMessenger.SendMessage(chatId, "Додайте бота в чат обговорень каналу та дайте йому права адміністратора",
                replyMarkup: new InlineKeyboardButton[][]
                    {
                        [("Зроблено", Constants.CommandNames.ACTION_CONFIRMATION_ADDING_BOT)]
                    });
        }

        private void RequestForwardedMessage(long chatId)
        {
            _botMessenger.SendMessage(chatId, "Надішліть будь-яке повідомлення з чату обговорень");
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