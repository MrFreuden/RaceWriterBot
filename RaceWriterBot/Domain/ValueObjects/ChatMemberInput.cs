using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models;
using Telegram.Bot.Types;

namespace RaceWriterBot.Domain.ValueObjects
{
    public class ChatMemberInput : IStateInput
    {
        public UserId UserId { get; }
        public TargetChatId TargetChatId { get; }
        public bool Status { get; }

        public ChatMemberInput(UserId userId, TargetChatId targetChatId)
        {
            UserId = userId;
            TargetChatId = targetChatId;
        }

        public Type GetInputType() => typeof(ChatMemberInput);
    }
}
