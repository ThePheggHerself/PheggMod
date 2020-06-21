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
            if(isLocalPlayer || !NetworkServer.active)
                return orig_HurtPlayer(info, go);

            PheggPlayer pPlayer = new PheggPlayer(go);
            Role playerRole = pPlayer.role;

            PheggPlayer pAttacker = null;
            Role attackerRole = null;
            if (info.GetPlayerObject() != null)
            {
                pAttacker = new PheggPlayer(info.GetPlayerObject());
                attackerRole = pAttacker.role;
            }

            PMDamageType kT;
            if (info.GetDamageType() == DamageTypes.Flying)
                kT = PMDamageType.AntiCheat;
            else if (info.GetPlayerObject() == null)
                kT = PMDamageType.WorldKill;
            else if (go.GetComponent<Handcuffs>().CufferId > -1 && pAttacker.refHub.characterClassManager.IsAnyScp())
                kT = PMDamageType.DisarmedKill;
            else if (IsTeamDamage(playerRole.team, attackerRole.team))
                kT = PMDamageType.TeamKill;
            else
                kT = PMDamageType.Normal;

            PlayerHurtCache pHC = new PlayerHurtCache
            {
                PlayerOriginalRole = pPlayer.roleType,
                AttackerOriginalRole = pAttacker == null ? RoleType.None : pAttacker.roleType,
                PMDamageType = kT,
                HitInfo = info
            };


            bool result = orig_HurtPlayer(info, go);

            if (!go.GetComponent<CharacterClassManager>().isLocalPlayer && info.GetDamageType() != DamageTypes.None && !go.GetComponent<CharacterClassManager>().GodMode)
            {
                bool IsKill = pPlayer.roleType == RoleType.Spectator && pHC.PlayerOriginalRole != RoleType.Spectator;

                if (IsKill)
                    try
                    {
                        Base.Debug("Triggering PlayerDeathEvent");
                        PluginManager.TriggerEvent<IEventHandlerPlayerDeath>(new PlayerDeathEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType(), pHC));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerDeathEvent: {e.InnerException}");
                    }

                else
                    try
                    {
                        Base.Debug("Triggering PlayerHurtEvent");
                        PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(pPlayer, pAttacker, info.Amount, info.GetDamageType(), pHC));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerHurtEvent: {e.InnerException}");
                    }


                if (PMConfigFile.enable008)
                {
                    if (IsKill && (info.GetDamageType() == DamageTypes.Scp0492 || info.GetDamageType() == DamageTypes.Poison))
                    {
                        pPlayer.roleType = RoleType.Scp0492;

                        Ragdoll rd = pPlayer.gameObject.GetComponent<Ragdoll>();
                        Base.Info(rd.transform.position.ToString());
                    }

                    if (pAttacker.roleType == RoleType.Scp0492)
                    {
                        pPlayer.gameObject.GetComponent<PlayerEffectsController>().EnableEffect<CustomEffects.SCP008>(300f, false);
                        pPlayer.gameObject.GetComponent<HintDisplay>().Show(new TextHint("You have been infected!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 8));
                    }
                }
            }

            return result;
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
