using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using System.Threading.Channels;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class ViewManager : IViewManager
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly MenuManager _menuManager;
        private const int CountObjectsPerPage = 3;

        public ViewManager(MenuManager menuManager, IUserDataStorage userDataStorage)
        {
            _userDataStorage = userDataStorage;
            _menuManager = menuManager;
        }

        public void Settings(long chatId)
        {
            var targetChatSessions = _userDataStorage.GetUser(chatId).GetTargetChatSessions();
            if (targetChatSessions != null)
            {
                if (targetChatSessions.Count == 0)
                {
                    var menu = new Menu
                    {
                        Text = "У вас немає активних каналів",
                        ButtonsData = { ["Створити"] = Constants.CommandNames.ACTION_CREATE_TARGET_CHAT }
                    };
                    _menuManager.ShowMenu(chatId, menu);
                }
                else
                {
                    _menuManager.ShowPagingMenu(chatId, "Активні канали", targetChatSessions.ToList(), session => session.Name, Constants.CommandNames.CHANNELS_PAGE, CountObjectsPerPage);
                }
            }
        }

        public void ShowMessageDetails(long userId, PostMessagePair pair, int messageId)
        {
            throw new NotImplementedException();
        }

        public void ShowTemplateMessage(long userId, HashtagSession hashtag, int messageId)
        {
            var menu = new Menu
            {
                Text = hashtag.TextTemplate,
                ButtonsData = { ["Редагувати"] = $"{Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE}_{hashtag.HashtagName}" },
            };
            _menuManager.ShowMenu(userId, menu, messageId);
        }

        public void ShowHashtags(long userId, TargetChatSession channel, int messageId)
        {
            var hashtags = _userDataStorage.GetUser(userId).GetHashtagSessions(channel.TargetChatId);

            if (hashtags == null || hashtags.Count == 0)
            {
                var menu = new Menu
                {
                    Text = $"Канал {channel.Name} не має хештегів",
                    ButtonsData = { ["Створити"] = $"{Constants.CommandNames.ACTION_ADD_HASHTAG}_{channel.GetHashCode()}" }
                };
                _menuManager.ShowMenu(userId, menu, messageId);
                return;
            }

            _menuManager.ShowPagingMenu(userId, $"Хештеги для каналу {channel.Name}:", hashtags.ToList(), hashtag => hashtag.HashtagName, Constants.CommandNames.HASHTAGS_PAGE, CountObjectsPerPage, messageId);
        }

        public void AddBotToTargetChatSettings(long chatId)
        {
            var menu = new Menu
            {
                Text = $"Додайте бота в чат обговорень каналу та дайте йому права адміністратора",
                ButtonsData = { ["Зроблено"] = Constants.CommandNames.ACTION_CONFIRMATION_ADDING_BOT }
            };
            _menuManager.ShowMenu(chatId, menu);
        }

        public void RequestForwardedMessage(long chatId)
        {
            var menu = new Menu
            {
                Text = $"Надішліть будь-яке повідомлення з чату обговорень",
            };
            _menuManager.ShowMenu(chatId, menu);
        }


    }
}