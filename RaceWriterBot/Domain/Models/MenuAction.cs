namespace RaceWriterBot.Domain.Models
{
    public class MenuAction
    {
        public MenuAction(string text, string callbackData)
        {
            Text = text;
            CallbackData = callbackData;
        }

        public string Text { get; set; }
        public string CallbackData { get; set; }
    }
}
