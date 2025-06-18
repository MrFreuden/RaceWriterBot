using RaceWriterBot.asdfadgfh;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Temp
{
    public class UpdateProcessor
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserDataStorage _dataStorage;
        private readonly BotDataStorage _botStorage;
        private readonly IBotMessenger _botMessenger;

        public UpdateProcessor(ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _dataStorage = new UserDataStorage();
            _botStorage = new BotDataStorage();
            _botMessenger = new BotMessenger(botClient);
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
                case "/start": _dataStorage.AddNewUser(message.Chat.Id);
                    break;
                case "/settings": Settings(message.Chat.Id);
                    
                    break;
                default:
                    break;
            }
        }

        private void Settings(long chatId)
        {
            var targetChatSession = _dataStorage.GetTargetChatSessions(chatId);
            if (targetChatSession.Count == 0)
            {
                _botMessenger.SendMessage(chatId, "У вас немає активних каналів", 
                    replyMarkup: new InlineKeyboardButton[][]
                    {
                        [("Створити", "CreateTargetChatSession")]
                    });
            }
            else
            {
                _botMessenger.SendMessage(chatId, "Активні канали",
                    replyMarkup: new InlineKeyboardButton[][]
                    {
                        [(_dataStorage.GetTargetChatSessions(chatId).ToString(), "1")]
                    });
            }
        }

        private void RegisterNewTargetChat(Message message)
        {
            _dataStorage.AddTargetChatSession(message.From.Id, new TargetChatSession(message.ForwardFromChat.Title, message.ForwardFromChat.Id));
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
            switch (query.Data)
            {
                case "CreateTargetChatSession": AddBotToTargetChatSettings(query.From.Id);
                    break;
                case "UserConfirmAddingBotToTargetChat": RequestForwardedMessage(query.From.Id);
                    break;
                case "2":
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

            foreach (Match match in matches)
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

        //private List<InlineKeyboardButton[]> GetButton<T>(List<T> list, int countPerLine)
        //{
        //    var count = Math.Floor((decimal)list.Count / countPerLine);
        //    var result = new List<InlineKeyboardButton>();
        //    for (int i = 0; i < count; i++)
        //    {

        //    }
        //    var keyboard = new InlineKeyboardButton[countPerLine + 2];


        //}



        //public Task ProcessPost(Message post)
        //{

        //}

        //public Task ProcessEditMessage(Message editMessage)
        //{

        //}
    }
}