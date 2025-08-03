using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.States
{
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
}
