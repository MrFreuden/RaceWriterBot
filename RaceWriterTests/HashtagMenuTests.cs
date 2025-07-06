namespace RaceWriterTests
{
    public class HashtagMenuTests : TestBase
    {
        [Test]
        public async Task ProcessQuerySelectedHashtag_ShouldSendDefaultTextTemplateAndMenuButtonsEditHashtagNameAndEditTextTemplate()
        {

        }

        [Test]
        public async Task ProcessQuerySelectedButtonEditTextTemplate_ShouldSendMessageAndSaveDialogState()
        {

        }

        [Test]
        public async Task ProcessMessageTextTemplate_WhenTextIsCorrect_ShouldEditTextTemplateAndBackPrevMenu()
        {

        }

        [Test]
        public async Task ProcessMessageTextTemplate_WhenTextIsIncorrect_ShouldSendMessageAndRetry()
        {

        }

        [Test]
        public async Task ProcessQuerySelectedButtonEditHashtagName_WhenNameIsUnique_ShouldEditHashtagNameAndBackPrevMenu()
        {

        }

        [Test]
        public async Task ProcessQuerySelectedButtonEditHashtagName_WhenNameIsNonUnique_ShouldSendMessageAndRetry()
        {

        }
    }
    
}
