using RaceWriterBot.Interfaces;

namespace RaceWriterBot.Models
{
    public class DialogState<T> : IDialogState
    {
        public string ExpectedAction { get; set; }
        public T Context { get; set; }
    }
}