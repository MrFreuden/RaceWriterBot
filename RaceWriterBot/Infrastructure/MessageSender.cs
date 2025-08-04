using RaceWriterBot.Application.DTOs;
using Telegram.Bot;
using Telegram.Bot.Types;

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
            if (message.MessageId != null)
            {
                await EditMessageText(message);
            }
            return message.ReplyMarkup == null 
                ? await _client.SendMessage(message.UserId.Id, message.Text)
                : await _client.SendMessage(message.UserId.Id, message.Text, replyMarkup: message.ReplyMarkup);
        }
        public async Task<Message> EditMessageReplyMarkup(MessageDTO message)
        {
            return await _client.EditMessageReplyMarkup(
                message.UserId.Id,
                message.MessageId.Value,
                message.InlineKeyboardMarkup);
        }

        private async Task<Message> EditMessageText(MessageDTO message)
        {
            return message.ReplyMarkup == null
                ? await _client.EditMessageText(message.UserId.Id, message.MessageId.Value, message.Text)
                : await _client.EditMessageText(
                    message.UserId.Id,
                    message.MessageId.Value,
                    message.Text,
                    replyMarkup: message.InlineKeyboardMarkup);
        }

        
    }
    public interface IMessageSender
    {
        Task<Message> EditMessageReplyMarkup(MessageDTO message);
        //Task<Message> EditMessageText(MessageDTO message);
        Task<Message> SendMessage(MessageDTO message);
    }
}
