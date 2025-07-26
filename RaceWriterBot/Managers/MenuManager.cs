using RaceWriterBot.Enums;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using System.Net.NetworkInformation;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class MenuManager
    {
        private readonly IBotMessenger _botMessenger;
        private readonly IUserDataStorage _userDataStorage;
        private const int _countObjectsPerPage = 3;

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
            var pageType = menu.PageType.Value.ToString();

            var paging = new Paging<KeyValuePair<string, string>>(
                buttonItems, 
                item => item.Key,
                $"{pageType}_", 
                _countObjectsPerPage);

            _userDataStorage.GetUser(userId).SavePagination(pageType, paging);

            var markup = paging.GetPageMarkup(0);

            return await SendAndRemember(userId, menu.Text, markup);
        }

        public async Task HandlePaginationAction<T>(
            long userId, 
            long chatId,
            PageType pageType, 
            string action, 
            string data,
            Action<T> onItemSelected)
        {
            var paging = _userDataStorage.GetUser(userId).GetPagination<KeyValuePair<string, string>>(pageType.ToString());
            if (paging == null) return;

            switch (action)
            {
                case "page":
                    if (int.TryParse(data, out var pageNumber))
                    {
                        var markup = paging.GetPageMarkup(pageNumber);
                        var messageId = _userDataStorage.GetUser(userId).LastMessageIdFromBot;
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
                    await NavigateBack(userId);
                    break;
            }
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