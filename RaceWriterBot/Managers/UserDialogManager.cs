using RaceWriterBot.Models;

namespace RaceWriterBot.Managers
{
    public class UserDialogManager
    {
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