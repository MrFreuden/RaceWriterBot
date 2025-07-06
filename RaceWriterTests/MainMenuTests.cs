using Moq;
using RaceWriterBot.Temp;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace RaceWriterTests
{
    public class MainMenuTests : TestBase
    {
        [Test]
        public async Task Start_Command_Should_AddUserAndSendWelcomeMessage()
        {
            var update = CreateMessageUpdate("/start");

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockUserStorage.Verify(s => s.AddNewUser(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Ласкаво просимо"))),
                Times.Once);

        }

        [Test]
        public async Task Settings_Command_WithNoActiveChannels_Should_ShowCreateButton()
        {
            var update = CreateMessageUpdate("/settings");

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
        public async Task CreateTargetChat_Callback_Should_ShowInstructions()
        {
            var update = CreateCallbackQueryUpdate("CreateTargetChatSession");

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => !string.IsNullOrEmpty(text)),
                It.IsAny<ReplyMarkup>()),
                Times.Once);
        }

        [Test]
        public async Task BotAddedAsAdmin_Should_RegisterTargetChatId()
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
        public async Task UserConfirm_BotAdded_Should_RequestForwardedMessage()
        {
            var update = CreateCallbackQueryUpdate("UserConfirmAddingBotToTargetChat");

            mockBotStorage.Setup(b => b.AddTargetChatId(channelChat.Id));

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => !string.IsNullOrEmpty(text))),
                Times.Once);
        }

        [Test]
        public async Task Settings_Command_WithOneActiveChannel_Should_ShowChannelsList()
        {
            var update = CreateMessageUpdate("/settings");

            var targetSession = new TargetChatSession("Тестовый канал", 100);
            
            mockUserStorage
                .Setup(s => s.GetTargetChatSessions(testUser.Id))
                .Returns(new List<TargetChatSession> { targetSession });

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Активні канали")),
                It.Is<InlineKeyboardMarkup>(m => m.InlineKeyboard.First().First().Text == "Тестовый канал")),
                Times.Once);
        }

        [Test]
        public async Task Settings_Command_WithMultipleChannels_Should_ShowPaginatedList()
        {
            var update = CreateMessageUpdate("/settings");

            var channels = Enumerable.Range(1, 6)
                .Select(i => new TargetChatSession($"Канал {i}", 100 + i))
                .ToList();

            mockUserStorage
                .Setup(s => s.GetTargetChatSessions(testUser.Id))
                .Returns(channels);

            await handler.HandleUpdateAsync(dummyBot, update, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Активні канали")),
                It.Is<InlineKeyboardMarkup>(m => m.InlineKeyboard.Count() >= 3 &&
                m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("Вперед")))),
                Times.Once);
        }
    }
    
}
