using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Application.Interfaces
{
    public interface IStateFactory
    {
        IState CreateFromCallback(string? callbackData, UserId userId);
    }
}
