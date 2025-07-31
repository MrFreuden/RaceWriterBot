using RaceWriterBot.Domain.Exceptions;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Models.Entity
{
    public class User
    {
        private readonly List<TargetChat> _targetChats;
        public UserId UserId { get; }

        public User(UserId userId)
        {
            UserId = userId;
            _targetChats = new List<TargetChat>();
        }

        public void AddTargetChat(TargetChat targetChat)
        {
            var dublicat = _targetChats.FirstOrDefault(c => c.TargetChatId == targetChat.TargetChatId);
            if (dublicat != null)
                throw new DuplicateChatException();

            _targetChats.Add(targetChat);
        }

        public void AddTargetChat(string name, TargetChatId targetChatId)
        {
            var dublicat = _targetChats.FirstOrDefault(c => c.TargetChatId == targetChatId);
            if (dublicat != null)
                throw new DuplicateChatException();

            _targetChats.Add(new TargetChat(name, targetChatId));
        }

        public void AddHashtag(TargetChatId targetChatId, Hashtag hashtag)
        {
            var targetChat = _targetChats.FirstOrDefault(c => c.TargetChatId == targetChatId) ?? throw new ChatNotFoundException();

            targetChat.AddHashtag(hashtag);
        }

        public TargetChat GetTargetChat(TargetChatId targetChatId)
        {
            var targetChat = _targetChats.FirstOrDefault(c => c.TargetChatId == targetChatId);
            if (targetChat == default)
                throw new ChatNotFoundException();

            return targetChat;
        }

        public IReadOnlyList<TargetChat> GetTargetChats()
        {
            return _targetChats.AsReadOnly(); //TODO: Позже переделать, что бы не возвращать объекты и сохранить целостность
        }

        public void UpdateHashtagTemplateText(TargetChatId targetChatId, HashtagName hashtagName, string newText)
        {
            GetTargetChat(targetChatId).GetHashtag(hashtagName).EditTemplateText(newText);
        }

        public void UpdateHashtagName(TargetChatId targetChatId, HashtagName hashtagName, HashtagName newHashtagName)
        {
            GetTargetChat(targetChatId).GetHashtag(hashtagName).EditHashtagName(newHashtagName);
        }
    }
}
