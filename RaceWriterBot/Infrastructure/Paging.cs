using RaceWriterBot.Enums;
using RaceWriterBot.Presentation.Handlers;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Infrastructure
{
    public class Paging
    {
        private readonly List<InlineKeyboardButton> _values;
        //private readonly Func<T, string> _itemTextSelector;
        private readonly PageType _callbackPrefix;
        public int PageSize { get; }
        public int TotalPages { get; }

        public Paging(List<InlineKeyboardButton> values, PageType callbackPrefix, int pageSize)
        {
            _values = values;
            _callbackPrefix = callbackPrefix;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)values.Count / pageSize);
        }

        public InlineKeyboardMarkup GetPageMarkup(int pageNumber)
        {
            var keyboard = GetKeyboard(pageNumber, TotalPages);
            return keyboard;
        }

        public InlineKeyboardButton GetItem(string data)
        {
            return _values.FirstOrDefault(i => i.GetHashCode().ToString() == data);
        }

        private List<InlineKeyboardButton> GetPage(int page)
        {
            return _values.Skip(page * PageSize).Take(PageSize).ToList();
        }

        private InlineKeyboardMarkup GetKeyboard(int page, int totalPages)
        {
            var rows = new List<List<InlineKeyboardButton>>();
            var pageItems = GetPage(page);

            var objectButtons = pageItems
                .Select(item => InlineKeyboardButton.WithCallbackData(item.Text, $"{CallbackType.Paging}_{_callbackPrefix}_{PaginationAction.Item}_{item.GetHashCode()}"))
                .ToList();

            rows.Add(pageItems);

            var navButtons = new List<InlineKeyboardButton>();
            if (page > 0)
                navButtons.Add(InlineKeyboardButton.WithCallbackData(Constants.CommandNames.Prev, $"{CallbackType.Paging}_{_callbackPrefix}_{PaginationAction.Page}_{page - 1}"));
            if (page < totalPages - 1)
                navButtons.Add(InlineKeyboardButton.WithCallbackData(Constants.CommandNames.Next, $"{CallbackType.Paging}_{_callbackPrefix}_{PaginationAction.Page}_{page + 1}"));

            if (navButtons.Count != 0)
                rows.Add(navButtons);

            return new InlineKeyboardMarkup(rows);
        }
    }
}
