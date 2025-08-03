using RaceWriterBot.Domain.ValueObjects;

namespace RaceWriterBot.Domain.Models.Entity
{
    public class Hashtag
    {
        private const string _defaultTemplateText = "18:20 - 0 вільних місць\r\n18:35 - 0 вільних місць\r\n18:55 - 0 вільних місць";
        public HashtagName Name { get; private set; }
        public string TemplateText { get; private set; }


        public Hashtag(HashtagName name)
        {
            Name = name;
            TemplateText = _defaultTemplateText;
        }

        public Hashtag(HashtagName name, string templateText)
        {
            Name = name;
            TemplateText = templateText;
        }

        public void EditHashtagName(HashtagName name)
        {
            Name = name;
        }

        public void EditTemplateText(string templateText)
        {
            if (string.IsNullOrEmpty(templateText))
                throw new ArgumentNullException(nameof(templateText));

            TemplateText = templateText;
        }
    }
}