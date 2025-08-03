using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Presentation.Interfaces;
using Telegram.Bot.Types;

namespace RaceWriterBot.Presentation.Handlers
{
    public class CallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly IMessageSender _messageSender;
        private readonly IStateService _stateService;
        
        public CallbackQueryHandler(IMessageSender messageSender, IStateService stateService)
        {
            _messageSender = messageSender;
            _stateService = stateService;
        }

        public async Task ProcessCallbackQuery(CallbackQuery query)
        {
            var userId = new UserId(query.From.Id);
            var response = _stateService.HandleCallback(query.Data, userId);
            var message = await _messageSender.SendMessage(response);
        }
    }
}