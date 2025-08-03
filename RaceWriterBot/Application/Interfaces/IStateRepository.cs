using RaceWriterBot.Domain.Interfaces;
using RaceWriterBot.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceWriterBot.Application.Interfaces
{
    public interface IStateRepository
    {
        IState GetActiveState(UserId userId);
        void AddState(UserId userId, IState state);
        void RemoveState(UserId userId);
        bool HasActiveState(UserId userId);
    }
}
