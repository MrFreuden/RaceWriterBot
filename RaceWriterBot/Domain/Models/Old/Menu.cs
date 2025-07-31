using RaceWriterBot.Enums;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Infrastructure.Handlers;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Domain.Models.Old
{
    public class Menu
    {
        public string Text;
        public Dictionary<string, string> ButtonsData = [];
        public PageType? PageType;
        private bool _isInputAwaiting = false;
        public const int MaxPerRow = 3;
        public const int MaxDataRows = 2;
        public Paging Paging {  get; set; }
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

            rows.Add([InlineKeyboardButton.WithCallbackData("Назад", $"{CallbackType.Command}_{CallbackAction.Back}")]);

            return new InlineKeyboardMarkup(rows);
        }

        public Menu AddButton(string text, string callbackData)
        {
            ButtonsData[text] = callbackData;
            return this;
        }
    }
}