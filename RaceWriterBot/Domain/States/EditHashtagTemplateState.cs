using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.States
{
    internal class EditHashtagTemplateState : IState
    {
        private readonly User _user;
        private readonly TargetChatId _targetChatId;
        private readonly HashtagName _hashtagName;
        public EditHashtagTemplateState(User user, string[] arguments)
        {
            if (arguments.Length != 4)
                throw new ArgumentException();

            if (!long.TryParse(arguments[2], out var chatId))
                throw new ArgumentException();

            _user = user;
            _targetChatId = new TargetChatId(chatId);
            _hashtagName = new HashtagName(arguments[3]);
        }

        public Task ExecuteAsync(StringInput input)
        {
            _user.UpdateHashtagTemplateText(_targetChatId, _hashtagName, input.Value);
            return Task.CompletedTask;
        }

        public InputRequestType GetRequiredInput() => InputRequestType.TemplateText;
    }
}