using System;
using System.Collections.Generic;
using System.Linq;
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
        public PlayerHurtEvent(PheggPlayer player, PheggPlayer attacker, float damage, DamageTypes.DamageType damageType) : base(player)
        {
            Attacker = attacker;
            Damage = damage;
            DamageType = damageType;
        }

        public PheggPlayer Attacker { get; private set; }
        public float Damage { get; private set; }
        public DamageTypes.DamageType DamageType { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerHurt)handler).OnPlayerHurt(this);
        }
    }

    //PlayerDieevent
    public interface IEventHandlerPlayerDie : IEventHandler
    {
        void OnPlayerDie(PlayerDeathEvent ev);
    }
    public class PlayerDeathEvent : PlayerEvent
    {
        public PlayerDeathEvent(PheggPlayer player, PheggPlayer attacker, float damage, DamageTypes.DamageType damageType) : base(player)
        {
            Attacker = attacker;
            Damage = damage;
            DamageType = damageType;
        }

        public PheggPlayer Attacker { get; private set; }
        public float Damage { get; private set; }
        public DamageTypes.DamageType DamageType { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerDie)handler).OnPlayerDie(this);
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
}
