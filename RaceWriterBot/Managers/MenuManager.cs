using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class MenuManager
    {
        private readonly IBotMessenger _botMessenger;
        private readonly IUserDataStorage _userDataStorage;

        public MenuManager(IBotMessenger botMessenger, IUserDataStorage userDataStorage)
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
            _userDataStorage.GetUser(userId).AddMenuHistory(menu);

            var markup = menu.GetMarkup();

            return await SendAndRemember(userId, menu.Text, markup);
        }

        public async Task<Message> ShowPagingMenu<T>(
            long userId, string title,
            List<T> values, Func<T, string> itemTextSelector,
            string pageType, int pageSize = 3, int? messageId = null)
        {
            var paging = new Paging<T>(values, itemTextSelector, $"{pageType}_", pageSize);

            _userDataStorage.GetUser(userId).SavePagination(pageType, paging);

            var markup = paging.GetPageMarkup(0);

            return await SendAndRemember(userId, title, markup);
        }

        public async Task HandlePaginationAction<T>(
            long userId, long chatId, int messageId,
            string pageType, string action, string data,
            Action<T> onItemSelected)
        {
            var paging = _userDataStorage.GetUser(userId).GetPagination<T>(pageType);
            if (paging == null) return;

            switch (action)
            {
                case "page":
                    if (int.TryParse(data, out var pageNumber))
                    {
                        var markup = paging.GetPageMarkup(pageNumber);
                        await _botMessenger.EditMessageReplyMarkup(chatId, messageId, markup);
                    }
                    break;

                case "item":
                    var selectedItem = paging.GetItem(data);
                    if (selectedItem != null)
                    {
                        onItemSelected(selectedItem);
                    }
                    break;

                case "back":
                    await NavigateBack(userId, messageId);
                    break;
            }
        }

        public async Task<Message> NavigateBack(long userId, int? messageId = null)
        {
            var history = _userDataStorage.GetUser(userId).GetMenuHistory();

            if (history.Count == 0)
            {
                throw new NotImplementedException();
            }
            var menu = history.Pop();
            var markup = menu.GetMarkup();

            return await SendAndRemember(userId, menu.Text, markup);
        }

        public void ClearHistory()
        {
            throw new NotImplementedException();
        }
    }
}