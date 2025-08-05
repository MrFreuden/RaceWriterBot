using RaceWriterBot.Application.DTOs;
using RaceWriterBot.Domain.Models;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Interfaces
{
    public interface IMenuService
    {
        MessageDTO BuildSimpleMenu(UserId userId, string text, List<MenuAction> actions, int? messageId);
    }
}