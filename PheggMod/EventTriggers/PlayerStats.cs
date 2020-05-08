#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using MEC;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
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
                //if (Commands.CustomInternalCommands.nodamageplayers.ContainsKey(go.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId))
                //    info.Amount = 0;

                PheggPlayer pPlayer = new PheggPlayer(go);
                PlayerStats Pstats = go.GetComponent<PlayerStats>();
                PheggPlayer pAttacker = null;

                if (info.GetPlayerObject() != null) { pAttacker = new PheggPlayer(info.GetPlayerObject()); }

                if (Pstats.health - info.Amount < 1)
                    try
                    {
                        PluginManager.TriggerEvent<IEventHandlerPlayerDeath>(new PlayerDeathEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType()));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerDeathEvent: {e.ToString()}");
                    }
                else
                    try
                    {
                        PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType()));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerHurtEvent: {e.ToString()}");
                    }
            }

            orig_HurtPlayer(info, go);

            return false;
        }

        public extern void orig_Roundrestart();
        public new void Roundrestart()
        {
            try
            {
                Commands.CustomInternalCommands.isLightsout = false;
                PMAlphaWarheadController.nukeLock = false;
                Commands.CustomInternalCommands.nodamageplayers.Clear();

                if (Commands.CustomInternalCommands.reloadPlugins)
                    Timing.RunCoroutine(TriggerPluginReload());
            }
            catch (Exception) { }

            orig_Roundrestart();
        }

        private IEnumerator<float> TriggerPluginReload()
        {
            yield return Timing.WaitForSeconds(ConfigFile.ServerConfig.GetFloat("auto_round_restart_time", 10) + 1f);

            PluginManager.Reload();
        }
    }
}
