﻿using GameCore;
using MEC;
using Mirror;
using PheggMod.API.Commands;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;

namespace PheggMod.Commands
{
	public class CustomInternalCommands
	{
		internal static char[] validUnits = { 'm', 'h', 'd', 'w', 'M', 'y' };
		internal static TimeSpan GetBanDuration(char unit, int amount)
		{
			switch (unit)
			{
				default:
					return new TimeSpan(0, 0, amount, 0);
				case 'h':
					return new TimeSpan(0, amount, 0, 0);
				case 'd':
					return new TimeSpan(amount, 0, 0, 0);
				case 'w':
					return new TimeSpan(7 * amount, 0, 0, 0);
				case 'M':
					return new TimeSpan(30 * amount, 0, 0, 0);
				case 'y':
					return new TimeSpan(365 * amount, 0, 0, 0);
			}
		}
		public static bool CheckPermissions(CommandSender sender, string queryZero, PlayerPermissions perm)
		{
			if (ServerStatic.IsDedicated && sender.FullPermissions)
			{
				return true;
			}
			if (PermissionsHandler.IsPermitted(sender.Permissions, perm))
			{
				return true;
			}

			sender.RaReply(queryZero.ToUpper() + "#You don't have permissions to execute this command.\nMissing permission: " + perm, false, true, "");
			return false;
		}
		public static List<GameObject> GetPlayersFromString(string users)
		{
			if (users.ToLower() == "*")
				return PlayerManager.players;

			string[] playerStrings = users.Split('.');
			List<GameObject> playerList = new List<GameObject>();

			foreach (string player in playerStrings)
			{
				GameObject go = PlayerManager.players.Where(p => p.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString() == player
				|| p.GetComponent<NicknameSync>().MyNick.ToLower() == player || p.GetComponent<CharacterClassManager>().UserId2 == player).FirstOrDefault();
				if (go.Equals(default(GameObject)) || go == null) continue;
				else
				{
					playerList.Add(go);
				}
			}

			return playerList;
		}

		readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;

		#region RA-Only Commands

		[PMCommand("curpos"), PMParameters("PlayerID"), PMCommandSummary("Tells you your current position")]
		public void cmd_pos(CommandInfo info)
		{
			Vector3 pos = info.gameObject.GetComponent<PlayerMovementSync>().RealModelPosition;

			info.commandSender.RaReply(info.commandName.ToUpper() + $"#Current player position: x={pos.x} y={pos.y} z={pos.z}", true, true, "");
		}

		[PMCommand("getsize"), PMAlias("getscale"), PMParameters("playerid"), PMCommandSummary("Sets the scale of a player"), PMPermission(PlayerPermissions.PlayersManagement), /*PMDisabled(true)*/]
		public void cmd_getsize(CommandInfo info)
		{
			List<GameObject> pList = GetPlayersFromString(info.commandArgs[1]);
			if (pList.Count < 1)
				info.commandSender.RaReply(info.commandName.ToUpper() + $"#No player found", false, true, "");

			else
				info.commandSender.RaReply(info.commandName.ToUpper() + $"#Scale of {pList[0].GetComponent<NicknameSync>().MyNick} is {pList[0].transform.localScale}", true, true, "");
		}

		// The code for the 3 commands was kindly donated to PheggMod by KrypTheBear#3301
		// Many thanks for the code <3

