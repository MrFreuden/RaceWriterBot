using RaceWriterBot.Presentation.Handlers;

namespace RaceWriterBot.Domain.Interfaces
{
    public interface IState
    {
        InputRequestType GetRequiredInput();
        Task ExecuteAsync(string input);
    }
}
