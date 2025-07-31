using RaceWriterBot.Domain.Exceptions;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Models.Entity
{
    public class TargetChat
    {
        private readonly List<Hashtag> _hashtags;
        public string Name { get; private set; }
        public TargetChatId TargetChatId { get; private set; }

        public TargetChat(string name, TargetChatId chatId)
        {
            _hashtags = new List<Hashtag>();
            Name = name;
            TargetChatId = chatId;
        }

        public TargetChat(string name, TargetChatId chatId, params Hashtag[] hashtags)
        {
            _hashtags = hashtags.ToList();
            Name = name;
            TargetChatId = chatId;
        }

        public void AddHashtag(Hashtag hashtag)
        {
            var dublicat = _hashtags.FirstOrDefault(h => h.Name == hashtag.Name);
            if (dublicat != null)
                throw new DublicatHashtagException();

            _hashtags.Add(hashtag);
        }

        public void AddHashtag(HashtagName hashtagName)
        {
            var dublicat = _hashtags.FirstOrDefault(h => h.Name == hashtagName);
            if (dublicat != null)
                throw new DublicatHashtagException();

            _hashtags.Add(new Hashtag(hashtagName));
        }

        public Hashtag GetHashtag(HashtagName name)
        {
            var hashtag = _hashtags.FirstOrDefault(h => h.Name == name);
            if (hashtag == default)
                throw new HashtagNotFoundException();

            return hashtag;
        }

        public HashtagName[] GetHashtagsName()
        {
            return _hashtags.Select(h => h.Name).ToArray();
        }
    }
}