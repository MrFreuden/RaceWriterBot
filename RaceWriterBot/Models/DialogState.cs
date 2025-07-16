namespace RaceWriterBot.Models
{
    public interface IDialogState
    {
        string ExpectedAction { get; set; }
    }
    public class DialogState<T> : IDialogState
    {
        public string ExpectedAction { get; set; }
        public T Context { get; set; }
    }
}