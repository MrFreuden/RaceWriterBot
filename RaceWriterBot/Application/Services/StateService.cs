using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Presentation.Interfaces;

namespace RaceWriterBot.Application.Services
{
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
}