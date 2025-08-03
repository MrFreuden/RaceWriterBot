using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Infrastructure;
using Telegram.Bot.Types;

namespace RaceWriterBot.Presentation.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IMessageSender _botMessenger;

        public MessageHandler(IMessageSender botMessenger)
        {
            _botMessenger = botMessenger;
        }

        public Task ProcessMessage(Message message)
        {
            return Task.CompletedTask;
        }

        private string[] ParseHashtagFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Array.Empty<string>();
            }

            var words = text.Split(' ', '\n', '\r', '\t');
            var hashtags = words
                .Where(word => !string.IsNullOrEmpty(word) && word.StartsWith('#'))
                .Select(hashtag => hashtag.TrimEnd('.', ',', '!', '?', ':', ';', ')'))
                .Where(hashtag => hashtag.Length > 1)
                .ToArray();

            return hashtags;
        }

        private bool IsPrivateMessage(Message message)
        {
            return message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private;
        }

        private bool IsForwardedMessage(Message message)
        {
            return message.ForwardFromChat != null;
        }

        private bool IsReplyToPostMessage(Message message)
        {
            return message.ReplyToMessage != null;
        }

        private void ProcessPrivateMessage(Message message)
        {
            
        }
    }
}