using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;

namespace RaceWriterBot.Infrastructure
{

    public class UserDataStorage : IUserDataStorage
    {
        private readonly Dictionary<long, UserSession> _userSessions = [];
        private readonly Dictionary<long, Stack<Menu>> _userMenuHistory = [];
        private readonly Dictionary<long, IDialogState> _userDialogs = [];
        private readonly Dictionary<long, Dictionary<string, object>> _userPaginations = [];

        public UserDataStorage()
        {
            var defaultUser = new UserSession { UserChatId = 190866300 };
            defaultUser.AddTargetChatSession(new TargetChatSession("Test1", 123));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test2", 124));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test3", 125));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test4", 126));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test5", 127));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test6", 128));
            defaultUser.AddTargetChatSession(new TargetChatSession("Test7", 129));

            _userSessions.Add(190866300, defaultUser);
        }

        public void AddUserSession(long userId)
        {
            if (!_userSessions.ContainsKey(userId))
                _userSessions.Add(userId, new UserSession() { UserChatId = userId });
        }

        public UserSession GetUserSession(long userId)
        {
            return _userSessions.TryGetValue(userId, out var session)
            ? session
            : throw new InvalidOperationException("User not found");
        }

        public void AddMenuHistory(long userId, Menu menu)
        {
            if (!_userMenuHistory.ContainsKey(userId))
                _userMenuHistory[userId] = new Stack<Menu>();

            _userMenuHistory[userId].Push(menu);
        }

        public Stack<Menu> GetMenuHistory(long userId)
        {
            if (!_userMenuHistory.ContainsKey(userId))
                _userMenuHistory[userId] = new Stack<Menu>();

            return _userMenuHistory[userId];
        }

        public void AddUserDialog(long userId, IDialogState dialogState)
        {
            _userDialogs[userId] = dialogState;
        }

        public IDialogState GetUserDialog(long userId)
        {
            return _userDialogs.TryGetValue(userId, out var dialog) ? dialog : null;
        }

        public void SavePagination<T>(long userId, string paginationType, Paging<T> paging)
        {
            if (!_userPaginations.TryGetValue(userId, out var paginations))
            {
                paginations = new Dictionary<string, object>();
                _userPaginations[userId] = paginations;
            }

            paginations[paginationType] = paging;
        }

        public Paging<T> GetPagination<T>(long userId, string pagionationType)
        {
            if (_userPaginations.TryGetValue(userId, out var paginations) &&
                paginations.TryGetValue(pagionationType, out var paging) &&
                paging is Paging<T> typedPaging)
            {
                return typedPaging;
            }

            return null;
        }

        public TargetChatSession GetTargetChatSession(long userId, long targetChatId)
        {
            throw new NotImplementedException();
        }

        public HashtagSession? GetHashtagSession(long userId, string hashtagName)
        {
            var userSession = GetUserSession(userId);
            var hashtag = userSession.TargetChats
                .SelectMany(c => c.Hashtags)
                .FirstOrDefault(h => h.HashtagName == hashtagName);
            return hashtag;
        }


        public IReadOnlyList<TargetChatSession> GetTargetChatSessions(long userId)
        {
            return GetUserSession(userId).TargetChats;
        }

        public IReadOnlyList<HashtagSession> GetHashtagSessions(long userId, long targetChatId)
        {
            var user = GetUserSession(userId);
            var chat = user.TargetChats.FirstOrDefault(c => c.TargetChatId == targetChatId)
                       ?? throw new InvalidOperationException("Target chat not found");

            return chat.Hashtags;
        }

        public void AddTargetChatSession(long userId, TargetChatSession targetChatSession)
        {
            var user = GetUserSession(userId);
            user.AddTargetChatSession(targetChatSession);
        }

        public void AddHashtagSession(long userId, long targetChatId, HashtagSession hashtag)
        {
            var user = GetUserSession(userId);
            user.AddHashtag(targetChatId, hashtag);
        }

        public void UpdateHashtagTemplate(long userId, string hashtagName, string newTemplate)
        {
            var user = GetUserSession(userId);
            user.UpdateHashtagTextTemplate(hashtagName, newTemplate);
        }

        public void UpdateHashtagName(long userId, string hashtagName, string newName)
        {
            var user = GetUserSession(userId);
            user.UpdateHashtagName(hashtagName, newName);
        }

        public void SetExpectedAction<T>(long userId, string action, T context = default)
        {
            _userDialogs[userId] = new DialogState<T> { ExpectedAction = action, Context = context };
        }

        public IDialogState? GetCurrentDialog(long userId)
        {
            return _userDialogs.TryGetValue(userId, out var dialog) ? dialog : null;
        }

        public T GetDialogContext<T>(long userId)
        {
            if (_userDialogs.TryGetValue(userId, out var dialog) &&
                dialog is DialogState<T> typedDialog)
            {
                return typedDialog.Context;
            }
            return default;
        }

        public bool TryGetDialogState<T>(long userId, out T context, out string action)
        {
            context = default;
            action = null;

            if (_userDialogs.TryGetValue(userId, out var dialog) &&
                dialog is DialogState<T> typedDialog)
            {
                context = typedDialog.Context;
                action = typedDialog.ExpectedAction;
                return true;
            }

            return false;
        }

        public void ClearDialog(long userId)
        {
            _userDialogs.Remove(userId);
        }
    }
}
