using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models.Entity;

namespace RaceWriterBot.Application.Interfaces
{
    public interface IStateFactory
    {
        IState CreateFromCallback(string? callbackData, User userId);
    }
}
