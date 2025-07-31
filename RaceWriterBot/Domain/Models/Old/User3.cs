using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;

namespace RaceWriterBot.Domain.Models.Old
{
    public class User3
    {
        private UserSession _userSession;
        private Stack<Menu> _menuHistory;
        private Menu _currentMenu;
        private IDialogState _dialogState;

        public Paging Paging { get; set; }
        private int _lastMessageIdFromBot;
        public int LastMessageIdFromBot
        {
            get => _lastMessageIdFromBot;

            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, _lastMessageIdFromBot);
                _lastMessageIdFromBot = value;
            }
        }

        public User3(long userId)
        {
            _userSession = new UserSession(userId);
            _menuHistory = new Stack<Menu>();
        }

        public void SetCurrentMenu(Menu menu)
        {
            if (_currentMenu == default)
            {
                _currentMenu = menu;
            }
            else
            {
                _menuHistory.Push(_currentMenu);
                _currentMenu = menu;
            }
        }

        public Menu GetLastMenu()
        {
            if (_menuHistory.Count != 0)
            {
                _currentMenu = default;
                return _menuHistory.Pop();
            }
            throw new InvalidOperationException();
        }

        public UserSession GetUserSession()
        {
            return _userSession;
        }

        public void AddUserDialog(IDialogState dialogState)
        {
            _dialogState = dialogState;
        }

        public IDialogState GetUserDialog()
        {
            return _dialogState;
        }


        public Paging GetPagination()
        {
            return _currentMenu.Paging;
        }

        public TargetChatSession? GetTargetChatSession(long targetChatId)
        {
            return _userSession.TargetChats.FirstOrDefault(c => c.TargetChatId == targetChatId);
        }

        public HashtagSession? GetHashtagSession(string hashtagName)
        {
            var hashtag = _userSession.TargetChats
                .SelectMany(c => c.Hashtags)
                .FirstOrDefault(h => h.HashtagName == hashtagName);
            return hashtag;
        }

        public IReadOnlyList<TargetChatSession> GetTargetChatSessions()
        {
            return _userSession.TargetChats;
        }

        public IReadOnlyList<HashtagSession> GetHashtagSessions(long targetChatId)
        {
            var chat = _userSession.TargetChats.FirstOrDefault(c => c.TargetChatId == targetChatId)
                       ?? throw new InvalidOperationException("Target chat not found");

            return chat.Hashtags;
        }

        public void AddTargetChatSession(long chatId, string titel)
        {
            if (!string.IsNullOrEmpty(titel))
            {
                _userSession.AddTargetChatSession(new TargetChatSession(titel, chatId));
            }
            else
            {
                throw new ArgumentNullException(nameof(titel));
            }
        }

        public void AddHashtagSession(long targetChatId, HashtagSession hashtag)
        {
            _userSession.AddHashtag(targetChatId, hashtag);
        }

        public void UpdateHashtagTemplate(string hashtagName, string newTemplate)
        {
            _userSession.UpdateHashtagTextTemplate(hashtagName, newTemplate);
        }

        public void UpdateHashtagName(string hashtagName, string newName)
        {
            _userSession.UpdateHashtagName(hashtagName, newName);
        }

        public void SetExpectedAction<T>(string action, T context = default)
        {
            _dialogState = new DialogState<T> { ExpectedAction = action, Context = context };
        }

        public IDialogState? GetCurrentDialog()
        {
            return _dialogState;
        }

        public T GetDialogContext<T>()
        {
            if (_dialogState is DialogState<T> typedDialog)
            {
                return typedDialog.Context;
            }
            return default;
        }

        public bool TryGetDialogState<T>(out T context, out string action)
        {
            context = default;
            action = null;

            if (_dialogState is DialogState<T> typedDialog)
            {
                context = typedDialog.Context;
                action = typedDialog.ExpectedAction;
                return true;
            }

            return false;
        }

        public void ClearDialog()
        {
            _dialogState = null;
        }

        internal string? GetTrackedHashtag(long channelId, string[] hashtags)
        {
            var hashtagSessions = GetHashtagSessions(channelId);
            return hashtags.FirstOrDefault(h => hashtagSessions.Any(s => s.HashtagName == h));
        }
    }
}
