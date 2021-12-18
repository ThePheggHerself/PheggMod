#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using Hints;
using MEC;
using Mirror;
using MonoMod;
using PheggMod.API;
using PheggMod.API.Events;
using PheggMod.Patches.API;
using PheggMod.CustomEffects;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PheggMod.Patches
{
	[MonoModPatch("global::PlayerStatsSystem.PlayerStats")]
	public class PMPlayerStats : PlayerStatsSystem.PlayerStats
	{
		public static DateTime LastRespawn = new DateTime();

		private bool _eventsLinked = false;

		private extern void orig_Start();
		private void Start()
		{
			orig_Start();

			if (this.isLocalPlayer)
			{
				OnAnyPlayerDamaged += PlayerDamaged;
				OnAnyPlayerDied += PlayerKilled;
			}
		}

		private extern void orig_OnDestroy();
		private void OnDestroy()
		{
			orig_OnDestroy();

			if (this.isLocalPlayer)
			{
				OnAnyPlayerDamaged -= PlayerDamaged;
				OnAnyPlayerDied -= PlayerKilled;
			}
		}

		private void PlayerDamaged(ReferenceHub refhub, DamageHandlerBase handler)
		{
			if (!(handler is StandardDamageHandler standard))
				return;

			try
			{
				Base.Debug("Triggering PlayerHurtEvent");
				PluginManager.TriggerEvent<IEventHandlerPlayerHurt>(new PlayerHurtEvent(new PheggPlayer(refhub), standard));
			}
			catch (Exception e)
			{
				Base.Error($"Error triggering PlayerHurtEvent: {e.InnerException}");
			}

			if (!PMConfigFile.enable008)
				return;

			if(handler is PMScpDamageHandler scpDH && scpDH._translationId == DeathTranslations.Zombie.Id)
			{
				SCP008 effect = refhub.playerEffectsController.GetEffect<SCP008>();

				if (effect == null || !effect.IsEnabled)
					refhub.playerEffectsController.EnableEffect<SCP008>(300f, false);
				else
					effect.Intensity++;
			}
		}

		private void PlayerKilled(ReferenceHub refhub, DamageHandlerBase handler)
		{
			if (!(handler is StandardDamageHandler standard))
				return;

			var pPlayer = new PheggPlayer(refhub);

			try
			{
				Base.Debug("Triggering PlayerDeathEvent");
				PluginManager.TriggerEvent<IEventHandlerPlayerDeath>(new PlayerDeathEvent(new PheggPlayer(refhub), standard));
			}
			catch (Exception e)
			{
				Base.Error($"Error triggering PlayerDeathEvent: {e.InnerException}");
			}

			if (!PMConfigFile.enable008)
				return;

			if ((handler is PMScpDamageHandler scpDH && scpDH._translationId == DeathTranslations.Zombie.Id) || (handler is UniversalDamageHandler uDH && uDH.TranslationId == DeathTranslations.Poisoned.Id))
			{
				try
				{
					Base.Info("AAA");
					pPlayer.roleType = RoleType.Scp0492;
				}
				catch(Exception e)
				{
					Base.Error(e.ToString());
				}
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
