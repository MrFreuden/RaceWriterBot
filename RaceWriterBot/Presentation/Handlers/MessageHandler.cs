using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Infrastructure;
using Telegram.Bot.Types;

namespace RaceWriterBot.Presentation.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IMessageSender _messageSender;
        private readonly IMessageParser _messageParser;

        public MessageHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public async Task ProcessMessage(Message message)
        {
            var response = _messageParser.HandleMessage(message);
            var m = await _messageSender.SendMessage(response);
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
    }
}