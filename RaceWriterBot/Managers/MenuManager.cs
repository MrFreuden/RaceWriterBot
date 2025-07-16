using RaceWriterBot.Infrastructure;
using RaceWriterBot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class Menu
    {
        public string Text;
        public string[] Buttons;
    }

    public class MenuManager
    {
        private readonly IBotMessenger _botMessenger;
        

        public MenuManager(IBotMessenger botMessenger)
        {
            _botMessenger = botMessenger;
        }

        public void ShowMenu()
        {

        }

        public Menu NavigateBack()
        {
            if (_menuHistory.TryPeek(out var menu))
            { 
            
            }
            return menu;
        }

        public void ClearHistory()
        {
            _menuHistory.Clear();
        }
        private void HandlePaginationAction<T>(
            long userId, long chatId, int messageId,
            string pageType, string action, string data,
            Action<T> onItemSelected)
        {
            var paging = _paginationState.GetPagination<T>(userId, pageType);
            if (paging == null) return;

            switch (action)
            {
                case "page":
                    if (int.TryParse(data, out var pageNumber))
                    {
                        var markup = paging.GetPageMarkup(pageNumber);
                        _botMessenger.EditMessageReplyMarkup(chatId, messageId, markup);
                    }
                    break;

                case "item":
                    var selectedItem = paging.GetItem(data);
                    if (selectedItem != null)
                    {
                        SaveNavigationState(userId, pageType, selectedItem);
                        onItemSelected(selectedItem);
                    }
                    break;

                case "back":
                    NavigateBack(userId, chatId, messageId);
                    break;
            }
        }

        private void SaveNavigationState<T>(long userId, string pageType, T context)
        {
            if (!_navigationHistory.TryGetValue(userId, out var history))
            {
                history = new Stack<(string pageType, object context)>();
                _navigationHistory[userId] = history;
            }

            history.Push((pageType, context));
        }

        private void NavigateBack(long userId, long chatId, int messageId)
        {
            if (!_navigationHistory.TryGetValue(userId, out var history) || history.Count == 0)
            {
                Settings(userId);
                return;
            }

            var (previousPageType, previousContext) = history.Pop();

            switch (previousPageType)
            {
                case Constants.CommandNames.CHANNELS_PAGE:
                    Settings(userId);
                    break;

                case Constants.CommandNames.HASHTAGS_PAGE:
                    if (previousContext is TargetChatSession session)
                        ShowHashtags(userId, session, messageId);
                    break;

                case Constants.CommandNames.MESSAGES_PAGE:
                    if (previousContext is HashtagSession hashtag)
                        ShowTemplateMessage(userId, hashtag, messageId);
                    break;
            }
        }
    }
}