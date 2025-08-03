using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Presentation.Handlers;
using RaceWriterBot.Presentation.Interfaces;
using static RaceWriterBot.Constants;

namespace RaceWriterBot.Presentation.Services
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

    public class EditHashtagState : IState
    {
        private readonly IUserRepository _userRepository;
        private readonly UserId _userId;
        private readonly TargetChatId _chatId;
        private readonly HashtagName _hashtagName;

        public EditHashtagState(IUserRepository userRepository, UserId userId, string[] arguments)
        {
            if (arguments.Length != 4)
                throw new ArgumentException();

            if (!long.TryParse(arguments[2], out var chatId))
                throw new ArgumentException();

            _userId = userId;
            _chatId = new TargetChatId(chatId);
            _hashtagName = new HashtagName(arguments[3]);
            _userRepository = userRepository;
        }

        public Task ExecuteAsync(string newHashtagName)
        {
            var user = _userRepository.GetUser(_userId);
            user.UpdateHashtagName(_chatId, _hashtagName, new HashtagName(newHashtagName));
            return Task.CompletedTask;
        }

        public InputRequestType GetRequiredInput() => InputRequestType.HashtagName;
    }

    public class AddHashtagState : IState
    {
        private readonly IUserRepository _userRepository;
        private readonly UserId _userId;
        private readonly TargetChatId _chatId;

        public AddHashtagState(IUserRepository userRepository, UserId userId, string[] arguments)
        {
            if (arguments.Length != 3)
                throw new ArgumentException();

            if (!long.TryParse(arguments[2], out var chatId))
                throw new ArgumentException();

            _userId = userId;
            _chatId = new TargetChatId(chatId);
            _userRepository = userRepository;
        }

        public Task ExecuteAsync(string hashtagName)
        {
            var user = _userRepository.GetUser(_userId);
            var hashtag = new Hashtag(new HashtagName(hashtagName));
            user.AddHashtag(_chatId, hashtag);
            return Task.CompletedTask;
        }

        public InputRequestType GetRequiredInput() => InputRequestType.HashtagName;
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
