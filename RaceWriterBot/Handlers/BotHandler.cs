using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace RaceWriterBot.Handlers
{
    public interface ICustomUpdateHandler
    {
        Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
        Task HandleErrorAsync(Exception exception, HandleErrorSource source, CancellationToken cancellationToken);
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
        Task UnknownUpdateHandlerAsync(Update update);
    }

    public class UpdateHandlerAdapter : IUpdateHandler
    {
        private readonly ICustomUpdateHandler _handler;

        public UpdateHandlerAdapter(ICustomUpdateHandler handler)
        {
            _handler = handler;
        }

        public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            return _handler.HandleUpdateAsync(update, cancellationToken);
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return _handler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            return _handler.HandleErrorAsync(exception, source, cancellationToken);
        }
    }

    public class BotHandler : ICustomUpdateHandler
    {
        private UpdateProcessor _updateProcessor;
        private CallbackQueryHandler _callbackQueryHandler;

        public BotHandler(IBotMessenger botMessenger, IBotDataStorage botDataStorage, IUserDataStorage userDataStorage, IViewManager viewManager, MenuManager menuManager, IDialogProcessor dialogProcessor)
        {
            _updateProcessor = new UpdateProcessor(botMessenger, userDataStorage, botDataStorage, viewManager);
            _callbackQueryHandler = new CallbackQueryHandler(userDataStorage, botDataStorage, botMessenger, dialogProcessor, viewManager, menuManager);
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
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

        public async Task HandleErrorAsync(Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
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
    //public class BotHandler : IUpdateHandler
    //{
    //    private readonly ITelegramBotClient _bot;
    //    private UpdateProcessor _updateProcessor;

    //    public BotHandler(ITelegramBotClient botClient)
    //    {
    //        _bot = botClient;
    //        _updateProcessor = new UpdateProcessor(new BotMessenger(botClient));
    //    }

    //    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    //    {
    //        cancellationToken.ThrowIfCancellationRequested();
    //        await (update switch
    //        {
    //            //{ ChannelPost: { } post } => _updateProcessor.ProcessPost(post),
    //            { Message: { } message } => _updateProcessor.ProcessMessage(message),
    //            //{ EditedMessage: { } message } => _updateProcessor.ProcessEditMessage(message),
    //            { MyChatMember: { } myChatMember } => _updateProcessor.ProcessChatMember(myChatMember),
    //            { CallbackQuery: { } callbackQuery } => _updateProcessor.ProcessCallbackQuery(callbackQuery),
    //            //{ InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
    //            //{ ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
    //            _ => UnknownUpdateHandlerAsync(update)
    //        });
    //    }

    //    private async Task UnknownUpdateHandlerAsync(Update update)
    //    {
    //        Console.WriteLine($"Unknown update type: {update.Type}");
    //        await Task.Delay(500);
    //    }

    //    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine(exception);
    //        await Task.Delay(2000, cancellationToken);
    //    }
    //}
}