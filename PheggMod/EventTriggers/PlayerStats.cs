#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using Hints;
using MEC;
using Mirror;
using MonoMod;
using PheggMod.API;
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
        public extern bool orig_HurtPlayer(HitInfo info, GameObject go);
        public new bool HurtPlayer(HitInfo info, GameObject go)
        {
            try
            {
                if (isLocalPlayer || !NetworkServer.active)
                    return orig_HurtPlayer(info, go);

                PheggPlayer player = new PheggPlayer(go);
                PheggPlayer attacker = null;
                try { attacker = new PheggPlayer(info.GetPlayerObject()); }
                catch { }

                PMDamageType kT;

                if (info.GetDamageType() == DamageTypes.Flying)
                    kT = PMDamageType.AntiCheat;
                else if (attacker == null || attacker.isEmpty)
                    kT = PMDamageType.WorldKill;

                else if (player.refHub.handcuffs.CufferId > -1 && attacker.refHub.characterClassManager.IsAnyScp())
                    kT = PMDamageType.DisarmedKill;
                else if (IsTeamDamage(player.role.team, attacker.role.team))
                    kT = PMDamageType.TeamKill;
                else
                    kT = PMDamageType.Normal;

                PlayerHurtCache pHC = new PlayerHurtCache { PlayerOriginalRole = player.roleType, AttackerOriginalRole = attacker?.roleType, PMDamageType = kT, HitInfo = info };

                bool result = orig_HurtPlayer(info, go);
                if (player.refHub.characterClassManager.isLocalPlayer || info.GetDamageType() == DamageTypes.None || player.refHub.characterClassManager.GodMode)
                    return result;

                bool IsKill = player.roleType == RoleType.Spectator && pHC.PlayerOriginalRole != RoleType.Spectator;
                if (IsKill)
                    try
                    {
                        Base.Debug("Triggering PlayerDeathEvent");
                        PluginManager.TriggerEvent<IEventHandlerPlayerDeath>(new PlayerDeathEvent(player, attacker, info.Amount, info.GetDamageType(), pHC));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerDeathEvent: {e.InnerException}");
                    }
                else
                    try
                    {
                        Base.Debug("Triggering PlayerHurtEvent");
                        PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(player, attacker, info.Amount, info.GetDamageType(), pHC));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerHurtEvent: {e.InnerException}");
                    }

                if (PMConfigFile.enable008)
                {
                    if (IsKill && (info.GetDamageType() == DamageTypes.Scp0492 || info.GetDamageType() == DamageTypes.Poison))
                        player.roleType = RoleType.Scp0492;

                    else if (attacker != null && attacker.roleType == RoleType.Scp0492)
                    {
                        CustomEffects.SCP008 effect = player.refHub.playerEffectsController.GetEffect<CustomEffects.SCP008>();

                        if (effect == null || !effect.Enabled)
                            player.gameObject.GetComponent<PlayerEffectsController>().EnableEffect<CustomEffects.SCP008>(300f, false);

                        else
                            player.refHub.playerEffectsController.GetEffect<CustomEffects.SCP008>().intensity++;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Base.Info(e.ToString());
                return orig_HurtPlayer(info, go);
            }
        }

        public extern void orig_Roundrestart();
        public new void Roundrestart()
        {
            try
            {
                Commands.CustomInternalCommands.isLightsout = false;
                PMAlphaWarheadController.nukeLock = false;

                if (Commands.CustomInternalCommands.reloadPlugins)
                {
                    Timing.RunCoroutine(TriggerPluginReload());
                    Commands.CustomInternalCommands.reloadPlugins = false;
                }
            }
            catch (Exception) { }

            orig_Roundrestart();
        }

        private IEnumerator<float> TriggerPluginReload()
        {
            yield return Timing.WaitForSeconds(ConfigFile.ServerConfig.GetFloat("auto_round_restart_time", 10) + 1f);

            PluginManager.Reload();
        }

        public static bool IsTeamDamage(Team player, Team attacker)
        {
            if (player == attacker)
                return true;
            else if ((player == Team.CDP || player == Team.CHI) && (attacker == Team.CDP || attacker == Team.CHI))
                return true;
            else if ((player == Team.MTF || player == Team.MTF) && (attacker == Team.RSC || attacker == Team.MTF))
                return true;
            else return false;
        }
    }
}
