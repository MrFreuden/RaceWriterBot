using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Temp
{
    public class Paging<T>
    {
        
        private readonly List<T> _values;
        private readonly Func<T, string> _itemTextSelector;
        private readonly string _callbackPrefix;
        public int PageSize { get; }

        public int TotalPages { get; }


        public Paging(
            List<T> values, 
            Func<T, string> itemTextSelector, 
            string callbackPrefix, 
            int pageSize)
        {
            _values = values;
            _itemTextSelector = itemTextSelector;
            _callbackPrefix = callbackPrefix;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)values.Count / pageSize);
        }

        public InlineKeyboardMarkup GetPageMarkup(int pageNumber)
        {
            var keyboard = GetKeyboard(pageNumber, TotalPages);
            return keyboard;
        }

        public T GetItem(string data)
        {
            return _values.FirstOrDefault(i => i.GetHashCode().ToString() == data);
        }

        private List<T> GetPage(int page)
        {
            return _values.Skip(page * PageSize).Take(PageSize).ToList();
        }

        private InlineKeyboardMarkup GetKeyboard(int page, int totalPages)
        {
            var rows = new List<List<InlineKeyboardButton>>();
            var pageItems = GetPage(page);

            var objectButtons = pageItems
                .Select(item => InlineKeyboardButton.WithCallbackData(_itemTextSelector(item), $"{_callbackPrefix}item_{item.GetHashCode()}"))
                .ToList();

            rows.Add(objectButtons);

            var navButtons = new List<InlineKeyboardButton>();
            if (page > 0)
                navButtons.Add(InlineKeyboardButton.WithCallbackData(Constants.CommandNames.Prev, $"{_callbackPrefix}page_{page - 1}"));
            if (page < totalPages - 1)
                navButtons.Add(InlineKeyboardButton.WithCallbackData(Constants.CommandNames.Next, $"{_callbackPrefix}page_{page + 1}"));

            if (navButtons.Any())
                rows.Add(navButtons);

            rows.Add(new List<InlineKeyboardButton> {
                InlineKeyboardButton.WithCallbackData("Назад", $"{_callbackPrefix}{Constants.CommandNames.ACTION_BACK}") });

            return new InlineKeyboardMarkup(rows);
        }
    }
}
