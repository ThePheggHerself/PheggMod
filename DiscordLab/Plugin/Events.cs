﻿using PheggMod.API;
using PheggMod.API.Events;
using PheggMod.API.Plugin;
using PlayerStatsSystem;
using System;

namespace DiscordLab
{
	internal class Events : IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandlerPlayerLeave, IEventHandlerAdminQuery, IEventHandlerGlobalBan, IEventHandlerLczDecontaminate, IEventHandlerPlayerBan,
		IEventHandlerPlayerDeath, IEventHandlerPlayerEscape, IEventHandlerPlayerHurt, IEventHandlerPlayerKick, IEventHandlerPlayerSpawn, IEventHandlerPlayerThrowGrenade, IEventHandlerRespawn, IEventHandlerRoundEnd,
		IEventHandlerRoundStart, IEventHandlerWarheadCancel, IEventHandlerWarheadDetonate, IEventHandlerWarheadStart
	{
		internal static DateTime RoundEnded;
		private DateTime? roundStart = null;

		public void OnAdminQuery(AdminQueryEvent ev)
		{
			DiscordLab.bot.NewMessage($"```yaml\nAdmin: {ev.Admin}\nExecuted: {ev.Query.ToUpper()}```");

			string[] querySplit = ev.Query.Split(' ');

			if (querySplit[0].ToUpper().Contains("MUTE") && !querySplit[0].StartsWith("@") && ev.Query.Split(' ').Length > 1)
			{

				foreach (string plr in querySplit[1].Split('.'))
				{
					if (!int.TryParse(plr, out int plrId))
						continue;

					ReferenceHub hub = ReferenceHub.GetHub(plrId);

					DiscordLab.bot.NewMessage($"{ev.Admin} updated mute status for {hub.nicknameSync.MyNick} ({hub.characterClassManager.UserId})");
				}
			}
		}

		public void OnGlobalBan(GlobalBanEvent ev)
			=> DiscordLab.bot.NewMessage($"{ev.Player} was globally banned for cheating");
		public void OnLczDecontaminate(LczDecontaminateEvent ev)
			=> DiscordLab.bot.NewMessage($"Light containment zone decontamination has begun!");
		public void OnPlayerBan(PlayerBanEvent ev)
			=> DiscordLab.bot.NewMessage($"**New Ban!**```autohotkey\nUser: {ev.Player}\nAdmin: {ev.Admin}\nDuration: {ev.Duration / 60} {(ev.Duration > 1 ? "minutes" : "minute")}\nReason: {(string.IsNullOrEmpty(ev.Reason) ? "No reason provided" : ev.Reason)}```");
		public void OnPlayerEscape(PlayerEscapeEvent ev)
			=> DiscordLab.bot.NewMessage($"{ev.Player.nameClean} escaped the facility and became {ev.newRole}");
		public void OnPlayerJoin(PlayerJoinEvent ev)
			=> DiscordLab.bot.NewMessage($"**{ev.Player.nameClean} ({ev.Player.userId} from ||~~{ev.Player.ipAddress}~~||) has joined the server**");
		public void OnPlayerKick(PlayerKickEvent ev)
			=> DiscordLab.bot.NewMessage($"**Player kicked!**```autohotkey\nUser: {ev.Player}\nAdmin: {(ev.Admin.ToString() == " ()" ? "Server console" : ev.Admin.ToString())}```");
		public void OnPlayerLeave(PlayerLeaveEvent ev)
			=> DiscordLab.bot.NewMessage($"{ev.Player} disconnected from the server.");
		public void OnRespawn(RespawnEvent ev)
			=> DiscordLab.bot.NewMessage(ev.IsCI ? "**Attention all personnel: Chaos insurgency breach in progress**" : "**Mobile Task Force unit Epsilon 11 has entered the facility!**");
		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
			=> DiscordLab.bot.NewMessage($"{ev.Player.nameClean} threw {ev.Grenade}");
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
			=> DiscordLab.bot.NewMessage("Waiting for players...");
		public void OnWarheadCancel(WarheadCancelEvent ev)
			=> DiscordLab.bot.NewMessage($"{ev.Disabler.nameClean} canceled the warhead detonation");
		public void OnWarheadDetonate(WarheadDetonateEvent ev)
			=> DiscordLab.bot.NewMessage("The alpha warhead has been detonated");
		public void OnWarheadStart(WarheadStartEvent ev)
			=> DiscordLab.bot.NewMessage(ev.InitialStart ? $"**Alpha warhead detonation sequence engaged! The underground section of the facility will be detonated in T-minus {ev.TimeToDetonation} seconds!**" : $"**Alpha warhead detonation sequence resumed! {ev.TimeToDetonation} seconds to detonation!**");


		private string _lastSpawnMessage = "a";
		public void OnPlayerSpawn(PlayerSpawnEvent ev)
		{
			string newMsg = $"{ev.Player.nameClean} spawned as {ev.Role}";
			if (_lastSpawnMessage == newMsg) return;

			DiscordLab.bot.NewMessage(newMsg);
			_lastSpawnMessage = newMsg;
		}
		public void OnRoundEnd(RoundEndEvent ev)
		{
			DiscordLab.bot.NewMessage($"**Round Ended**\n```Round Time: {new DateTime(TimeSpan.FromSeconds((DateTime.Now - (DateTime)roundStart).TotalSeconds).Ticks):HH:mm:ss}"
				+ $"\nEscaped Class-D: {ev.Class_D.Escaped_ClassD}/{ev.Class_D.Starting_ClassD}"
				+ $"\nRescued Scientists: {ev.Scientist.Escaped_Scientists}/{ev.Scientist.Starting_Scientists}"
				+ $"\nTerminated SCPs: {ev.SCP.Terminated_SCPs}/{ev.SCP.Starting_SCPs}"
				+ $"\nWarhead Status: {(AlphaWarheadController.Host.detonated == false ? "Not Detonated" : $"Detonated")}```");

			roundStart = null;
			RoundEnded = DateTime.Now;
		}
		public void OnRoundStart(RoundStartEvent ev)
		{
			roundStart = DateTime.Now;
			DiscordLab.bot.NewMessage($"**A new round has begun**");
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!RoundSummary.RoundInProgress() || ev.Player.roleType == RoleType.Spectator) return;

			if (ev.StandardDamageHandler is AttackerDamageHandler aDH)
			{
				PheggMod.PheggPlayer attackerPlayer = new PheggMod.PheggPlayer(aDH.Attacker.Hub);

				if (aDH.IsSuicide)
					DiscordLab.bot.NewMessage($"{ev.Player.nameClean} self-harmed for {Math.Round(aDH.Damage)} with {GetDamageSource(aDH)}");
				else if (IsTeamDamage(ev.Player.team, attackerPlayer.team))
					DiscordLab.bot.NewMessage($"**{attackerPlayer.roleType} {attackerPlayer} attacked {ev.Player.roleType} {ev.Player} for {Math.Round(aDH.Damage)} with {GetDamageSource(aDH)}**");
				else if (ev.Player.Disarmed && !attackerPlayer.refHub.characterClassManager.IsAnyScp())
					DiscordLab.bot.NewMessage($"__{attackerPlayer.roleType} {attackerPlayer} attacked {ev.Player.roleType} {ev.Player} for {Math.Round(aDH.Damage)} with {GetDamageSource(aDH)}__");
				else
					DiscordLab.bot.NewMessage($"{attackerPlayer.nameClean} -> {ev.Player.nameClean} -> {Math.Round(aDH.Damage)} ({GetDamageSource(aDH)})");
			}
			else if (ev.StandardDamageHandler is WarheadDamageHandler wDH)
			{
				DiscordLab.bot.NewMessage($"{ev.Player.nameClean} was partially vaporized by alpha warhead");
			}
		}


		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!RoundSummary.RoundInProgress() || ev.Player.roleType == RoleType.Spectator) return;

			if (ev.StandardDamageHandler is AttackerDamageHandler aDH)
			{
				PheggMod.PheggPlayer attackerPlayer = new PheggMod.PheggPlayer(aDH.Attacker.Hub);

				if (aDH.IsSuicide)
					DiscordLab.bot.NewMessage($"{ev.Player.nameClean} committed suicide with {GetDamageSource(aDH)}");
				else if (IsTeamDamage(ev.Player.team, attackerPlayer.team))
					DiscordLab.bot.NewMessage($"**Teamkill** \n```autohotkey\nPlayer: {attackerPlayer.roleType} {attackerPlayer} \nKilled: {ev.Player.roleType} {ev.Player}\nUsing: {GetDamageSource(aDH)}```");
				else if (ev.Player.Disarmed && !attackerPlayer.refHub.characterClassManager.IsAnyScp())
					DiscordLab.bot.NewMessage($"__Disarmed Kill__\n```autohotkey\nPlayer: {attackerPlayer.roleType} {attackerPlayer} \nKilled: {ev.Player.roleType} {ev.Player}\nUsing: {GetDamageSource(aDH)}```");
				else
					DiscordLab.bot.NewMessage($"{attackerPlayer.roleType} {attackerPlayer.nameClean} killed {ev.Player.roleType} {ev.Player.nameClean} using {GetDamageSource(aDH)}");
			}
			else if (ev.StandardDamageHandler is WarheadDamageHandler wDH)
			{
				DiscordLab.bot.NewMessage($"{ev.Player.nameClean} was vaporized by alpha warhead");
			}
			else if (ev.StandardDamageHandler is UniversalDamageHandler uDH)
			{
				DiscordLab.bot.NewMessage($"{ev.Player.nameClean} died to {DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel}");
			}
		}

		public static bool IsTeamDamage(Team player, Team attacker)
		{
			if (player == attacker)
				return true;
			else if ((player == Team.CDP || player == Team.CHI) && (attacker == Team.CDP || attacker == Team.CHI))
				return true;
			else if ((player == Team.RSC || player == Team.MTF) && (attacker == Team.RSC || attacker == Team.MTF))
				return true;
			else return false;
		}
		public static string GetDamageSource(AttackerDamageHandler aDH)
		{
			if (aDH is FirearmDamageHandler fDH)
				return fDH.WeaponType.ToString();
			else if (aDH is ExplosionDamageHandler eDH)
				return "Grenade";
			else if (aDH is MicroHidDamageHandler mhidDH)
				return "Micro HID";
			else if (aDH is RecontainmentDamageHandler reconDH)
				return "Recontainment";
			else if (aDH is Scp018DamageHandler scp018DH)
				return "SCP 018";
			else if (aDH is Scp096DamageHandler scp096DH)
				return "SCP 096";
			else if (aDH is ScpDamageHandler scpDH)
				return scpDH.Attacker.Hub.characterClassManager.CurClass.ToString();



			else return "THIS SHOULD NEVER APPEAR!!!";
		}

		public static string BullshitNameChange(RoleType role)
		{
			switch (role)
			{
				case RoleType.Scp173:
					return "173";
				case RoleType.ClassD:
					return "ClassD";
				case RoleType.Spectator:
					return "Spectator";
				///...
				///Skip all the classes

				default:
					return "Whatever";
			}
		}

		public static string BullshitNameChange2(ItemType Item)
		{
			switch (Item)
			{
				case ItemType.KeycardJanitor:
					return "Janitor";
				case ItemType.KeycardScientist:
					return "Scientist";
				case ItemType.KeycardResearchCoordinator:
					return "Research";
				///...
				///Skip all the items
				case ItemType.GunCOM15:
					return "COM15";
				case ItemType.GunE11SR:
					return "E11 Rifle";

				default:
					return "Whatever";
			}
		}
	}
}