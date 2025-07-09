using Moq;
using RaceWriterBot;
using RaceWriterBot.Temp;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RaceWriterTests
{
    public class PaginationTests : TestBase
    {
        private const int CountObjectsPerPage = 3;
        private Update _update;
        private List<TargetChatSession> _testChannels;
        private string _paginationPrefix;

        [SetUp]
        public new void Setup()
        {
            base.Setup();
            _update = CreateMessageUpdate(Constants.CommandNames.Settings);
            _testChannels = Enumerable.Range(1, 8)
                .Select(i => new TargetChatSession($"Канал {i}", 100 + i))
                .ToList();
            _paginationPrefix = $"{Constants.CommandNames.CHANNELS_PAGE}_";
            mockUserStorage
                .Setup(s => s.GetTargetChatSessions(testUser.Id))
                .Returns(_testChannels);
        }

        [Test]
        public async Task FirstPage_Should_ShowNextButtonButNotPrevButton()
        {
            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Активні канали")),
                It.Is<InlineKeyboardMarkup>(
                    m => m.InlineKeyboard.Count() >= 3 &&
                    m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("➡️")) &&
                    m.InlineKeyboard.ElementAt(1).Any(btn => !btn.Text.Contains("⬅️")
                    ))),
                Times.Once);
        }

        [Test]
        public async Task MiddlePage_Should_ShowBothNextAndPrevButtons()
        {
            var queryUpdate = CreateCallbackQueryUpdate($"{_paginationPrefix}page_{1}");

            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);
            await handler.HandleUpdateAsync(dummyBot, queryUpdate, CancellationToken.None);



            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.EditMessageReplyMarkup(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<int>(i => i == 1),
                It.Is<InlineKeyboardMarkup>(
                    m => m.InlineKeyboard.Count() >= 3 &&
                    m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("➡️")) &&
                    m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("⬅️")
                    ))),
                Times.Once);
        }

        [Test]
        public async Task LastPage_Should_ShowPrevButtonButNotNextButton()
        {
            var queryUpdate = CreateCallbackQueryUpdate($"{_paginationPrefix}page_{2}");

            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);
            await handler.HandleUpdateAsync(dummyBot, queryUpdate, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.EditMessageReplyMarkup(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<int>(i => i == 1),
                It.Is<InlineKeyboardMarkup>(
                    m => m.InlineKeyboard.Count() >= 3 &&
                    m.InlineKeyboard.ElementAt(1).Any(btn => !btn.Text.Contains("➡️")) &&
                    m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("⬅️")
                    ))),
                Times.Once);
        }

        [Test]
        public async Task FirstPage_Should_ShowCorrectChannels()
        {
            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);

            var expectedChannels = _testChannels.Take(CountObjectsPerPage).ToList();

            mockMessenger.Verify(b => b.SendMessage(
                It.Is<ChatId>(c => c.Identifier == testUser.Id),
                It.Is<string>(text => text.Contains("Активні канали")),
                It.Is<InlineKeyboardMarkup>(
                    m => ValidateChannelButtons(m.InlineKeyboard.ElementAt(0), expectedChannels)
                    )),
                Times.Once);
        }

        [Test]
        public async Task MiddlePage_Navigation_Buttons_Should_HaveCorrectCallbackData()
        {
            var queryUpdate = CreateCallbackQueryUpdate($"{_paginationPrefix}page_{1}");

            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);
            await handler.HandleUpdateAsync(dummyBot, queryUpdate, CancellationToken.None);

            mockMessenger.Verify(b => b.EditMessageReplyMarkup(
                It.IsAny<ChatId>(),
                It.IsAny<int>(),
                It.Is<InlineKeyboardMarkup>(m => CheckMiddlePageNavigation(m))),
                Times.Once);
        }


        [Test]
        public async Task Channel_Buttons_Should_HaveCorrectCallbackData()
        {
            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);

            var expectedChannels = _testChannels.Take(CountObjectsPerPage).ToList();

            mockMessenger.Verify(b => b.SendMessage(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(m => ValidateCallbackChannelButtons(m, expectedChannels))),
                Times.Once);
        }

        [Test]
        public async Task EmptyCollection_Should_NotShowNavigationButtons()
        {
            mockUserStorage
                .Setup(s => s.GetTargetChatSessions(testUser.Id))
                .Returns([]);

            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);

            mockUserStorage.Verify(s => s.GetTargetChatSessions(testUser.Id), Times.Once);

            mockMessenger.Verify(b => b.SendMessage(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(
                    m => m.InlineKeyboard.Count() == 1 && //TODO: Заменить на два, потому что надо учитывать кнопку возврата
                    m.InlineKeyboard.ElementAt(0).Any(btn => !btn.Text.Contains("➡️")) &&
                    m.InlineKeyboard.ElementAt(0).Any(btn => !btn.Text.Contains("⬅️") //TODO: Убрать выбор элемента по индексу
                    ))),
                Times.Once);
        }

        [Test]
        public async Task Pagination_Should_UseCorrectPageSize()
        {
            var manyChannels = Enumerable.Range(1, CountObjectsPerPage * 2 + 1)
                .Select(i => new TargetChatSession($"Канал {i}", 100 + i))
                .ToList();

            mockUserStorage
                .Setup(s => s.GetTargetChatSessions(testUser.Id))
                .Returns(manyChannels);

            await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);

            var expectedFirstPageChannels = manyChannels.Take(CountObjectsPerPage).ToList();
            mockMessenger.Verify(b => b.SendMessage(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.Is<InlineKeyboardMarkup>(m =>
                    m.InlineKeyboard.ElementAt(0).Count() == CountObjectsPerPage &&
                    ValidateChannelButtons(m.InlineKeyboard.ElementAt(0), expectedFirstPageChannels)
                )),
                Times.Once);
        }
        //TODO: Раскоментить после реализации возврата
        //[Test]
        //public async Task PaginationState_Should_SaveAndRetrievePagination()
        //{
        //    await handler.HandleUpdateAsync(dummyBot, _update, CancellationToken.None);

        //    var queryUpdatePage1 = CreateCallbackQueryUpdate($"{_paginationPrefix}page_{1}");
        //    await handler.HandleUpdateAsync(dummyBot, queryUpdatePage1, CancellationToken.None);

        //    mockMessenger.Verify(b => b.EditMessageReplyMarkup(
        //        It.IsAny<ChatId>(),
        //        It.IsAny<int>(),
        //        It.Is<InlineKeyboardMarkup>(m =>
        //            m.InlineKeyboard.Count() >= 3 &&
        //            m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("➡️")) &&
        //            m.InlineKeyboard.ElementAt(1).Any(btn => btn.Text.Contains("⬅️"))
        //        )),
        //        Times.Once);

        //    var channelUpdate = CreateCallbackQueryUpdate($"{_paginationPrefix}item_{_testChannels[0].GetHashCode()}");
        //    await handler.HandleUpdateAsync(dummyBot, channelUpdate, CancellationToken.None);

        //    var backUpdate = CreateCallbackQueryUpdate($"{Constants.CommandNames.HASHTAGS_PAGE}_{Constants.CommandNames.ACTION_BACK}");
        //    await handler.HandleUpdateAsync(dummyBot, backUpdate, CancellationToken.None);

        //    mockMessenger.Verify(b => b.EditMessageText(
        //        It.IsAny<ChatId>(),
        //        It.IsAny<int>(),
        //        It.Is<string>(s => s.Contains("Активні канали")),
        //        It.IsAny<InlineKeyboardMarkup>()),
        //        Times.AtLeastOnce);
        //}

        private bool ValidateCallbackChannelButtons(InlineKeyboardMarkup markup, List<TargetChatSession> expectedChannels)
        {
            if (markup.InlineKeyboard.Count() < 2)
                return false;
            var itemsRow = markup.InlineKeyboard.ElementAt(0);
            var buttons = itemsRow.ToList();
            if (buttons.Count != expectedChannels.Count)
                return false;

            for (int i = 0; i < buttons.Count; i++)
            {
                var channel = expectedChannels[i];
                var button = buttons[i];

                if (button.Text != channel.Name)
                    return false;

                var expectedCallback = $"{_paginationPrefix}item_{channel.GetHashCode()}";
                if (button.CallbackData != expectedCallback)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateChannelButtons(IEnumerable<InlineKeyboardButton> buttons, List<TargetChatSession> expectedChannels)
        {
            var buttonsList = buttons.ToList();
            if (buttonsList.Count != expectedChannels.Count)
                return false;

            for (int i = 0; i < buttonsList.Count; i++)
            {
                if (buttonsList[i].Text != expectedChannels[i].Name)
                    return false;
            }

            return true;
        }

        private bool CheckMiddlePageNavigation(InlineKeyboardMarkup markup)
        {
            if (markup.InlineKeyboard.Count() < 2)
                return false;

            var navigationRow = markup.InlineKeyboard.ElementAt(1);

            var nextButton = navigationRow.FirstOrDefault(b => b.Text.Contains(Constants.CommandNames.Next));
            if (nextButton == null || nextButton.CallbackData != $"{_paginationPrefix}page_2")
                return false;

            var prevButton = navigationRow.FirstOrDefault(b => b.Text.Contains(Constants.CommandNames.Prev));
            if (prevButton == null || prevButton.CallbackData != $"{_paginationPrefix}page_0")
                return false;

            return true;
        }
    }
}
