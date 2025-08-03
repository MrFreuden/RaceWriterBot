using RaceWriterBot.Domain.Models.Old;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class MenuManager
    {
        private readonly IMessageSender _botMessenger;
        private readonly IUserDataStorage _userDataStorage;
        private const int _countObjectsPerPage = 3;


        public MenuManager(IMessageSender botMessenger, IUserDataStorage userDataStorage)
        {
            _botMessenger = botMessenger;
            _userDataStorage = userDataStorage;
        }

        private async Task<Message> SendAndRemember(long userId, string text, InlineKeyboardMarkup markup)
        {
            var user = _userDataStorage.GetUser(userId);
            var messageId = user.LastMessageIdFromBot;
            Message message;
            if (messageId == 0)
            {
                message = await _botMessenger.SendMessage(userId, text, markup);
                user.LastMessageIdFromBot = message.Id;
            }
            else
            {
                message = await _botMessenger.EditMessageText(userId, messageId, text, markup);
            }

            return message;
        }

        public async Task<Message> ShowMenu(long userId, Menu menu)
        {
            if (!menu.IsInputAwaiting)
            {
                _userDataStorage.GetUser(userId).SetCurrentMenu(menu);
            }
            if (menu.ButtonsData.Count > _countObjectsPerPage)
            {
                return await ShowPagingMenu(userId, menu);
            }
            else
            {
                var markup = menu.GetMarkup();

                return await SendAndRemember(userId, menu.Text, markup);
            }
        }

        private async Task<Message> ShowPagingMenu(long userId, Menu menu)
        {
            var buttonItems = menu.ButtonsData
                .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value))
                .ToList();

            var buttons = buttonItems.Select(b => new InlineKeyboardButton(b.Key, b.Value)).ToList();

            menu.Paging = new Paging(buttons, menu.PageType.Value, _countObjectsPerPage);

            var markup = menu.Paging.GetPageMarkup(0);

            return await SendAndRemember(userId, menu.Text, markup);
        }

        public async Task HandleActionItem<T>(long userId, T item, Action<T> onItemSelected)
        {
            if (item != null)
            {
                onItemSelected(item);
            }
        }

        public async Task HandleActionPage(long userId, int pageNumber)
        {
            var paging = _userDataStorage.GetUser(userId).GetPagination();
            if (paging == null) return;

            var markup = paging.GetPageMarkup(pageNumber);

            var messageId = _userDataStorage.GetUser(userId).LastMessageIdFromBot;

            await _botMessenger.EditMessageReplyMarkup(userId, messageId, markup);
        }

        
        public async Task<Message> NavigateBack(long userId)
        {
            var lastMenu = _userDataStorage.GetUser(userId).GetLastMenu();

            return await ShowMenu(userId, lastMenu);
        }

        public void ClearHistory()
        {
            throw new NotImplementedException();
        }
    }
}