using RaceWriterBot.Domain.Models;
using RaceWriterBot.Domain.Models.Old;
using RaceWriterBot.Enums;
using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using System;
using System.Net.Sockets;
using Telegram.Bot.Types;

namespace RaceWriterBot.Infrastructure.Handlers
{
    public class CallbackQueryHandler
    {
        private readonly IUserDataStorage _userDataStorage;
        private readonly IBotDataStorage _botStorage;
        private readonly IBotMessenger _botMessenger;
        private readonly IDialogProcessor _dialogProcessor;
        private readonly IViewManager _viewManager;
        private readonly MenuManager _menuManager;

        public CallbackQueryHandler(IUserDataStorage userDataStorage, IBotDataStorage botStorage, IBotMessenger botMessenger, IDialogProcessor dialogProcessor, IViewManager viewManager, MenuManager menuManager)
        {
            _userDataStorage = userDataStorage;
            _botStorage = botStorage;
            _botMessenger = botMessenger;
            _dialogProcessor = dialogProcessor;
            _viewManager = viewManager;
            _menuManager = menuManager;
        }

        public async Task ProcessCallbackQuery(CallbackQuery query)
        {
            if (!string.IsNullOrEmpty(query.Data))
            {
                var parts = query.Data.Split('_');
                var type = ParseCallbackType(parts);
                if (parts.Length <= 1)
                {
                    throw new ArgumentException();
                }
                parts = parts.Skip(1).ToArray();
                switch (type)
                {
                    case CallbackType.Command:
                        var cb = ParseCallbackDataAsCommand(parts, query.From.Id);
                        await RouteToHandler(cb);
                        break;

                    case CallbackType.Paging:
                        var cb1 = ParseCallbackDataAsPaging(parts, query.From.Id);
                        await HandlePagination(cb1);
                        break;

                    default:
                        break;
                }
            }
        }

        private CallbackType ParseCallbackType(string[] parts)
        {
            if (Enum.TryParse(parts.First(), out CallbackType type))
            {
                return type;
            }
            throw new ArgumentException();
        }

        private ParsedCallback ParseCallbackDataAsCommand(string[] query, long userId)
        {
            var parameter = query.Length > 1 ? query[1] : null;

            if (Enum.TryParse(query.First(), out CallbackAction action))
            {
                return new ParsedCallback { Action = action, Parameter = parameter, UserId = userId };
            }

            throw new ArgumentException();
        }

        private ParsedCallback ParseCallbackDataAsPaging(string[] query, long userId)
        {
            if (query.Length != 3)
            {
                throw new ArgumentException();
            }
            if (Enum.TryParse(query[0], out PageType pageType) && Enum.TryParse(query[1], out PaginationAction action))
            {
                var parameter = query[2];
                return new ParsedCallback { PageType = pageType, PaginationAction = action, Parameter = parameter, UserId = userId };
            }
            throw new ArgumentException();
        }

        private async Task RouteToHandler(ParsedCallback callback)
        {
            switch (callback.Action)
            {
                case CallbackAction.Back:
                    await _menuManager.NavigateBack(callback.UserId);
                    break;
                case CallbackAction.EditHashtagTemplate:
                    await _viewManager.StartEditHashtagTemplate(callback.UserId, callback.Parameter);
                    break;
                case CallbackAction.AddHashtag:
                    if (long.TryParse(callback.Parameter, out var channelId))
                    {
                        await _viewManager.AddNewHashtag(callback.UserId, channelId);
                    }
                    break;
                case CallbackAction.CreateTargetChat:
                    await _viewManager.AddBotToTargetChatSettings(callback.UserId);
                    break;
                case CallbackAction.AddBot:
                    await _viewManager.RequestForwardedMessage(callback.UserId);
                    break;
                case CallbackAction.EditHashtagName:
                    // Обработка изменения имени хэштега
                    // Требуется реализация метода в _viewManager
                    break;
                case CallbackAction.Unknown:
                default:
                    // Обработка неизвестного действия
                    throw new NotImplementedException();
                    break;
            }
        }

        private async Task HandlePagination(ParsedCallback callback)
        {
            var user = _userDataStorage.GetUser(callback.UserId);
            switch (callback.PaginationAction)
            {
                case PaginationAction.Item:
                    switch (callback.PageType)
                    {
                        case PageType.Channels:
                            await _menuManager.HandleActionItem<TargetChatSession>(
                                callback.UserId, user.GetTargetChatSession(long.Parse(callback.Parameter)),
                                (session) => _viewManager.ShowHashtags(callback.UserId, session));
                            break;

                        case PageType.Hashtags:
                            await _menuManager.HandleActionItem<HashtagSession>(
                                callback.UserId, user.GetHashtagSession(callback.Parameter),
                                (hashtag) => _viewManager.ShowTemplateMessage(callback.UserId, hashtag));
                            break;

                        case PageType.Messages:
                            await _menuManager.HandleActionItem(
                                callback.UserId, new PostMessagePair(),
                                (pair) => _viewManager.ShowMessageDetails(callback.UserId, pair));
                            break;
                    }
                    break;

                case PaginationAction.Page:
                    var pageNumber = int.Parse(callback.Parameter);
                    await _menuManager.HandleActionPage(callback.UserId, pageNumber);
                    break;

                default:
                    break;
            }
        }
    }

    public enum CallbackType
    {
        Command,
        Paging
    }

    public enum CallbackAction
    {
        Unknown,
        Back,
        EditHashtagTemplate,
        AddHashtag,
        EditHashtagName,
        CreateTargetChat,
        AddBot
    }

    public class ParsedCallback
    {
        public long UserId { get; set; }
        public CallbackAction? Action { get; set; }
        public PaginationAction? PaginationAction { get; set; }
        public PageType? PageType { get; set; }
        public string? Parameter { get; set; }
    }
}