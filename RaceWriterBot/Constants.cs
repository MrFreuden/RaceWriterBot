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
            public const string ACTION_BACK = "back";
            public const string ACTION_EDIT_HASHTAG_TEMPLATE = "edit_hashtag_template";
            public const string ACTION_ADD_HASHTAG = "add_hashtag";
            public const string ACTION_EDIT_HASHTAG = "edit_hashtag";
            public const string ACTION_CREATE_TARGET_CHAT = "CreateTargetChatSession";
            public const string ACTION_CONFIRMATION_ADDING_BOT = "UserConfirmAddingBotToTargetChat";
        }
    }
}
