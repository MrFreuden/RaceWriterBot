using Telegram.Bot;
using Telegram.Bot.Types;
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
    }

    public interface IBotMessenger
    {
        Task<Message> SendMessage(ChatId chatId, string text);
        Task<Message> SendMessage(ChatId chatId, string text, ReplyMarkup replyMarkup);
    }
}
