using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using RaceWriterBot.Models;
using Telegram.Bot.Types;

namespace RaceWriterBot.Handlers
{
    public class MessageHandler
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly IBotDataStorage _botStorage;
        private readonly IBotMessenger _botMessenger;
        private readonly IDialogProcessor _dialogProcessor;
        private readonly IViewManager _viewManager;

        public MessageHandler(
            IBotMessenger botMessenger,
            IUserDataStorage userDataStorage,
            IBotDataStorage botDataStorage, IViewManager viewManager)
        {
            _userDataStorage = userDataStorage;
            _botStorage = botDataStorage;
            _botMessenger = botMessenger;
            _dialogProcessor = new DialogProcessor(userDataStorage);
            _viewManager = viewManager;
        }

        public Task ProcessMessage(Message message)
        {
            var userId = message.From.Id;
            if (IsPrivateMessage(message))
            {
                if (_dialogProcessor.HasActiveDialog(userId))
                {
                    if (_dialogProcessor.ProcessDialogMessage(userId, message))
                    {
                        _viewManager.ReturnToPreviousMenu(userId);
                    }
                    else
                    {
                        _viewManager.ShowErrorMessage(userId);
                    }
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
                throw new NotImplementedException();
                //var parsed = ParseMessage(message);
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
                    _userDataStorage.AddUser(message.Chat.Id);
                    _botMessenger.SendMessage(message.Chat.Id, "Ласкаво просимо");
                    _viewManager.Settings(message.Chat.Id);
                    break;
                case "/settings":
                    _viewManager.Settings(message.Chat.Id);
                    break;
                default:
                    break;
            }
        }


        private void RegisterNewTargetChat(Message message)
        {
            _userDataStorage.GetUser(message.From.Id).AddTargetChatSession(message.ForwardFromChat.Id, message.ForwardFromChat.Title);
        }



        private void CheckAccess(Message forwardedMessage)
        {
            if (_botStorage.AddOwner(forwardedMessage.From.Id, forwardedMessage.ForwardFromChat.Id))
            {
                RegisterNewTargetChat(forwardedMessage);
                _botMessenger.SendMessage(forwardedMessage.From.Id, "Успiх");
                _viewManager.Settings(forwardedMessage.From.Id);
            }
            else
            {
                _botMessenger.SendMessage(forwardedMessage.From.Id, "Помилка");
            }

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