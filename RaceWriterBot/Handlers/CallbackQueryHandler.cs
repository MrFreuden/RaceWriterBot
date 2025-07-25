﻿using RaceWriterBot.Infrastructure;
using RaceWriterBot.Interfaces;
using RaceWriterBot.Managers;
using RaceWriterBot.Models;
using Telegram.Bot.Types;

namespace RaceWriterBot.Handlers
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

        public Task ProcessCallbackQuery(CallbackQuery query)
        {
            if (query.Data.StartsWith(Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE))
            {
                var hashtagName = query.Data.Split('_').Last();
                StartEditHashtagTemplate(query.From.Id, hashtagName, query.Message.MessageId);
                return Task.CompletedTask;
            }

            if (query.Data.StartsWith(Constants.CommandNames.ACTION_ADD_HASHTAG))
            {
                if (int.TryParse(query.Data.Split("_").Last(), out var channelHash))
                {
                    AddNewHashtag(query.From.Id, channelHash, query.Message.MessageId);
                }
                return Task.CompletedTask;
            }

            var segments = query.Data.Split('_', 3);
            if (segments.Length >= 2)
            {
                var pageType = segments[0];
                var action = segments[1];
                HandlePagination(query, pageType, action, segments.Length > 2 ? segments[2] : null);
                return Task.CompletedTask;
            }
            switch (query.Data)
            {
                case Constants.CommandNames.ACTION_CREATE_TARGET_CHAT:
                    _viewManager.AddBotToTargetChatSettings(query.From.Id);
                    break;
                case Constants.CommandNames.ACTION_CONFIRMATION_ADDING_BOT:
                    _viewManager.RequestForwardedMessage(query.From.Id);
                    break;
                case Constants.CommandNames.ACTION_BACK:
                    _menuManager.NavigateBack(query.From.Id);
                    break;
                case "4":
                    break;
                case "":
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }

        private void HandlePagination(CallbackQuery query, string pageType, string action, string data)
        {
            var userId = query.From.Id;
            var chatId = query.Message.Chat.Id;

            switch (pageType)
            {
                case Constants.CommandNames.CHANNELS_PAGE:
                    _menuManager.HandlePaginationAction<TargetChatSession>(
                        userId, chatId, pageType, action, data,
                        (session) => _viewManager.ShowHashtags(userId, session));
                    break;

                case Constants.CommandNames.HASHTAGS_PAGE:
                    _menuManager.HandlePaginationAction<HashtagSession>(
                        userId, chatId, pageType, action, data,
                        (hashtag) => _viewManager.ShowTemplateMessage(userId, hashtag));
                    break;

                case Constants.CommandNames.MESSAGES_PAGE:
                    _menuManager.HandlePaginationAction<PostMessagePair>(
                        userId, chatId, pageType, action, data,
                        (pair) => _viewManager.ShowMessageDetails(userId, pair));
                    break;
            }
        }

        private void AddNewHashtag(long userId, int channelHash, int messageId)
        {
            var user = _userDataStorage.GetUser(userId);
            var channelSession = user.GetTargetChatSessions(channelHash);

            if (channelSession != null)
            {
                user.SetExpectedAction(Constants.CommandNames.ACTION_ADD_HASHTAG, channelSession);

                _botMessenger.SendMessage(
                    userId,
                    "Введіть новий хештег");
            }
        }

        private void StartEditHashtagTemplate(long userId, string hashtagName, int messageId)
        {
            var user = _userDataStorage.GetUser(userId);
            var hashtag = user.GetHashtagSession(hashtagName);
            if (hashtag == null)
            {
                _botMessenger.SendMessage(
                    userId,
                    "Хештег не знайдено або у вас немає прав для його редагування.");
                return;
            }

            user.SetExpectedAction(Constants.CommandNames.ACTION_EDIT_HASHTAG_TEMPLATE, hashtag);

            _botMessenger.SendMessage(
                userId,
                "Будь ласка, введіть новий текст шаблону для хештега #" + hashtagName);
        }
    }
}