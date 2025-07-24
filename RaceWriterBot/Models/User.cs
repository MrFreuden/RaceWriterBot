using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using Telegram.Bot.Types;

namespace RaceWriterBot.Models
{
    public class User
    {
        private UserSession _userSession;
        private Stack<Menu> _menuHistory;
        private IDialogState _dialogState;
        private Dictionary<string, object> _userPaginations;
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

        public User(long userId)
        {
            _userSession = new UserSession(userId);
            _menuHistory = new Stack<Menu>();
            _userPaginations = new Dictionary<string, object>();
        }

        public UserSession GetUserSession()
        {
            return _userSession;
        }

        public void AddMenuHistory(Menu menu)
        {
            _menuHistory.Push(menu);
        }

        public Stack<Menu> GetMenuHistory()
        {
            return _menuHistory;
        }

        public void AddUserDialog(IDialogState dialogState)
        {
            _dialogState = dialogState;
        }

        public IDialogState GetUserDialog()
        {
            return _dialogState;
        }

        public void SavePagination<T>(string paginationType, Paging<T> paging)
        {
            _userPaginations[paginationType] = paging;
        }

        public Paging<T> GetPagination<T>(string paginationType)
        {
            if (_userPaginations.TryGetValue(paginationType, out var paging) &&
                paging is Paging<T> typedPaging)
            {
                return typedPaging;
            }

            return null;
        }

        public TargetChatSession GetTargetChatSession(long targetChatId)
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

        public TargetChatSession? GetTargetChatSessions(int channelHash)
        {
            return _userSession.TargetChats
                .Where(s => s.GetHashCode() == channelHash)
                .FirstOrDefault();
        }
    }
}
