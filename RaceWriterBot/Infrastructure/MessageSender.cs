using RaceWriterBot.Domain.ValueObjects;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Infrastructure
{
    public class MessageSender : IMessageSender
    {
        private readonly ITelegramBotClient _client;

        public MessageSender(ITelegramBotClient client)
        {
            _client = client;
        }

        public async Task<Message> SendMessage(MessageDTO message)
        {
            return message.ReplyMarkup == null 
                ? await _client.SendMessage(message.UserId.Id, message.Text)
                : await _client.SendMessage(message.UserId.Id, message.Text, replyMarkup: message.ReplyMarkup);
        }
        public async Task<Message> EditMessageReplyMarkup(MessageDTO message)
        {
            return await _client.EditMessageReplyMarkup(
                message.UserId.Id,
                message.MessageId,
                message.InlineKeyboardMarkup);
        }
        public async Task<Message> EditMessageText(MessageDTO message)
        {
            return message.ReplyMarkup == null
                ? await _client.EditMessageText(message.UserId.Id, message.MessageId, message.Text)
                : await _client.EditMessageText(
                    message.UserId.Id,
                    message.MessageId,
                    message.Text,
                    replyMarkup: message.InlineKeyboardMarkup);
        }

        
    }
    public class MessageDTO
    {
        public UserId UserId { get; set; }
        public string? Text { get; set; }
        public int? MessageId { get; set; }
        public ReplyMarkup? ReplyMarkup { get; set; }
        public InlineKeyboardMarkup? InlineKeyboardMarkup { get; set; }
    }
    public interface IMessageSender
    {
        Task<Message> EditMessageReplyMarkup(MessageDTO message);
        Task<Message> EditMessageText(MessageDTO message);
        Task<Message> SendMessage(MessageDTO message);
    }
}
