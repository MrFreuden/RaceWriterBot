using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class Menu
    {
        public string Text;
        public Dictionary<string, string> ButtonsData = [];
        public const int MaxPerRow = 3;
        public const int MaxDataRows = 2;

        public InlineKeyboardMarkup GetMarkup()
        {
            var rows = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();

            foreach (var button in ButtonsData)
            {
                currentRow.Add(InlineKeyboardButton.WithCallbackData(button.Key, button.Value));

                if (currentRow.Count == MaxPerRow)
                {
                    rows.Add(currentRow);
                    currentRow = [];
                }
            }

            if (currentRow.Count > 0)
            {
                rows.Add(currentRow);
            }

            rows.Add([InlineKeyboardButton.WithCallbackData("Назад", $"{Constants.CommandNames.ACTION_BACK}")]);

            return new InlineKeyboardMarkup(rows);
        }

        public Menu AddButton(string text, string callbackData)
        {
            ButtonsData[text] = callbackData;
            return this;
        }
    }

    public class MenuManager
    {
        private readonly IBotMessenger _botMessenger;
        private readonly IUserDataStorage _userDataStorage;

        public MenuManager(IBotMessenger botMessenger, IUserDataStorage userDataStorage)
        {
            _botMessenger = botMessenger;
            _userDataStorage = userDataStorage;
        }

        public async Task<Message> ShowMenu(long userId, Menu menu, int? messageId = null)
        {
            _userDataStorage.AddMenuHistory(userId, menu);

            var markup = menu.GetMarkup();

            if (messageId.HasValue)
                return await _botMessenger.EditMessageText(userId, messageId.Value, menu.Text, markup);
            else
                return await _botMessenger.SendMessage(userId, menu.Text, markup);
        }

        public async Task<Message> ShowPagingMenu<T>(
            long userId, string title, 
            List<T> values, Func<T, string> itemTextSelector, 
            string pageType, int pageSize = 3, int? messageId = null)
        {
            var paging = new Paging<T>(values, itemTextSelector, $"{pageType}_", pageSize);

            _userDataStorage.SavePagination(userId, pageType, paging);

            var markup = paging.GetPageMarkup(0);

            if (messageId.HasValue)
                return await _botMessenger.EditMessageText(userId, messageId.Value, title, markup);
            else
                return await _botMessenger.SendMessage(userId, title, markup);
        }

        public async Task HandlePaginationAction<T>(
            long userId, long chatId, int messageId,
            string pageType, string action, string data,
            Action<T> onItemSelected)
        {
            var paging = _userDataStorage.GetPagination<T>(userId, pageType);
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

        public async Task<Message> NavigateBack(long userId, int messageId)
        {
            var history = _userDataStorage.GetMenuHistory(userId);

            if (history.Count == 0)
            {
                throw new NotImplementedException();
            }
            var menu = history.Pop();
            var markup = menu.GetMarkup();

            return await _botMessenger.EditMessageText(userId, messageId, menu.Text, markup);
        }

        public void ClearHistory()
        {
            
        }
    }
}