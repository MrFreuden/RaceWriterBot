using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Models.Entity
{
    public class Event
    {
        private List<TimeSlot> _slots;

        public Event(Guid id, string title, ChannelId channelId, MessageId messageId, HashtagName hashtagName)
        {
            Id = id;
            Title = title;
            ChannelId = channelId;
            MessageId = messageId;
            HashtagName = hashtagName;
            _slots = new List<TimeSlot>();
        }

        public Guid Id { get; }
        public string Title { get; private set; }
        public ChannelId ChannelId { get; }
        public MessageId MessageId { get; }
        public HashtagName HashtagName { get; }

        public void AddSlot(TimeSlot slot)
        {
            ArgumentNullException.ThrowIfNull(slot);
            if (IsSlotCanBeAdded(slot))
            {
                _slots.Add(slot);
            }
            else
            {
                throw new ArgumentException(slot.Time.ToString());
            }
        }

        public bool IsSlotCanBeAdded(TimeSlot slot)
        {
            return _slots.FirstOrDefault(s => s.Time == slot.Time) == null;
        }

        public void RemoveSlot(TimeSlot slot)
        {
            _slots.Remove(slot);
        }
    }
}
