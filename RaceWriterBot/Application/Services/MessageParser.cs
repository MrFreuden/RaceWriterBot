using RaceWriterBot.Application.DTOs;
using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.ValueObjects;
using RaceWriterBot.Presentation.Handlers;
using Telegram.Bot.Types;
using User = RaceWriterBot.Domain.Models.Entity.User;

namespace RaceWriterBot.Application.Services
{
    public class MessageParser : IMessageParser
    {
        private readonly IStateRepository _stateRepository;
        private readonly IUserRepository _userRepository;

        public MessageDTO HandleMessage(Message message)
        {
            var userId = new UserId(message.From.Id);
            var user = _userRepository.GetUser(userId);

            var commandHandler = new CommandHandler(user, _userRepository);
            var stateHandler = new StateHandler(user, _stateRepository);
            var regHandler = new RegistrationHandler(user);

            commandHandler.SetNext(stateHandler).SetNext(regHandler);

            var result = commandHandler.Handle(message);

            return result;
        }
    }

    public class CommandHandler : Handler
    {
        private readonly IUserRepository _userRepository;
        public CommandHandler(User user, IUserRepository userRepository) : base(user)
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
                    var keyboard = ; //
                    return new MessageDTO { UserId = User.UserId, Text = "У вас немає активних каналів", InlineKeyboardMarkup = keyboard };
                }
                else
                {
                    var keyboard = ;
                    return new MessageDTO { UserId = User.UserId, Text = "Активні канали", InlineKeyboardMarkup = keyboard };
                }


            }

            return Next?.Handle(message);
        }
    }

    public class StateHandler : Handler
    {
        private readonly IStateRepository _stateRepository;
        public StateHandler(User user, IStateRepository stateRepository) : base(user)
        {
            _stateRepository = stateRepository;
        }

        public override MessageDTO Handle(Message message)
        {
            if (_stateRepository.HasActiveState(User.UserId))
            {
                var state = _stateRepository.GetActiveState(User.UserId);
                state.ExecuteAsync(message.Text);
            }

            return Next?.Handle(message);
        }
    }

    public class RegistrationHandler : Handler
    {
        public RegistrationHandler(User user) : base(user)
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

        protected Handler(User user)
        {
            User = user;
        }

        public Handler SetNext(Handler handler)
        {
            Next = handler;
            return handler;
        }

        public abstract MessageDTO Handle(Message message);
    }
}