using Telegram.Bot.Types;

namespace RaceWriterBot.Application.Interfaces
{
    public interface ICallbackQueryHandler
    {
        Task ProcessCallbackQuery(CallbackQuery callbackQuery);
    }
}
