namespace RaceWriterBot
{
    public static class Constants
    {
        public static class Emoji
        {
            public const string Eyes = "👀";
            public const string Heart = "❤️";
            public const string ThumbDown = "👎";
            public const string RightArrow = "➡️";
            public const string LeftArrow = "⬅️";
        }

        public static class CommandNames
        {
            public const string Start = "/start";
            public const string Settings = "/settings";
            public const string Like = Emoji.Heart;
            public const string Dislike = Emoji.ThumbDown;
            public const string Next = Emoji.RightArrow;
            public const string Prev = Emoji.LeftArrow;
        }
    }
}
