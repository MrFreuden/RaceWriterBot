using RaceWriterBot.Interfaces;
using RaceWriterBot.Models;
using RaceWriterBot.Temp;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Managers
{
    public class ViewManager : IViewManager
    {
        public void Settings(long chatId)
        {
            var targetChatSessions = _userDataStorage.GetTargetChatSessions(chatId);
            if (targetChatSessions != null)
            {
                if (targetChatSessions.Count == 0)
                {
                    _botMessenger.SendMessage(chatId, "У вас немає активних каналів",
                        replyMarkup: new InlineKeyboardButton[][]
                        {
                            [("Створити", Constants.CommandNames.ACTION_CREATE_TARGET_CHAT)]
                        });
                }
                else
                {
                    var paging = new Paging<TargetChatSession>(
                        targetChatSessions.ToList(),
                        session => session.Name,
                        $"{Constants.CommandNames.CHANNELS_PAGE}_",
                        CountObjectsPerPage);

                    _paginationState.SavePagination(chatId, Constants.CommandNames.CHANNELS_PAGE, paging);

                    var keyboard = paging.GetPageMarkup(0);
                    _botMessenger.SendMessage(chatId, "Активні канали", keyboard);
                }
            }
        }
        public void ShowMessageDetails(long userId, PostMessagePair pair, int messageId)
        {
            throw new NotImplementedException();
        }

        public void ShowTemplateMessage(long userId, HashtagSession hashtag, int messageId)
        {
            var text = hashtag.TextTemplate;
            var markup = new InlineKeyboardButton("Редагувати", $"EditTemplateMessageText_{hashtag.HashtagName}");
            _botMessenger.EditMessageText(userId, messageId, text, markup);
        }

        public void ShowHashtags(long userId, TargetChatSession channel, int messageId)
        {
            var hashtags = _userDataStorage.GetHashtagSessions(userId, channel.TargetChatId);

            if (hashtags == null || hashtags.Count == 0)
            {
                var keyboard = new InlineKeyboardButton("Створити", $"AddHashtag_{channel.GetHashCode()}");

                _botMessenger.EditMessageText(
                    userId,
                    messageId,
                    $"Канал {channel.Name} не має хештегів",
                    keyboard);
                return;
            }

            var paging = new Paging<HashtagSession>(
                hashtags.ToList(),
                hashtag => hashtag.HashtagName,
                $"{Constants.CommandNames.HASHTAGS_PAGE}_",
                CountObjectsPerPage);

            _paginationState.SavePagination(userId, Constants.CommandNames.HASHTAGS_PAGE, paging);

            var markup = paging.GetPageMarkup(0);
            _botMessenger.EditMessageText(userId, messageId, $"Хештеги для каналу {channel.Name}:", markup);
        }

    }
}