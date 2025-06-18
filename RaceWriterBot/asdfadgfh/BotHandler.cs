using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace RaceWriterBot.Temp
{
    public class BotHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _bot;
        private UpdateProcessor _updateProcessor;

        public BotHandler(ITelegramBotClient botClient)
        {
            _bot = botClient;
            _updateProcessor = new UpdateProcessor(_bot);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await (update switch
            {
                //{ ChannelPost: { } post } => _updateProcessor.ProcessPost(post),
                { Message: { } message } => _updateProcessor.ProcessMessage(message),
                //{ EditedMessage: { } message } => _updateProcessor.ProcessEditMessage(message),
                { MyChatMember: { } myChatMember } => _updateProcessor.ProcessChatMember(myChatMember),
                { CallbackQuery: { } callbackQuery } => _updateProcessor.ProcessCallbackQuery(callbackQuery),
                //{ InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
                //{ ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
                _ => UnknownUpdateHandlerAsync(update)
            });
        }

        private async Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            await Task.Delay(500);
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            await Task.Delay(2000, cancellationToken);
        }
    }
}