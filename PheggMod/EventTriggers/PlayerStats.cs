#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using System;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::PlayerStats")]
    public class PMPlayerStats : PlayerStats
    {
        //PlayerHurtEvent
        public extern bool orig_HurtPlayer(PlayerStats.HitInfo info, GameObject go);
        public new bool HurtPlayer(PlayerStats.HitInfo info, GameObject go)
        {
            if (!go.GetComponent<CharacterClassManager>().isLocalPlayer && info.GetDamageType() != DamageTypes.None)
            {
                PheggPlayer pPlayer = new PheggPlayer(go);
                PlayerStats Pstats = go.GetComponent<PlayerStats>();
                PheggPlayer pAttacker = null;

                if (info.GetPlayerObject() != null) { pAttacker = new PheggPlayer(info.GetPlayerObject()); }

                if (Pstats.health - info.Amount < 1) PluginManager.TriggerEvent<IEventHandlerPlayerDeath>(new PlayerDeathEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType()));
                else PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType()));
            }

            orig_HurtPlayer(info, go);

            return false;
        }
    }
}
