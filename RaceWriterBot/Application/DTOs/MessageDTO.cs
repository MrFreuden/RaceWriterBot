using RaceWriterBot.Domain.ValueObjects;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Application.DTOs
{
    public class MessageDTO
    {
        public UserId UserId { get; set; }
        public string? Text { get; set; }
        public int? MessageId { get; set; }
        public ReplyMarkup? ReplyMarkup { get; set; }
        public InlineKeyboardMarkup? InlineKeyboardMarkup { get; set; }
    }
}
