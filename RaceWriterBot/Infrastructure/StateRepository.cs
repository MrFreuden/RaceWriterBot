using RaceWriterBot.Application.Interfaces;
using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceWriterBot.Infrastructure
{
    internal class StateRepository : IStateRepository
    {
        private readonly Dictionary<UserId, IState> _states;

        public StateRepository(Dictionary<UserId, IState> states)
        {
            _states = states;
        }

        public void AddState(UserId userId, IState state)
        {
            if (_states.ContainsKey(userId))
                throw new ArgumentException("У пользователя уже есть активный диалог");

            _states[userId] = state;
        }

        public IState GetActiveState(UserId userId)
        {
            if (_states.TryGetValue(userId, out var state))
                return state;
            else
                throw new ArgumentException("У пользователя нет активного диалога");
        }

        public bool HasActiveState(UserId userId)
        {
            return _states.ContainsKey(userId);
        }

        public void RemoveState(UserId userId)
        {
            if (!_states.Remove(userId))
                throw new ArgumentException("Пользователя нету в базе");
        }
    }
}
