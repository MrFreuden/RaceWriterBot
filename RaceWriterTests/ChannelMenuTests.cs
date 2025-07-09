using Moq;
using RaceWriterBot;
using RaceWriterBot.Temp;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterTests
{
    public class ChannelMenuTests : TestBase
    {
        private Update _settingsMessage;
        private List<TargetChatSession> _testChannels;

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            _settingsMessage = CreateMessageUpdate(Constants.CommandNames.Settings);
            
            _testChannels = [new("Test1", 100)];
            mockUserStorage
               .Setup(s => s.GetTargetChatSessions(testUser.Id))
               .Returns(_testChannels);
            SendTestQuerys();
        }

        private async Task SendTestQuerys()
        {
            await handler.HandleUpdateAsync(dummyBot, _settingsMessage, CancellationToken.None);
            
        }

        [Test]
        public async Task ProcessQuerySelectedChat_WhenChatHasNoHashtag_ShouldSendMessageAndButton()
        {
            var channel = _testChannels.FirstOrDefault();
            var itemQuery = CreateCallbackQueryUpdate($"{Constants.CommandNames.CHANNELS_PAGE}_item_{channel.GetHashCode()}");

            await handler.HandleUpdateAsync(dummyBot, itemQuery, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetHashtagSessions(
                It.Is<long>(userId => userId == testUser.Id),
                It.Is<long>(targetId => targetId == channel.TargetChatId)), 
                Times.Once);

            mockMessenger.Verify(s => s.EditMessageText(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.IsAny<int>(),
                It.Is<string>(text => text.Contains("не має хештегів")),
                It.Is<InlineKeyboardMarkup>(m => m.InlineKeyboard.First().First().Text == "Створити")),
                Times.Once);
        }

        [Test]
        public async Task ProcessQueryAddHashtag_ShouldSaveDialogStateAndSendMessage()
        {
            var channel = _testChannels.First();

            var addHashtagQuery = CreateCallbackQueryUpdate($"{Constants.CommandNames.ACTION_ADD_HASHTAG}_{channel.GetHashCode()}");

            await handler.HandleUpdateAsync(dummyBot, addHashtagQuery, CancellationToken.None);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Введіть новий хештег"))),
                Times.Once);
        }

        [Test]
        public async Task ProcessMessageHastagName_WhenIsContinueDialogState_ShouldAddHashtagAndSendPagingOfHashtags()
        {
            var channel = _testChannels.First();
            var hashtagName = "NewTestHashtag";

            var addHashtagQuery = CreateCallbackQueryUpdate($"{Constants.CommandNames.ACTION_ADD_HASHTAG}_{channel.GetHashCode()}");
            await handler.HandleUpdateAsync(dummyBot, addHashtagQuery, CancellationToken.None);

            var messageWithHashtagName = CreateMessageUpdate(hashtagName);

            mockUserStorage
                .Setup(s => s.AddHashtagSession(
                    It.Is<long>(id => id == testUser.Id),
                    It.Is<long>(id => id == channel.TargetChatId),
                    It.IsAny<HashtagSession>()))
                .Callback<long, long, HashtagSession>((userId, chatId, hashtag) => {
                    channel.AddHashtag(hashtag);
                });

            mockUserStorage
                .Setup(s => s.GetHashtagSessions(testUser.Id, channel.TargetChatId))
                .Returns(() => channel.Hashtags);

            await handler.HandleUpdateAsync(dummyBot, messageWithHashtagName, CancellationToken.None);

            mockUserStorage.Verify(s => s.AddHashtagSession(
                It.Is<long>(id => id == testUser.Id),
                It.Is<long>(id => id == channel.TargetChatId),
                It.Is<HashtagSession>(h => h.HashtagName == hashtagName)),
                Times.Once);

            mockUserStorage.Verify(s => s.GetHashtagSessions(
                It.Is<long>(id => id == testUser.Id),
                It.Is<long>(id => id == channel.TargetChatId)),
                Times.AtLeastOnce);

            mockMessenger.Verify(s => s.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Хештеги для каналу")),
                It.Is<InlineKeyboardMarkup>(m => m.InlineKeyboard.First().First().Text == hashtagName)),
                Times.Once);
        }

        [Test]
        public async Task ProcessQuerySelectedChat_WhenChatHasOneHashtag_ShouldSendPagingOfHashtags()
        {
            var hashtags = new List<HashtagSession> { new() { HashtagName = "TestHashtag" } };
            var hashtag = hashtags.First();
            var channel = _testChannels.First();
            var itemQuery = CreateCallbackQueryUpdate($"{Constants.CommandNames.CHANNELS_PAGE}_item_{channel.GetHashCode()}");

            mockUserStorage
               .Setup(s => s.GetHashtagSessions(testUser.Id, channel.TargetChatId))
               .Returns(hashtags);

            await handler.HandleUpdateAsync(dummyBot, itemQuery, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetHashtagSessions(
                It.Is<long>(userId => userId == testUser.Id),
                It.Is<long>(targetId => targetId == channel.TargetChatId)),
                Times.Once);

            mockMessenger.Verify(s => s.EditMessageText(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.IsAny<int>(),
                It.Is<string>(text => text.Contains("Хештеги для каналу")),
                It.Is<InlineKeyboardMarkup>(m => m.InlineKeyboard.First().First().Text == hashtag.HashtagName)),
                Times.Once);
        }
    }
    
}
