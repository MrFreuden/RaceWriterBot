using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using Telegram.Bot.Types;

namespace RaceWriterBot.Managers
{
    public class DialogProcessor : IDialogProcessor
    {
        private readonly IUserDataStorage _userDataStorage;

        public DialogProcessor(IUserDataStorage userDataStorage)
        {
            _userDataStorage = userDataStorage;
        }

        public bool HasActiveDialog(long userId)
        {
            var dialog = _userDataStorage.GetCurrentDialog(userId);
            return dialog != default;
        }

        public void ProcessDialogMessage(long userId, Message message)
        {
            var dialog = _userDataStorage.GetCurrentDialog(userId);
            switch (dialog.ExpectedAction)
            {
                case Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE:
                    ProcessHashtagTemplateEdit(message);
                    break;

                case Constants.CommandNames.ACTION_ADD_HASHTAG:
                    ProcessHashtagAdd(message);
                    break;

                case Constants.CommandNames.ACTION_EDIT_HASHTAG:
                    ProcessHashtagEdit(message);
                    break;
            }
        }

        public void ProcessHashtagAdd(Message message)
        {
            var userId = message.From.Id;
            if (_userDataStorage.TryGetDialogState(userId, out TargetChatSession chatSession, out _))
            {
                _userDataStorage.ClearDialog(userId);


                if (chatSession != null)
                {
                    //TODO
                    var hash = new HashtagSession() { HashtagName = message.Text };
                    _userDataStorage.AddHashtagSession(userId, chatSession.TargetChatId, hash);
                }
            }
        }

        public void ProcessHashtagEdit(Message message)
        {

        }

        public void ProcessHashtagTemplateEdit(Message message)
        {
            var userId = message.From.Id;
            if (_userDataStorage.TryGetDialogState(userId, out HashtagSession hashtag, out _))
            {
                _userDataStorage.ClearDialog(userId);


                if (hashtag != null)
                {
                    hashtag.TextTemplate = message.Text;
                    _userDataStorage.UpdateHashtagTemplate(userId, hashtag.HashtagName, hashtag.TextTemplate);
                }
            }
        }
    }
}