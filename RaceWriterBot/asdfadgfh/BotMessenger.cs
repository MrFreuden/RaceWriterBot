using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.asdfadgfh
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
            var task = await _client.SendMessage(chatId, text);
            return task;
        }

        public async Task<Message> SendMessage(ChatId chatId, string text, ReplyMarkup replyMarkup)
        {
            var task = await _client.SendMessage(chatId, text, replyMarkup: replyMarkup);
            return task;
        }

        public async Task<Message> EditMessageReplyMarkup(ChatId chatId, int messageId, InlineKeyboardMarkup keyboardMarkup)
        {
            var task = await _client.EditMessageReplyMarkup(chatId, messageId, keyboardMarkup);
            return task;
        }

        public async Task<Message> EditMessageText(ChatId chatId, int messageId, string text)
        {
            var task = await _client.EditMessageText(chatId, messageId, text);
            return task;
        }

        public async Task<Message> EditMessageText(ChatId chatId, int messageId, string text, InlineKeyboardMarkup keyboardMarkup)
        {
            var task = await _client.EditMessageText(chatId, messageId, text);
            return task;
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
