using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.States
{
    public class StateFactory : IStateFactory
    {
        private readonly IUserRepository _userRepository;

        public StateFactory(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IState CreateFromCallback(string? callbackData, UserId userId)
        {
            if (callbackData == null || string.IsNullOrEmpty(callbackData))
                throw new ArgumentException("Некорректные данные запроса");

            var parts = callbackData.Split('_');
            if (parts.Length < 2)
                throw new ArgumentException("Неверный формат данных команды");
            var stateType = parts[1];

            return stateType switch
            {
                CommandNames.EDIT_HASHTAG => new EditHashtagState(_userRepository, userId, parts),
                CommandNames.ADD_HASHTAG => new AddHashtagState(_userRepository, userId, parts),

                _ => throw new ArgumentException($"Неизвестная команда: {stateType}")
            };
        }
    }

    public static class CommandNames
    {
        public const string Start = "/start";
        public const string Settings = "/settings";
        public const string BACK = "back";
        public const string EDIT_HASHTAG_TEMPLATE = "EditHashtagTemplate";
        public const string ADD_HASHTAG = "AddHashtag";
        public const string EDIT_HASHTAG = "EditHashtag";
        public const string CREATE_TARGET_CHAT = "CreateTargetChat";
        public const string CONFIRMATION_ADDING_BOT = "UserConfirmAddingBotToTargetChat";
    }
}
