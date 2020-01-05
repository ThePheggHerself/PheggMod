#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using Mirror;
using System.Net;
using MEC;

namespace PheggMod.API
{
    public interface IEventHandlerPlayerDie : IEventHandler
    {
        void OnPlayerdie(PlayerDeathEvent ev);
    }

    public class PlayerDeathEvent : Event
    {
        public PheggPlayer player { get; internal set; }
        public PheggPlayer attacker { get; internal set; }
        public PlayerStats.HitInfo hitInfo { get; internal set; }

        public PlayerDeathEvent(GameObject plr, GameObject atr, PlayerStats.HitInfo info)
        {
            player = new PheggPlayer(plr);
            attacker = new PheggPlayer(atr);
            //weapon = info.GetDamageType();
            hitInfo = info;
        }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPlayerDie)handler).OnPlayerdie(this);
        }
    }
}
