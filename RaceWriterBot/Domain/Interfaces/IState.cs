using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.Interfaces
{
    public interface IState
    {
        InputRequestType GetRequiredInput();
        Task ExecuteAsync(string input);
    }
}
