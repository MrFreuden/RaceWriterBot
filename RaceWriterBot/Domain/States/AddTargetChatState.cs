using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.States
{
    public class AddTargetChatState : IState
    {
        private readonly User _user;

        public AddTargetChatState(User user)
        {
            _user = user;
        }

        public Task ExecuteAsync(string input)
        {
            throw new NotImplementedException();
        }

        public InputRequestType GetRequiredInput() => InputRequestType.AddBotToChat;
    }
}