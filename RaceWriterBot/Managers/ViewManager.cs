using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;

namespace RaceWriterBot.Managers
{
    public class ViewManager : IViewManager
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly MenuManager _menuManager;
        private const int _countObjectsPerPage = 3;

        public ViewManager(MenuManager menuManager, IUserDataStorage userDataStorage)
        {
            _userDataStorage = userDataStorage;
            _menuManager = menuManager;
        }

        public async Task Settings(long chatId)
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
                    await _menuManager.ShowMenu(chatId, menu);
                }
                else
                {
                    await _menuManager.ShowPagingMenu(chatId, "Активні канали", targetChatSessions.ToList(), session => session.Name, Constants.CommandNames.CHANNELS_PAGE, _countObjectsPerPage);
                }
            }
        }

        public async Task ShowMessageDetails(long userId, PostMessagePair pair)
        {
            throw new NotImplementedException();
        }

        public async Task ShowTemplateMessage(long userId, HashtagSession hashtag)
        {
            var menu = new Menu
            {
                Text = hashtag.TextTemplate,
                ButtonsData = { ["Редагувати"] = $"{Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE}_{hashtag.HashtagName}" },
            };
            await _menuManager.ShowMenu(userId, menu);
        }

        public async Task ShowHashtags(long userId, TargetChatSession channel)
        {
            var hashtags = _userDataStorage.GetUser(userId).GetHashtagSessions(channel.TargetChatId);

            if (hashtags == null || hashtags.Count == 0)
            {
                var menu = new Menu
                {
                    Text = $"Канал {channel.Name} не має хештегів",
                    ButtonsData = { ["Створити"] = $"{Constants.CommandNames.ACTION_ADD_HASHTAG}_{channel.GetHashCode()}" }
                };
                await _menuManager.ShowMenu(userId, menu);
                return;
            }

            await _menuManager.ShowPagingMenu(
                userId,
                $"Хештеги для каналу {channel.Name}:",
                hashtags.ToList(),
                hashtag => hashtag.HashtagName,
                Constants.CommandNames.HASHTAGS_PAGE,
                _countObjectsPerPage);
        }

        public async Task AddBotToTargetChatSettings(long chatId)
        {
            var menu = new Menu
            {
                Text = $"Додайте бота в чат обговорень каналу та дайте йому права адміністратора",
                ButtonsData = { ["Зроблено"] = Constants.CommandNames.ACTION_CONFIRMATION_ADDING_BOT }
            };
            await _menuManager.ShowMenu(chatId, menu);
        }

        public async Task RequestForwardedMessage(long chatId)
        {
            var menu = new Menu
            {
                Text = $"Надішліть будь-яке повідомлення з чату обговорень",
            };
            await _menuManager.ShowMenu(chatId, menu);
        }

        public async Task ReturnToPreviousMenu(long chatId)
        {
            await _menuManager.NavigateBack(chatId);
        }

        public async Task ShowErrorMessage(long chatId)
        {
            var menu = new Menu
            {
                Text = $"Сталася непердбачувана помилка",
            };
            await _menuManager.ShowMenu(chatId, menu);
        }

        public async Task AddNewHashtag(long userId, int channelHash)
        {
            var user = _userDataStorage.GetUser(userId);
            var channelSession = user.GetTargetChatSessions(channelHash);

            if (channelSession != null)
            {
                user.SetExpectedAction(Constants.CommandNames.ACTION_ADD_HASHTAG, channelSession);
                var menu = new Menu
                {
                    Text = $"Введіть новий хештег",
                };
                await _menuManager.ShowMenu(userId, menu);
            }
        }

        public async Task StartEditHashtagTemplate(long userId, string hashtagName)
        {
            var user = _userDataStorage.GetUser(userId);
            var hashtag = user.GetHashtagSession(hashtagName);
            var menu = new Menu();
            if (hashtag == null)
            {
                menu.Text = $"Хештег не знайдено або у вас немає прав для його редагування.";
                await _menuManager.ShowMenu(userId, menu);
                return;
            }

            user.SetExpectedAction(Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE, hashtag);

            menu.Text = $"Будь ласка, введіть новий текст шаблону для хештега #{hashtagName}";
            await _menuManager.ShowMenu(userId, menu);
        }
    }
}