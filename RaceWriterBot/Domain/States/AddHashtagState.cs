using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.States
{
    public class AddHashtagState : IState
    {
        private readonly User _user;
        private readonly TargetChatId _chatId;

        public AddHashtagState(User user, string[] arguments)
        {
            if (arguments.Length != 3)
                throw new ArgumentException();

            if (!long.TryParse(arguments[2], out var chatId))
                throw new ArgumentException();

            _user = user;
            _chatId = new TargetChatId(chatId);
        }

        public Task ExecuteAsync(string hashtagName)
        {
            var hashtag = new Hashtag(new HashtagName(hashtagName));
            _user.AddHashtag(_chatId, hashtag);
            return Task.CompletedTask;
        }

        public InputRequestType GetRequiredInput() => InputRequestType.HashtagName;
    }
}
