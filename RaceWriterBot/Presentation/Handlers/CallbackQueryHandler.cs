using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Presentation.Interfaces;
using Telegram.Bot.Types;

namespace RaceWriterBot.Presentation.Handlers
{
    public class CallbackQueryHandler
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
            await _messageSender.EditMessageText(response);
        }
    }

    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IStateFactory _stateFactory;
        private readonly Dictionary<InputRequestType, string> messages = new()
        {
            [InputRequestType.HashtagName] = "Введіть новий хештег",
            [InputRequestType.TemplateText] = "Введіть новий текст шаблону хештега"
        };

        public StateService(IStateRepository stateRepository, IStateFactory stateFactory)
        {
            _stateRepository = stateRepository;
            _stateFactory = stateFactory;
            
        }

        public MessageDTO HandleCallback(string data, UserId userId)
        {
            var state = _stateFactory.CreateFromCallback(data, userId);
            _stateRepository.AddState(userId, state);
            var m = new MessageDTO { UserId = userId, Text = messages[state.GetRequiredInput()], MessageId = 0 };
            return m;
        }
    }
   
    public interface IStateService
    {
        MessageDTO HandleCallback(string data, UserId userId); 
    }

    public enum InputRequestType
    {
        HashtagName,
        TemplateText,
    }
}