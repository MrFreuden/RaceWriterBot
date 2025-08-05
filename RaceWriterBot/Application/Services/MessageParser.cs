using RaceWriterBot.Application.DTOs;
using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.Models;
using RaceWriterBot.Domain.States;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Enums;
using RaceWriterBot.Presentation.Handlers;
using Telegram.Bot.Types;
using User = RaceWriterBot.Domain.Models.Entity.User;

namespace RaceWriterBot.Application.Services
{
    public class MessageParser : IMessageParser
    {
        private readonly IStateRepository _stateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMenuService _menuService;

        public MessageParser(IStateRepository stateRepository, IUserRepository userRepository, IMenuService menuService)
        {
            _stateRepository = stateRepository;
            _userRepository = userRepository;
            _menuService = menuService;
        }

        public MessageDTO HandleMessage(Message message)
        {
            var userId = new UserId(message.From.Id);
            var user = _userRepository.GetUser(userId);

            var commandHandler = new CommandHandler(user, _userRepository, _menuService);
            var stateHandler = new StateHandler(user, _stateRepository, _menuService);
            var regHandler = new RegistrationHandler(user, _menuService);

            commandHandler.SetNext(stateHandler).SetNext(regHandler);

            var result = commandHandler.Handle(message);

            return result;
        }

        public MessageDTO HandleChatMember(ChatMemberUpdated myChatMember)
        {
            var userId = new UserId(myChatMember.From.Id);

            if (!_stateRepository.HasActiveState(userId))
            {
                return new MessageDTO
                {
                    UserId = userId,
                    Text = "Необходимо сначала выбрать добавление чата через меню"
                };
            }

            var state = _stateRepository.GetActiveState(userId);

            if (state.GetRequiredInput() != InputRequestType.AddBotToChat)
            {
                return new MessageDTO
                {
                    UserId = userId,
                    Text = "Неверный тип ввода для текущего состояния"
                };
            }

            if (!myChatMember.NewChatMember.IsAdmin)
            {
                return new MessageDTO
                {
                    UserId = userId,
                    Text = "Бот должен иметь права администратора для работы с чатом"
                };
            }

            var chatId = new TargetChatId(myChatMember.Chat.Id);
            var chatName = myChatMember.Chat.Title;

            var input = new ChatMemberInput(userId, chatId);
            try
            {
                state.ExecuteAsync(input);

                _stateRepository.RemoveState(userId);

                return new MessageDTO
                {
                    UserId = userId,
                    Text = $"Чат '{chatName}' успешно добавлен. Теперь вы можете настроить хештеги."
                };
            }
            catch (Exception ex)
            {
                return new MessageDTO
                {
                    UserId = userId,
                    Text = $"Ошибка при добавлении чата: {ex.Message}"
                };
            }
        }
    }

    public class CommandHandler : Handler
    {
        private readonly IUserRepository _userRepository;
        public CommandHandler(User user, IUserRepository userRepository, IMenuService menuService) : base(user, menuService)
        {
            _userRepository = userRepository;
        }

        public override MessageDTO Handle(Message message)
        {
            if (message.Text == "/start")
            {
                _userRepository.AddUser(User);
                return new MessageDTO() { UserId = User.UserId, Text = "Ласкаво просимо" };
            }

            if (message.Text == "/channels")
            {
                var chats = User.GetTargetChats();
                if (chats.Count == 0)
                {
                    var actions = new List<MenuAction>();
                    actions.Add(new MenuAction(MenuTextsConst.Create, CommandNames.ADD_TARGET_CHAT));
                    return MenuService.BuildSimpleMenu(User.UserId, MenuTextsConst.NoChats, actions, 0);
                }
                else
                {
                    var actions = new List<MenuAction>();
                    actions.Add(new MenuAction(MenuTextsConst.Create, CommandNames.ADD_TARGET_CHAT));
                    return MenuService.BuildSimpleMenu(User.UserId, MenuTextsConst.ActiveChats, actions, 0);
                }


            }

            return Next?.Handle(message);
        }
    }

    public class StateHandler : Handler
    {
        private readonly IStateRepository _stateRepository;
        public StateHandler(User user, IStateRepository stateRepository, IMenuService menuService) : base(user, menuService)
        {
            _stateRepository = stateRepository;
        }

        public override MessageDTO Handle(Message message)
        {
            if (_stateRepository.HasActiveState(User.UserId))
            {
                var state = _stateRepository.GetActiveState(User.UserId);
                //if (state.GetRequiredInput() == InputRequestType.Text)
                //{
                   
                //}
                var input = new StringInput(message.Text);
                state.ExecuteAsync(input);
            }

            return Next?.Handle(message);
        }
    }

    public class RegistrationHandler : Handler
    {
        public RegistrationHandler(User user, IMenuService menuService) : base(user, menuService)
        {
        }

        public override MessageDTO Handle(Message message)
        {
            return Next?.Handle(message);
        }
    }

    public abstract class Handler
    {
        protected Handler Next;
        protected User User;
        protected IMenuService MenuService;

        protected Handler(User user, IMenuService menuService)
        {
            User = user;
            MenuService = menuService;
        }

        public Handler SetNext(Handler handler)
        {
            Next = handler;
            return handler;
        }

        public abstract MessageDTO Handle(Message message);
    }
}