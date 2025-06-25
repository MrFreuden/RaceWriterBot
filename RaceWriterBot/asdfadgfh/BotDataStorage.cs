
namespace RaceWriterBot.Temp
{
    public class BotDataStorage : IBotDataStorage
    {
        private readonly Dictionary<long, long> _keyValuePairs = new Dictionary<long, long>();
        public bool AddOwner(long ownerId, long targetChatId)
        {
            if (_keyValuePairs.TryGetValue(targetChatId, out long val))
            {
                if (val == default)
                {
                    _keyValuePairs[targetChatId] = ownerId;
                    return true;
                }
            }
            return false;
        }

        public void AddTargetChatId(long targetChatId)
        {
            _keyValuePairs.Add(targetChatId, default);
        }
    }
    
    public interface IBotDataStorage
    {
        bool AddOwner(long ownerId, long targetChatId);
        void AddTargetChatId(long targetChatId);
    }
}