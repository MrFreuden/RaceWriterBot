using RaceWriterBot.Domain.Models.Old;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Presentation.Handlers;
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
            var dialog = _userDataStorage.GetUser(userId).GetCurrentDialog();
            return dialog != default;
        }

        public bool ProcessDialogMessage(long userId, Message message)
        {
            var dialog = _userDataStorage.GetUser(userId).GetCurrentDialog();
            if (!Enum.TryParse(dialog.ExpectedAction, out CallbackAction action))
            {
                return false;
            }
            return action switch
            {
                CallbackAction.EditHashtagTemplate => ProcessHashtagTemplateEdit(message),
                CallbackAction.AddHashtag => ProcessHashtagAdd(message),
                CallbackAction.EditHashtagName => ProcessHashtagEdit(message),
                _ => false,
            };
        }

        private bool ProcessHashtagAdd(Message message)
        {
            var user = _userDataStorage.GetUser(message.From.Id);
            if (user.TryGetDialogState(out TargetChatSession chatSession, out _))
            {
                user.ClearDialog();
                
                if (chatSession != null)
                {
                    var hash = new HashtagSession() { HashtagName = message.Text };
                    user.AddHashtagSession(chatSession.TargetChatId, hash);
                    return true;
                }
            }
            return false;
        }

        private bool ProcessHashtagEdit(Message message)
        {
            return false;
        }

        private bool ProcessHashtagTemplateEdit(Message message)
        {
            var user = _userDataStorage.GetUser(message.From.Id);
            var userId = message.From.Id;
            if (user.TryGetDialogState(out HashtagSession hashtag, out _))
            {
                user.ClearDialog();

                if (hashtag != null)
                {
                    hashtag.TextTemplate = message.Text;
                    user.UpdateHashtagTemplate(hashtag.HashtagName, hashtag.TextTemplate);
                    return true;
                }
            }
            return false;
        }
    }
}