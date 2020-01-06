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

using PheggMod.API.Events;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::PlayerStats")]
    class PMPlayerStats : PlayerStats
    {
        //PlayerHurtEvent
        public extern bool orig_HurtPlayer(PlayerStats.HitInfo info, GameObject go);
        public new bool HurtPlayer(PlayerStats.HitInfo info, GameObject Player)
        {
            try
            {
                if (!Player.GetComponent<CharacterClassManager>().isLocalPlayer)
                {
                    PheggPlayer pPlayer = new PheggPlayer(Player);
                    PheggPlayer pAttacker = null;

                    if (info.GetPlayerObject() != null) { pAttacker = new PheggPlayer(info.GetPlayerObject()); }

                    PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType()));
                }
            }
            catch (Exception e) { Base.Error(e.Message); }

            orig_HurtPlayer(info, Player);

            return false;
        }
    }
}
