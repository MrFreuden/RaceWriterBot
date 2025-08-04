using RaceWriterBot.Application.DTOs;
using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Presentation.Interfaces
{
    public interface IStateService
    {
        MessageDTO HandleCallback(string data, UserId userId); 
    }
}