namespace RaceWriterBot.Temp
{
    public interface IDialogState
    {
        string ExpectedAction { get; set; }
    }
    public class DialogState<T> : IDialogState
    {
        public string ExpectedAction { get; set; }
        public T Context { get; set; }
    }

    public class UserDialogManager
    {
        private readonly Dictionary<long, IDialogState> _userDialogs = new();

        public void SetExpectedAction<T>(long userId, string action, T context = default)
        {
            _userDialogs[userId] = new DialogState<T> { ExpectedAction = action, Context = context };
        }

        public IDialogState GetCurrentDialog(long userId)
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