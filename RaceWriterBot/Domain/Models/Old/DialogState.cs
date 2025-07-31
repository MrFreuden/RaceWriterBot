using RaceWriterBot.Interfaces;

namespace RaceWriterBot.Domain.Models.Old
{
    public class DialogState<T> : IDialogState
    {
        public string ExpectedAction { get; set; }
        public T Context { get; set; }
    }
}