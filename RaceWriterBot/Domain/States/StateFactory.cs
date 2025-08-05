using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models;
using RaceWriterBot.Domain.Models.Entity;
using RaceWriterBot.Enums;

namespace RaceWriterBot.Domain.States
{
    public class StateFactory : IStateFactory
    {
        public StateFactory()
        {

        }

        public IState CreateFromCallback(string? callbackData, User user)
        {
            if (callbackData == null || string.IsNullOrEmpty(callbackData))
                throw new ArgumentException("Некорректные данные запроса");

            var parts = callbackData.Split('_');
            if (parts.Length < 2)
                throw new ArgumentException("Неверный формат данных команды");
            var stateType = parts[1];

            return stateType switch
            {
                CommandNames.EDIT_HASHTAG => new EditHashtagState(user, parts),
                CommandNames.ADD_HASHTAG => new AddHashtagState(user, parts),
                CommandNames.EDIT_HASHTAG_TEMPLATE => new EditHashtagTemplateState(user, parts),
                CommandNames.ADD_TARGET_CHAT => new AddTargetChatState(user),
                //CommandNames.CONFIRMATION_ADDING_BOT => AddingBotAsAdminState()

                _ => throw new ArgumentException($"Неизвестная команда: {stateType}")
            };
        }
    }

    public class PagingState : IState
    {
        List<MenuAction> _actions;
        public Task ExecuteAsync(IStateInput input)
        {
            throw new NotImplementedException();
        }

        public InputRequestType GetRequiredInput() => InputRequestType.PaginationAction;
    }

    public static class CommandNames
    {
        public const string Start = "/start";
        public const string Settings = "/settings";
        public const string BACK = "back";
        public const string EDIT_HASHTAG_TEMPLATE = "EditHashtagTemplate";
        public const string ADD_HASHTAG = "AddHashtag";
        public const string EDIT_HASHTAG = "EditHashtag";
        public const string ADD_TARGET_CHAT = "AddTargetChat";
        public const string CONFIRMATION_ADDING_BOT = "UserConfirmAddingBotToTargetChat";
        public const string PAGING = "Paging";
    }
}
