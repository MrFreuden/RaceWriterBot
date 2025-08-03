using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Infrastructure;

namespace RaceWriterBot.Presentation.Interfaces
{
    public interface IStateService
    {
        MessageDTO HandleCallback(string data, UserId userId); 
    }
}