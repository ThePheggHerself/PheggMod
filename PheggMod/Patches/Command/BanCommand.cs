#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using GameCore;
using RemoteAdmin;
using UnityEngine;
using Utils;
using CommandSystem;

namespace PheggMod.Patches
{
	[MonoModPatch("global::CommandSystem.Commands.RemoteAdmin.BanCommand")]
	public class BanCommand : CommandSystem.Commands.RemoteAdmin.BanCommand
	{
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out string[] newArgs, false);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId/Name array processing.";
				return false;
			}
			if (newArgs == null)
			{
				response = "An error occured while processing this command.\nUsage: " + this.DisplayCommandUsage();
				return false;
			}
			string reason = string.Empty;
			if (newArgs.Length > 1)
			{
				reason = newArgs.Skip(1).Aggregate((string current, string n) => current + " " + n);
			}
			long minutes = 0L;
			try
			{
				minutes = Misc.RelativeTimeToSeconds(newArgs[0], 60);
			}
			catch
			{
				response = "Invalid time: " + newArgs[0];
				return false;
			}
			if (minutes < 0)
			{
				minutes = 0;
				newArgs[0] = "0";
			}
			if (minutes == 0 && !sender.CheckPermission(new PlayerPermissions[]
			{
				PlayerPermissions.KickingAndShortTermBanning,
				PlayerPermissions.BanningUpToDay,
				PlayerPermissions.LongTermBanning
			}, out response))
			{
				return false;
			}
			if (minutes > 0 && minutes <= 3600 && !sender.CheckPermission(PlayerPermissions.KickingAndShortTermBanning, out response))
			{
				return false;
			}
			if (minutes > 3600 && minutes <= 86400 && !sender.CheckPermission(PlayerPermissions.BanningUpToDay, out response))
			{
				return false;
			}
			if (minutes > 86400 && !sender.CheckPermission(PlayerPermissions.LongTermBanning, out response))
			{
				return false;
			}
			ushort bannedPlayercount = 0;
			ushort errors = 0;
			string text2 = string.Empty;
			foreach (ReferenceHub referenceHub in list)
			{
				try
				{
					if (referenceHub == null)
					{
						errors += 1;
					}
					else
					{
						string combinedName = referenceHub.nicknameSync.CombinedName;
						CommandSender commandSender;
						if ((commandSender = (sender as CommandSender)) != null && !commandSender.FullPermissions)
						{
							UserGroup group = ((PMServerRoles)referenceHub.serverRoles).Group;
							byte b = (byte)((group != null) ? group.RequiredKickPower : 0);
							if (b > commandSender.KickPower)
							{
								errors += 1;
								text2 = string.Format("You can't kick/ban {0}. Required kick power: {1}, your kick power: {2}.", combinedName, b, commandSender.KickPower);
								sender.Respond(text2, false);
								continue;
							}
						}
						ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
						{
							sender.LogName,
							" banned player ",
							referenceHub.LoggedNameFromRefHub(),
							". Ban duration: ",
							newArgs[0],
							". Reason: ",
							(reason == string.Empty) ? "(none)" : reason,
							"."
						}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
						if (ServerStatic.GetPermissionsHandler().IsVerified && referenceHub.serverRoles.BypassStaff)
						{
							QueryProcessor.Localplayer.GetComponent<BanPlayer>().BanUser(referenceHub.gameObject, 0L, reason, sender.LogName);
						}
						else
						{
							if (minutes == 0L && ConfigFile.ServerConfig.GetBool("broadcast_kicks", false))
							{
								QueryProcessor.Localplayer.GetComponent<Broadcast>().RpcAddElement(ConfigFile.ServerConfig.GetString("broadcast_kick_text", "%nick% has been kicked from this server.").Replace("%nick%", combinedName), ConfigFile.ServerConfig.GetUShort("broadcast_kick_duration", 5), Broadcast.BroadcastFlags.Normal);
							}
							else if (minutes != 0L && ConfigFile.ServerConfig.GetBool("broadcast_bans", true))
							{
								QueryProcessor.Localplayer.GetComponent<Broadcast>().RpcAddElement(ConfigFile.ServerConfig.GetString("broadcast_ban_text", "%nick% has been banned from this server.").Replace("%nick%", combinedName), ConfigFile.ServerConfig.GetUShort("broadcast_ban_duration", 5), Broadcast.BroadcastFlags.Normal);
							}
							QueryProcessor.Localplayer.GetComponent<BanPlayer>().BanUser(referenceHub.gameObject, minutes, reason, sender.LogName);
						}
						bannedPlayercount += 1;
					}
				}
				catch (Exception ex)
				{
					errors += 1;
					Debug.Log(ex);
					text2 = "Error occured during banning: " + ex.Message + ".\n" + ex.StackTrace;
				}
			}
			if (errors == 0)
			{
				string arg = "Banned";
				int num4;
				if (int.TryParse(newArgs[0], out num4))
				{
					arg = ((num4 > 0) ? "Banned" : "Kicked");
				}
				response = string.Format("Done! {0} {1} player{2}", arg, bannedPlayercount, (bannedPlayercount == 1) ? "!" : "s!");
				return true;
			}
			response = string.Format("Failed to execute the command! Failures: {0}\nLast error log:\n{1}", errors, text2);
			return false;
		}
	}
}
