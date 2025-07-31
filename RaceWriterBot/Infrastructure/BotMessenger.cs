using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Infrastructure
{
    public class BotMessenger : IBotMessenger
    {
        private readonly ITelegramBotClient _client;

        public BotMessenger(ITelegramBotClient client)
        {
            _client = client;
        }

        public async Task<Message> SendMessage(ChatId chatId, string text)
        {
            return await _client.SendMessage(chatId, text);
        }

        public async Task<Message> SendMessage(ChatId chatId, string text, ReplyMarkup replyMarkup)
        {
            return await _client.SendMessage(chatId, text, replyMarkup: replyMarkup);
        }

        public async Task<Message> EditMessageReplyMarkup(ChatId chatId, int messageId, InlineKeyboardMarkup keyboardMarkup)
        {
            return await _client.EditMessageReplyMarkup(chatId, messageId, keyboardMarkup);
        }

        public async Task<Message> EditMessageText(ChatId chatId, int messageId, string text)
        {
            return await _client.EditMessageText(chatId, messageId, text);
        }

        public async Task<Message> EditMessageText(ChatId chatId, int messageId, string text, InlineKeyboardMarkup keyboardMarkup)
        {
            return await _client.EditMessageText(chatId, messageId, text, replyMarkup: keyboardMarkup);
        }
    }

    public interface IBotMessenger
    {
        Task<Message> EditMessageReplyMarkup(ChatId chatId, int messageId, InlineKeyboardMarkup keyboardMarkup);
        Task<Message> EditMessageText(ChatId chatId, int messageId, string text, InlineKeyboardMarkup keyboardMarkup);
        Task<Message> EditMessageText(ChatId chatId, int messageId, string text);
        Task<Message> SendMessage(ChatId chatId, string text);
        Task<Message> SendMessage(ChatId chatId, string text, ReplyMarkup replyMarkup);
    }
}
