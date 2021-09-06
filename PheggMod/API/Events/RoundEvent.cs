using PheggMod.Patches;
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

    //WaitingForPlayersEvent
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

    //RoundStartEvent
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

    //RoundEndEvent
    public interface IEventHandlerRoundEnd : IEventHandler
    {
        void OnRoundEnd(RoundEndEvent ev);
    }
    public class RoundEndEvent : RoundEvent
    {
        public RoundEndEvent(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, PMRoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd, string Roundtime)
        {
            SCP = new SCPs { SCP_Kills = scp_kills, Starting_SCPs = list_start.scps_except_zombies, Ending_SCPs = list_finish.scps_except_zombies, Terminated_SCPs = list_start.scps_except_zombies - list_finish.scps_except_zombies };
            Class_D = new ClassD { Starting_ClassD = list_start.class_ds, Escaped_ClassD = e_ds };
            Scientist = new Scientists { Starting_Scientists = list_start.scientists, Escaped_Scientists = e_sc };

            TimeSpan tspan = TimeSpan.FromSeconds(list_finish.time - list_start.time);
            RoundTime = string.Format("{0} minutes and {1} seconds", (int)tspan.TotalMinutes, tspan.Seconds);
            LeadingTeam = leadingTeam;
        }

        public class SCPs
        {
            public int SCP_Kills { get; internal set; }
            public int Starting_SCPs { get; internal set; }
            public int Ending_SCPs { get; internal set; }
            public int Terminated_SCPs { get; internal set; }
        }
        public class ClassD
        {
            public int Escaped_ClassD { get; internal set; }
            public int Starting_ClassD { get; internal set; }
        }
        public class Scientists
        {
            public int Escaped_Scientists { get; internal set; }
            public int Starting_Scientists { get; internal set; }
        }

        public SCPs SCP { get; private set; }
        public ClassD Class_D { get; private set; }
        public Scientists Scientist { get; private set; }
        public string RoundTime { get; private set; }
        public PMRoundSummary.LeadingTeam LeadingTeam { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerRoundEnd)handler).OnRoundEnd(this);
        }
    }

    //WarheadStartEvent
    public interface IEventHandlerWarheadStart : IEventHandler
    {
        void OnWarheadStart(WarheadStartEvent ev);
    }
    public class WarheadStartEvent : RoundEvent
    {
        public WarheadStartEvent(bool initialStart, float timeToDetonation)
        {
            InitialStart = initialStart;
            TimeToDetonation = timeToDetonation;
        }
        public bool InitialStart { get; private set; }
        public float TimeToDetonation { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerWarheadStart)handler).OnWarheadStart(this);
        }
    }

    //WarheadCancelEvent
    public interface IEventHandlerWarheadCancel : IEventHandler
    {
        void OnWarheadCancel(WarheadCancelEvent ev);
    }
    public class WarheadCancelEvent : RoundEvent
    {
        public WarheadCancelEvent(PheggPlayer disabler = null)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            Disablier = disabler;
#pragma warning restore CS0612 // Type or member is obsolete
            Disabler = disabler;
        }
        [Obsolete]
        public PheggPlayer Disablier { get; private set; }
        public PheggPlayer Disabler { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerWarheadCancel)handler).OnWarheadCancel(this);
        }
    }

    //WarheadDetonateEvent
    public interface IEventHandlerWarheadDetonate : IEventHandler
    {
        void OnWarheadDetonate(WarheadDetonateEvent ev);
    }
    public class WarheadDetonateEvent : RoundEvent
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerWarheadDetonate)handler).OnWarheadDetonate(this);
        }
    }

	//LCZDecontaminateEvent
	public interface IEventHandlerLczDecontaminate : IEventHandler
    {
        void OnLczDecontaminate(LczDecontaminateEvent ev);
    }
    public class LczDecontaminateEvent : RoundEvent
    {
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerLczDecontaminate)handler).OnLczDecontaminate(this);
        }
    }

    //RespawnEvent
    public interface IEventHandlerRespawn : IEventHandler
    {
        void OnRespawn(RespawnEvent ev);
    }
    public class RespawnEvent : RoundEvent
    {
        public RespawnEvent(bool isci)
        {
            IsCI = isci;
        }

        public bool IsCI { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerRespawn)handler).OnRespawn(this);
        }
    }
}
