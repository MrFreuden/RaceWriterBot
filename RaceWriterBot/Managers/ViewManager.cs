using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.Models.Old;
using RaceWriterBot.Enums;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Presentation.Handlers;
using System.Threading.Channels;
using Telegram.Bot.Types;

namespace RaceWriterBot.Managers
{
    public class ViewManager : IViewManager
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly MenuManager _menuManager;

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
                        ButtonsData = { ["Створити"] = $"{CallbackType.Command}_{CallbackAction.CreateTargetChat}"},
                        PageType = PageType.Channels
                    };
                    await _menuManager.ShowMenu(chatId, menu);
                }
                else
                {
                    var menu = new Menu
                    {
                        Text = "Активні канали",
                        PageType = Enums.PageType.Channels,
                    };

                    foreach (var session in targetChatSessions)
                    {
                        menu.ButtonsData[session.Name] = $"{CallbackType.Paging}_{Enums.PageType.Channels}_{PaginationAction.Item}_{session.TargetChatId}";
                    }
                    await _menuManager.ShowMenu(chatId, menu);
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
                ButtonsData = { ["Редагувати"] = $"{CallbackType.Command}_{CallbackAction.EditHashtagTemplate}_{hashtag.HashtagName}" },
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
                    ButtonsData = { ["Створити"] = $"{CallbackType.Command}_{CallbackAction.AddHashtag}_{channel.TargetChatId}" },
                    PageType = PageType.Hashtags
                };
                await _menuManager.ShowMenu(userId, menu);
                return;
            }
            else
            {
                var menu = new Menu
                {
                    Text = $"Хештеги для каналу {channel.Name}:",
                    PageType = Enums.PageType.Channels,
                };

                foreach (var hashtag in hashtags)
                {
                    menu.ButtonsData[hashtag.HashtagName] = $"{CallbackType.Paging}_{Enums.PageType.Hashtags}_{PaginationAction.Item}_{hashtag.HashtagName}";
                }
                await _menuManager.ShowMenu(userId, menu);
            }
        }

        public async Task AddBotToTargetChatSettings(long chatId)
        {
            var menu = new Menu
            {
                Text = $"Додайте бота в чат обговорень каналу та дайте йому права адміністратора",
                ButtonsData = { ["Зроблено"] = $"{CallbackType.Command}_{CallbackAction.AddBot}" }
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
            var menu = _userDataStorage.GetUser(chatId).GetLastMenu();
            
            if (menu.PageType != null)
            {
                switch (menu.PageType)
                {
                    case PageType.Channels:
                        await Settings(chatId);
                        break;

                    case PageType.Hashtags:
                        await ShowHashtags(chatId,_userDataStorage.GetUser(chatId).GetTargetChatSession(long.Parse(menu.ButtonsData.First().Value.Split('_').Last())));
                        break;

                    default:
                        break;
                }
                return;
            }
            
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

        public async Task AddNewHashtag(long userId, long channelId)
        {
            var user = _userDataStorage.GetUser(userId);
            var channelSession = user.GetTargetChatSession(channelId);

            if (channelSession != null)
            {
                user.SetExpectedAction(CallbackAction.AddHashtag.ToString(), channelSession);
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

            user.SetExpectedAction(CallbackAction.EditHashtagTemplate.ToString(), hashtag);

            menu.Text = $"Будь ласка, введіть новий текст шаблону для хештега #{hashtagName}";
            await _menuManager.ShowMenu(userId, menu);
        }

        public async Task CommentWithTemplateMessage(long userId, long channelId, string hashtagName)
        {
            var user = _userDataStorage.GetUser(userId);
            var hashtag = user.GetHashtagSession(hashtagName);

            await 
        }
    }
}