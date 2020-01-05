using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API.Events
{
    public abstract class RoundEvent : Event
    {
    }

    public interface IEventHandlerWaitingForPlayers : IEventHandler
    {
        void OnWaitingForPlayers(WaitingForPlayersEvent ev);
    }
    public class WaitingForPlayersEvent : RoundEvent
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerWaitingForPlayers)handler).OnWaitingForPlayers(this);
        }
    }

    public interface IEventHandlerRoundStart : IEventHandler
    {
        void OnRoundStart(RoundStartEvent ev);
    }
    public class RoundStartEvent : RoundEvent
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerRoundStart)handler).OnRoundStart(this);
        }
    }
}
