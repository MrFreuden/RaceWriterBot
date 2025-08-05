using RaceWriterBot.Domain.Models;
using RaceWriterBot.Domain.ValueObjects;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterBot.Application.DTOs
{
    public class MessageDTO
    {
        public UserId UserId { get; set; }
        public string? Text { get; set; }
        public int? MessageId { get; set; }
        public List<MenuAction> Actions { get; set; }
    }
}