		//[PMCommand("grenade"), PMParameters("player"), PMCommandSummary("Spawns a grenade at a player")]
		public void cmd_grenade(CommandInfo info)
		{
			Vector3 NULL_VECTOR = new Vector3(0, 0, 0);

			List<GameObject> playerList = GetPlayersFromString(info.commandArgs[1]);

			foreach (GameObject plr in playerList)
			{
				if (plr.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
					continue;

				PheggPlayer pp = new PheggPlayer(plr);

				Object.Instantiate<ExplosionGrenade>(new ExplosionGrenade()).GetComponent<ThrowableItem>().ServerThrow(10, 10, NULL_VECTOR);
			}

			info.commandSender.RaReply(info.commandName.ToUpper() + $"#Spawned grenade on {(info.commandArgs[1] == "*" ? "All" : playerList.Count.ToString())} players", true, true, "");
		}
		// [PMCommand("flash"), PMParameters("player"), PMCommandSummary("Spawns a flashbang at a player")]
		public void cmd_flash(CommandInfo info)
		{
			Vector3 NULL_VECTOR = new Vector3(0, 0, 0);

			List<GameObject> playerList = GetPlayersFromString(info.commandArgs[1]);

			foreach (GameObject plr in playerList)
			{
				if (plr.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
					continue;

				PheggPlayer pp = new PheggPlayer(plr);

				// Initialization
				//GrenadeManager gm = pp.gameObject.GetComponent<GrenadeManager>();

				// Finalize creation & Spawn
				//Grenade nade = Object.Instantiate<GameObject>(gm.availableGrenades[1].grenadeInstance).GetComponent<Grenade>();
				//(nade).InitData(gm, NULL_VECTOR, NULL_VECTOR);
				//NetworkServer.Spawn(nade.gameObject);
			}

			info.commandSender.RaReply(info.commandName.ToUpper() + $"#Spawned flash on {(info.commandArgs[1] == "*" ? "All" : playerList.Count.ToString())} players", true, true, "");
		}
		//[PMCommand("ball"), PMParameters("player"), PMCommandSummary("Spawns 018 at a player")]
		public void cmd_ball(CommandInfo info)
		{
			Vector3 NULL_VECTOR = new Vector3(0, 0, 0);

			List<GameObject> playerList = GetPlayersFromString(info.commandArgs[1]);

			foreach (GameObject plr in playerList)
			{
				if (plr.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
					continue;

				PheggPlayer pp = new PheggPlayer(plr);

				// Initialization
				//GrenadeManager gm = pp.gameObject.GetComponent<GrenadeManager>();

				// Finalize creation & Spawn
				//Grenade nade = Object.Instantiate<GameObject>(gm.availableGrenades[2].grenadeInstance).GetComponent<Grenade>();
				//(nade).InitData(gm, NULL_VECTOR, NULL_VECTOR);
				//NetworkServer.Spawn(nade.gameObject);
			}

			info.commandSender.RaReply(info.commandName.ToUpper() + $"#Spawned 018 on {(info.commandArgs[1] == "*" ? "All" : playerList.Count.ToString())} players", true, true, "");
		}

		#endregion

		#region Server Console Commands

		//[PMCommand("status"), PMAlias("serverstatus", "serverinfo", "sinfo"), PMParameters(), PMConsoleRunnable(true)]
		public void cmd_sinfo(CommandInfo info)
		{
			string playerCount = $"{PlayerManager.players.Count} / {ConfigFile.ServerConfig.GetInt("max_players", 20)}";
			string roundCount = Base.roundCount.ToString();
			string roundDuration = RoundSummary.RoundInProgress() == true ? $"{new DateTime(TimeSpan.FromSeconds((DateTime.Now - (DateTime)Base.roundStartTime).TotalSeconds).Ticks):HH:mm:ss}" : "Round not started";
			string timeSinceStart = $"{new DateTime(TimeSpan.FromSeconds((double)(new decimal(Time.realtimeSinceStartup))).Ticks):HH:mm:ss}";
			string curPlayerID = PlayerManager.localPlayer.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString();
			string memory = $"{ ((GC.GetTotalMemory(false) / 1024) / 1024) } MB";


			//(double)(new decimal(Time.realtimeSinceStartup))

			string status = "Server status:"
					+ $"\nPlayer count: {playerCount}"
					+ $"\nRound count: {roundCount}"
					+ $"\nRound duration: {roundDuration}"
					+ $"\nTime since startup: {timeSinceStart}"
					+ $"\nCurrent PlayerID: {curPlayerID}"
					+ $"\nMemory usage: {memory}"
					;


			if (info.gameObject != null)
				info.commandSender.RaReply(info.commandName.ToUpper() + $"#{status}", true, true, "");
			else
				Base.Info(status);

			//+ $"\nMemory usage: {GC.GetTotalMemory(false)}"
		}

		#endregion
	}
}
