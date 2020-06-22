using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.API.Events
{
    public abstract class PlayerEvent : Event
    {
        public PlayerEvent(PheggPlayer player)
        {
            this.player = player;
        }

        private PheggPlayer player;
        public PheggPlayer Player { get => player; }
    }

    //PlayerHurtEvent
    public interface IEventHandlerPlayerHurt : IEventHandler
    {
        void OnPlayerHurt(PlayerHurtEvent ev);
    }
    public class PlayerHurtEvent : PlayerEvent
    {
        public PlayerHurtEvent(PheggPlayer player, PheggPlayer attacker, float damage, DamageTypes.DamageType damageType, PlayerStats.HitInfo info) : base(player)
        {
            Attacker = attacker;
            Damage = damage;
            DamageType = damageType;
            HitInfo = info;
        }

        public PheggPlayer Attacker { get; private set; }
        public float Damage { get; private set; }
        public DamageTypes.DamageType DamageType { get; private set; }
        public PlayerStats.HitInfo HitInfo { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerHurt)handler).OnPlayerHurt(this);
        }
    }

    //PlayerDieevent
    public interface IEventHandlerPlayerDeath : IEventHandler
    {
        void OnPlayerDie(PlayerDeathEvent ev);
    }
    public class PlayerDeathEvent : PlayerEvent
    {
        public PlayerDeathEvent(PheggPlayer player, PheggPlayer attacker, float damage, DamageTypes.DamageType damageType, PlayerStats.HitInfo info) : base(player)
        {
            Attacker = attacker;
            Damage = damage;
            DamageType = damageType;
            HitInfo = info;
        }

        public PheggPlayer Attacker { get; private set; }
        public float Damage { get; private set; }
        public DamageTypes.DamageType DamageType { get; private set; }
        public PlayerStats.HitInfo HitInfo { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerDeath)handler).OnPlayerDie(this);
        }
    }

    //PlayerSpawnEvent
    public interface IEventHandlerPlayerSpawn : IEventHandler
    {
        void OnPlayerSpawn(PlayerSpawnEvent ev);
    }
    public class PlayerSpawnEvent : PlayerEvent
    {
        public PlayerSpawnEvent(PheggPlayer player, RoleType role, Team team) : base(player)
        {
            Role = role;
            Team = team;
        }
        public RoleType Role { get; private set; }
        public Team Team { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerSpawn)handler).OnPlayerSpawn(this);
        }
    }

    //PlayerEscapeEvent
    public interface IEventHandlerPlayerEscape : IEventHandler
    {
        void OnPlayerEscape(PlayerEscapeEvent ev);
    }
    public class PlayerEscapeEvent : PlayerEvent
    {
        public PlayerEscapeEvent(PheggPlayer player, RoleType role, RoleType newrole, Team team) : base(player)
        {
            Role = role;
            newRole = newrole;
            Team = team;
        }
        public RoleType Role { get; private set; }
        public RoleType newRole { get; private set; }
        public Team Team { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerEscape)handler).OnPlayerEscape(this);
        }
    }

    //PlayerJoinEvent
    public interface IEventHandlerPlayerJoin : IEventHandler
    {
        void OnPlayerJoin(PlayerJoinEvent ev);
    }
    public class PlayerJoinEvent : PlayerEvent
    {
        public PlayerJoinEvent(PheggPlayer player) : base(player)
        {
            Name = player.name;
            UserID = player.userId;
            IpAddress = player.ipAddress;
        }

        public string Name { get; private set; }
        public string UserID { get; private set; }
        public string IpAddress { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerJoin)handler).OnPlayerJoin(this);
        }
    }

    //PlayerLeaveEvent
    public interface IEventHandlerPlayerLeave : IEventHandler
    {
        void OnPlayerLeave(PlayerLeaveEvent ev);
    }
    public class PlayerLeaveEvent : PlayerEvent
    {
        public PlayerLeaveEvent(PheggPlayer player) : base(player)
        {
            Name = player.name;
            UserID = player.userId;
            IpAddress = player.ipAddress;
        }

        public string Name { get; private set; }
        public string UserID { get; private set; }
        public string IpAddress { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerLeave)handler).OnPlayerLeave(this);
        }
    }

    //PlayerThrowGrenade
    public interface IEventHandlerPlayerThrowGrenade : IEventHandler
    {
        void OnThrowGrenade(PlayerThrowGrenadeEvent ev);
    }
    public class PlayerThrowGrenadeEvent : PlayerEvent
    {
        public PlayerThrowGrenadeEvent(PheggPlayer player, Grenades.GrenadeSettings settings) : base(player)
        {
            Name = player.name;
            Grenade = settings.apiName;
        }

        public string Name { get; private set; }
        public string Grenade { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerThrowGrenade)handler).OnThrowGrenade(this);
        }
    }

    //Report event
    public interface IEventHandlerPlayerReport : IEventHandler
    {
        void OnReport(PlayerReportEvent ev);
    }
    public class PlayerReportEvent : PlayerEvent
    {
        public PlayerReportEvent(PheggPlayer reporter, PheggPlayer target, string reason) : base(reporter)
        {
            Reporter = reporter;
            Target = target;
            Reason = reason;
        }

        public PheggPlayer Reporter { get; private set; }
        public PheggPlayer Target { get; private set; }
        public string Reason { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerReport)handler).OnReport(this);
        }
    }
}
