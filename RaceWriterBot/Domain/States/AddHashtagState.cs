using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.States
{
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
}
