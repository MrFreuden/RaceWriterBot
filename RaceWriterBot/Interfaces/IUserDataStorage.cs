using RaceWriterBot.Infrastructure;
using RaceWriterBot.Models;

namespace RaceWriterBot.Interfaces
{
    public interface IUserDataStorage
    {
        void AddHashtagSession(long userId, long targetChatId, HashtagSession hashtag);
        void AddMenuHistory(long userId, Menu menu);
        void AddTargetChatSession(long userId, TargetChatSession targetChatSession);
        void AddUserDialog(long userId, IDialogState dialogState);
        void AddUserSession(long userId);
        IReadOnlyList<HashtagSession> GetHashtagSessions(long userId, long targetChatId);
        Stack<Menu> GetMenuHistory(long userId);
        Paging<T> GetPagination<T>(long userId, string pagionationType);
        IReadOnlyList<TargetChatSession> GetTargetChatSessions(long userId);
        IDialogState GetUserDialog(long userId);
        UserSession GetUserSession(long userId);
        void SavePagination<T>(long userId, string paginationType, Paging<T> paging);
        void UpdateHashtagName(long userId, string hashtagName, string newName);
        void UpdateHashtagTemplate(long userId, string hashtagName, string newTemplate);
        HashtagSession? GetHashtagSession(long userId, string hashtagName);
        void SetExpectedAction<T>(long userId, string action, T context = default);
        IDialogState? GetCurrentDialog(long userId);
        T GetDialogContext<T>(long userId);
        bool TryGetDialogState<T>(long userId, out T context, out string action);
        void ClearDialog(long userId);
    }
}
