namespace RaceWriterBot
{
    public static class Constants
    {
        public static class Emoji
        {
            public const string Eyes = "👀";
            public const string Heart = "❤️";
            public const string ThumbDown = "👎";
        }

        public static class CommandNames
        {
            public const string Start = "/start";
            public const string Settings = "/settings";
            public const string Like = Emoji.Heart;
            public const string Dislike = Emoji.ThumbDown;
            public const string ACTION_BACK = "back";
            public const string CHANNELS_PAGE = "channels";
            public const string HASHTAGS_PAGE = "hashtags";
            public const string MESSAGES_PAGE = "messages";
            public const string ACTION_EDIT_HASHTAG_TEMPLATE = "edit_hashtag_template";
            public const string ACTION_ADD_HASHTAG = "add_hashtag";
            public const string ACTION_EDIT_HASHTAG = "edit_hashtag";
        }
    }
}
