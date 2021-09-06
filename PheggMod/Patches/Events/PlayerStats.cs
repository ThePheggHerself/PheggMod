﻿#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
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

namespace PheggMod.Patches
{
	[MonoModPatch("global::PlayerStats")]
	public class PMPlayerStats : PlayerStats
	{
		//PlayerHurtEvent
		public extern bool orig_HurtPlayer(HitInfo info, GameObject go, bool noTeamDamage = false, bool IsValidDamage = true);
		public new bool HurtPlayer(HitInfo info, GameObject go, bool noTeamDamage = false, bool IsValidDamage = true)
		{
			try
			{
				if (isLocalPlayer || !NetworkServer.active || go == null)
					try
					{
						return orig_HurtPlayer(info, go, noTeamDamage, IsValidDamage);
					}
					catch (Exception e)
					{
						Base.Error(e.ToString());
						return orig_HurtPlayer(info, go, noTeamDamage, IsValidDamage);
					}

				PheggPlayer player = new PheggPlayer(go);
				if (player == null)
					try
					{
						return orig_HurtPlayer(info, go, noTeamDamage, IsValidDamage);
					}
					catch (Exception e)
					{
						Base.Error(e.ToString());
						return orig_HurtPlayer(info, go, noTeamDamage, IsValidDamage);
					}

				if (player.refHub.characterClassManager.isLocalPlayer || info.GetDamageType() == DamageTypes.None || player.refHub.characterClassManager.GodMode)
					return orig_HurtPlayer(info, go, noTeamDamage, IsValidDamage);

				PheggPlayer attacker = null;
				try { attacker = new PheggPlayer(info.GetPlayerObject()); }
				catch { }

				if (FFDetector.FFDetector.DetectorEnabled)
					FFDetector.FFDetector.CalculateFF(go, info, out info.Amount);

				bool IsKill = info.Amount >= player.health;
				if (info.Amount > 0)
				{
					if (IsKill && info.Amount > 0)
						try
						{
							Base.Debug("Triggering PlayerDeathEvent");
							PluginManager.TriggerEvent<IEventHandlerPlayerDeath>(new PlayerDeathEvent(player, attacker, info.Amount, info.GetDamageType(), info));
						}
						catch (Exception e)
						{
							Base.Error($"Error triggering PlayerDeathEvent: {e.InnerException}");
						}
					else
						try
						{
							Base.Debug("Triggering PlayerHurtEvent");
							PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(player, attacker, info.Amount, info.GetDamageType(), info));
						}
						catch (Exception e)
						{
							Base.Error($"Error triggering PlayerHurtEvent: {e.InnerException}");
						}
				}

				bool result = orig_HurtPlayer(info, go, noTeamDamage, IsValidDamage);

				if (player == null)
					return result;

				if (PMConfigFile.enable008 && (info.GetDamageType() == DamageTypes.Scp0492 || info.GetDamageType() == DamageTypes.Poison))
				{
					if (IsKill)
						player.roleType = RoleType.Scp0492;

					else if (info.GetDamageType() == DamageTypes.Scp0492)
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
			catch (Exception)
			{
				return orig_HurtPlayer(info, go);
			}
		}
		public extern void orig_Roundrestart();
		public new void Roundrestart()
		{
			try
			{
				Commands.LightsoutCommand.isLightsout = false;
				PMAlphaWarheadController.nukeLock = false;
			}
			catch (Exception) { }

			orig_Roundrestart();
		}

		public static extern void orig_StaticChangeLevel(bool noShutdownMessage = false);
		public static void StaticChangeLevel(bool noShutdownMessage = false) => orig_StaticChangeLevel(noShutdownMessage);
	}
}
