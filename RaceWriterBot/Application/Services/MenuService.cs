using RaceWriterBot.Application.DTOs;
using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Application.Services
{
    public class MenuService : IMenuService
    {
        public MessageDTO BuildSimpleMenu(UserId userId, string text, List<MenuAction> actions, int? messageId)
        {
            if (messageId == null) 
                messageId = 0;

            return new MessageDTO { Actions = actions, MessageId = messageId, Text = text};
        }

        public MessageDTO BuildPaginatedMenu(
            UserId userId, string text, PaginatedResult<MenuAction> paginatedActions, string callbackPrefix, int messageId = 0)
        {
            var actions = new List<MenuAction>(paginatedActions.Items);

            var navigationButtons = new List<MenuAction>();

            if (paginatedActions.HasPrevious)
                navigationButtons.Add(new MenuAction("← Назад", $"{callbackPrefix}_page_{paginatedActions.CurrentPage - 1}"));

            if (paginatedActions.HasNext)
                navigationButtons.Add(new MenuAction("Вперед →", $"{callbackPrefix}_page_{paginatedActions.CurrentPage + 1}"));

            if (navigationButtons.Any())
                actions.AddRange(navigationButtons);

            var pageInfo = $"{text}\nСтраница {paginatedActions.CurrentPage} из {paginatedActions.TotalPages}";

            return new MessageDTO { Actions = actions, MessageId = messageId, Text = pageInfo, UserId = userId };
        }
    }


    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
    }

    public class PaginationService : IPaginationService
    {
        public PaginatedResult<T> GetPage<T>(List<T> allItems, int page, int pageSize)
        {
            var totalPages = (int)Math.Ceiling(allItems.Count / (double)pageSize);
            var items = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedResult<T>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }
    }

    public interface IPaginationService
    {
        PaginatedResult<T> GetPage<T>(List<T> allItems, int page, int pageSize);
    }
}
