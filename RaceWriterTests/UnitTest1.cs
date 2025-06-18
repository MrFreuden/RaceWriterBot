using Moq;
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
        private Mock<ITelegramBotClient> mockBot;
        private IUpdateHandler handler;

        [SetUp]
        public void Setup()
        {
            mockBot = new Mock<ITelegramBotClient>();
            handler = new BotHandler(mockBot.Object);
        }

        [Test]
        public async Task Test1()
        {
            var user = new User()
            {
                Id = 123456,
            };
            var chat = new Chat()
            {
                Id = user.Id,
                Type = Telegram.Bot.Types.Enums.ChatType.Private
            };
            var update = new Update
            {
                Message = new Message()
                {
                    Text = "/start",
                    From = user,
                    Chat = chat,
                },
            };

            await handler.HandleUpdateAsync(mockBot.Object, update, CancellationToken.None);


            mockBot.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == 123),
                It.Is<string>(text => text.Contains("Добро пожаловать")),
                It.IsAny<ParseMode>(),
                It.IsAny<ReplyParameters>(),
                It.IsAny<ReplyMarkup>(),
                It.IsAny<LinkPreviewOptions>(),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<MessageEntity>>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

        }
    }
}