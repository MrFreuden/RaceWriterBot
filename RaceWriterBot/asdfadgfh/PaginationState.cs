namespace RaceWriterBot.Temp
{
    public class PaginationState
    {
        private readonly Dictionary<long, Dictionary<string, object>> _userPaginations = new();

        public void SavePagination<T>(long userId, string paginationType, Paging<T> paging)
        {
            if (!_userPaginations.TryGetValue(userId, out var paginations))
            {
                paginations = new Dictionary<string, object>();
                _userPaginations[userId] = paginations;
            }

            paginations[paginationType] = paging;
        }

        public Paging<T> GetPagination<T>(long userId, string pagionationType)
        {
            if (_userPaginations.TryGetValue(userId, out var paginations) && 
                paginations.TryGetValue(pagionationType, out var paging) && 
                paging is Paging<T> typedPaging)
            {
                return typedPaging;
            }

            return null;
        }
    }
}
