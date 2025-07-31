using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace RaceWriterBot.Infrastructure.Handlers
{
    public class BotHandler : IUpdateHandler
    {
        private IMessageHandler _updateProcessor;
        private ICallbackQueryHandler _callbackQueryHandler;

        public BotHandler(IMessageHandler messageHandler, ICallbackQueryHandler callbackQueryHandler)
        {
            _updateProcessor = messageHandler;
            _callbackQueryHandler = callbackQueryHandler;
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
                { CallbackQuery: { } callbackQuery } => _callbackQueryHandler.ProcessCallbackQuery(callbackQuery),
                //{ InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
                //{ ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
                _ => UnknownUpdateHandlerAsync(update)
            });
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            await Task.Delay(2000, cancellationToken);
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            await Task.Delay(500);
        }
    }
}