using RaceWriterBot.Application.DTOs;
using Telegram.Bot.Types;

namespace RaceWriterBot.Presentation.Handlers
{
    public interface IMessageParser
    {
        MessageDTO HandleMessage(Message message);
    }
}