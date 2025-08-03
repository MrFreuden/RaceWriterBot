using Telegram.Bot.Types;

namespace RaceWriterBot.Application.Interfaces
{
    public interface IMessageHandler
    {
        Task ProcessMessage(Message message);
    }
}
