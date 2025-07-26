using RaceWriterBot.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Models
{
    public class Menu
    {
        public string Text;
        public Dictionary<string, string> ButtonsData = [];
        public PageType? PageType;
        private bool _isInputAwaiting = true;
        public const int MaxPerRow = 3;
        public const int MaxDataRows = 2;
        public bool IsInputAwaiting { get => _isInputAwaiting; set => _isInputAwaiting = value; }


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
}