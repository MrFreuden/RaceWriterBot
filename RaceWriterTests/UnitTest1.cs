
using Moq;
using RaceWriterBot.asdfadgfh;
using RaceWriterBot.Temp;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace RaceWriterTests
{
    public class Tests
    {
        private Mock<IBotMessenger> mockMessenger;
        private Mock<IUserDataStorage> mockUserStorage;
        private Mock<IBotDataStorage> mockBotStorage;
        private IUpdateHandler handler;
        private readonly ITelegramBotClient dummyBot = new Mock<ITelegramBotClient>().Object;

        private User testUser;
        private Chat privateChat;
        private Chat channelChat;
        private Chat discussionChat;

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

        [Test]
        public async Task ProcessStartCommand_ShouldSendWelcomeMessageAndAddUserToDatabase()
        {
            var update = new Update
            {
                Message = new Message()
                {
                    Text = "/start",
                    From = testUser,
                    Chat = privateChat,
                },
            };

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Ласкаво просимо"))),
                Times.Once);

            mockUserStorage.Verify(s => s.AddNewUser(testUser.Id), Times.Once);
        }

        [Test]
        public async Task ProcessSettingsCommand_WhenUserHasNoActive_ShouldSendMessageAndButton()
        {
            var update = new Update
            {
                Message = new Message()
                {
                    Text = "/settings",
                    From = testUser,
                    Chat = privateChat,
                },
            };

            mockUserStorage
                .Setup(s => s.GetTargetChatSessions(testUser.Id))
                .Returns(new List<TargetChatSession>());

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("У вас немає активних каналів")),
                It.IsAny<ReplyMarkup>()),
                Times.Once);
        }

        [Test]
        public async Task ProcessCreateTargetChatQuery_s()
        {
            var update = new Update
            {
                CallbackQuery = new CallbackQuery
                {
                    Data = "CreateTargetChatSession",
                    From = testUser
                },
            };

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => !string.IsNullOrEmpty(text)),
                It.IsAny<ReplyMarkup>()),
                Times.Once);
        }

        [Test]
        public async Task UserAddBotToAdmins_s()
        {
            var update = new Update
            {
                MyChatMember = new ChatMemberUpdated
                {
                    NewChatMember = new ChatMemberAdministrator
                    {
                        User = testUser
                    },
                    From = testUser,
                    Chat = channelChat
                },
            };

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockBotStorage.Verify(b => b.AddTargetChatId(channelChat.Id), Times.Once);
        }

        [Test]
        public async Task UserConfirm()
        {
            var update = new Update
            {
                CallbackQuery = new CallbackQuery
                {
                    Data = "UserConfirmAddingBotToTargetChat",
                    From = testUser
                },
            };

            mockBotStorage.Setup(b => b.AddTargetChatId(channelChat.Id));

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);


            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => !string.IsNullOrEmpty(text))),
                Times.Once);
        }

        
    }
    
}
