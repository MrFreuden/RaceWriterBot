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

        [Test]
        public async Task Navigation_Buttons_Should_HaveCorrectCallbackData()
        {
            // TODO: Реализовать проверку что кнопки навигации имеют правильные callback данные
        }

        [Test]
        public async Task Channel_Buttons_Should_HaveCorrectCallbackData()
        {
            // TODO: Реализовать проверку что кнопки каналов имеют правильные callback данные
        }

        [Test]
        public async Task EmptyCollection_Should_NotShowNavigationButtons()
        {
            // TODO: Реализовать проверку что при пустом списке каналов не отображаются кнопки навигации
        }

        [Test]
        public async Task Pagination_Should_UseCorrectPageSize()
        {
            // TODO: Реализовать проверку что пагинация использует правильный размер страницы
        }

        [Test]
        public async Task PaginationState_Should_SaveAndRetrievePagination()
        {
            // TODO: Реализовать проверку что состояние пагинации сохраняется и извлекается корректно
        }
    }
}
