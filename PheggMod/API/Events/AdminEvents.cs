using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API.Events
{
    public abstract class AdminEvent : Event
    {
    }

    //PlayerBanEvent
    public interface IEventHandlerPlayerBan : IEventHandler
    {
        void OnPlayerBan(PlayerBanEvent ev);
    }
    public class PlayerBanEvent : AdminEvent
    {
        public PlayerBanEvent(PheggPlayer player, int duration, PheggPlayer admin = null, string reason = null)
        {
            Duration = duration;
            Player = player;
            Admin = admin;
            Reason = reason;
        }
        public int Duration { get; internal set; }
        public PheggPlayer Player { get; internal set; }
        public PheggPlayer Admin { get; internal set; }
        public string Reason { get; internal set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerBan)handler).OnPlayerBan(this);
        }
    }

    //PlayerKickEvent
    public interface IEventHandlerPlayerKick : IEventHandler
    {
        void OnPlayerKick(PlayerKickEvent ev);
    }
    public class PlayerKickEvent : AdminEvent
    {
        public PlayerKickEvent(PheggPlayer player, PheggPlayer admin = null, string reason = null)
        {
            Player = player;
            Admin = admin;
            Reason = reason;
        }
        public PheggPlayer Player { get; internal set; }
        public PheggPlayer Admin { get; internal set; }
        public string Reason { get; internal set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerKick)handler).OnPlayerKick(this);
        }
    }

    //GlobalBanEvent
    public interface IEventHandlerGlobalBan : IEventHandler
    {
        void OnGlobalBan(GlobalBanEvent ev);
    }
    public class GlobalBanEvent : AdminEvent
    {
        public GlobalBanEvent(PheggPlayer player)
        {
            Player = player;
        }
        public PheggPlayer Player { get; internal set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerGlobalBan)handler).OnGlobalBan(this);
        }
    }

    //AdminQueryEvent
    public interface IEventHandlerAdminQuery : IEventHandler
    {
        void OnAdminQuery(AdminQueryEvent ev);
    }
    public class AdminQueryEvent : AdminEvent
    {
        public AdminQueryEvent(PheggPlayer admin, string query)
        {
            Admin = admin;
            Query = query;
        }
        public PheggPlayer Admin { get; internal set; }
        public string Query { get; internal set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerAdminQuery)handler).OnAdminQuery(this);
        }
    }

    //RefreshAdminPerms
    public interface IEventHandlerRefreshAdminPerms : IEventHandler
    {
        void OnRefreshAdminPerms(RefreshAdminPermsEvent ev);
    }
    public class RefreshAdminPermsEvent : AdminEvent
    {
        public RefreshAdminPermsEvent(PheggPlayer admin)
        {
            Admin = admin;
        }
        public PheggPlayer Admin { get; internal set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerRefreshAdminPerms)handler).OnRefreshAdminPerms(this);
        }
    }
}
