using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Models
{
    public class ChatMember
    {
        public UserId UserId { get; }
        public TargetChatId TargetChatId { get; }
        public bool Status { get; }

        private ChatMember(UserId userId, TargetChatId targetChatId, bool status)
        {
            UserId = userId;
            TargetChatId = targetChatId;
            Status = status;
        }

        public static ChatMember Create(UserId userId, TargetChatId targetChatId, bool status)
        {
            return new ChatMember(userId, targetChatId, status);
        }
    }
}
