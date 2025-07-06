using Moq;
using RaceWriterBot.asdfadgfh;
using RaceWriterBot.Temp;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace RaceWriterTests
{
    public abstract class TestBase
    {
        protected Mock<IBotMessenger> mockMessenger;
        protected Mock<IUserDataStorage> mockUserStorage;
        protected Mock<IBotDataStorage> mockBotStorage;
        protected IUpdateHandler handler;
        protected readonly ITelegramBotClient dummyBot = new Mock<ITelegramBotClient>().Object;

        protected User testUser;
        protected Chat privateChat;
        protected Chat channelChat;
        protected Chat discussionChat;

        [SetUp]
        public void Setup()
        {
            mockMessenger = new Mock<IBotMessenger>();
            mockUserStorage = new Mock<IUserDataStorage>();
            mockBotStorage = new Mock<IBotDataStorage>();
            var customHandler = new BotHandler(mockMessenger.Object, mockBotStorage.Object, mockUserStorage.Object);
            handler = new UpdateHandlerAdapter(customHandler);

            testUser = new User { Id = 123456 };
            privateChat = new Chat { Id = testUser.Id, Type = ChatType.Private };
            channelChat = new Chat { Id = testUser.Id / 2, Type = ChatType.Channel };
            discussionChat = new Chat { Id = testUser.Id / 3, Type = ChatType.Supergroup };
        }

        protected Update CreateMessageUpdate(string text) =>
            new()
            {
                Message = new Message
                {
                    Text = text,
                    From = testUser,
                    Chat = privateChat
                }
            };

        protected Update CreateCallbackQueryUpdate(string callBackData, Message message = null, int messageId = 1) =>
            new()
            {
                CallbackQuery = new CallbackQuery
                {
                    Data = callBackData,
                    From = testUser,
                    Message = message ?? new Message { Chat = privateChat, Id = messageId }
                }
            };
    }
}
