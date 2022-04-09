#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using CommandSystem;
using MonoMod;
using PheggMod.API.Commands;
using PheggMod.API.Events;
using PheggMod.Commands;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.Patches
{
	[MonoModPatch("global::RemoteAdmin.CommandProcessor")]
	public class PMCommandProcessor
	{
		internal static string lastCommand;

		public static void PMProcessQuery(string command, CommandSender sender) => ProcessQuery(command, sender);

		public static extern void orig_ProcessQuery(string q, CommandSender sender);
		public static void ProcessQuery(string q, CommandSender sender)
		{
			try
			{
				GameObject go = PlayerManager.players.Where(p => p.GetComponent<NicknameSync>().MyNick == sender.Nickname).FirstOrDefault();

				string[] query = q.Split(new char[] { ' ' });

				if (!q.ToUpper().Contains("SILENT") && !q.ToUpper().StartsWith("$"))
				{
					if (go != null)
					{
						lastCommand = q;
						PheggPlayer pheggPlayer = new PheggPlayer(go);

						if (q.ToUpper().StartsWith("CASSIE"))
							q = q + " PITCH_1";

						try
						{
							Base.Debug("Triggering AdminQueryEvent");
							PluginManager.TriggerEvent<IEventHandlerAdminQuery>(new AdminQueryEvent(pheggPlayer, q));
						}
						catch (Exception e)
						{
							Base.Error($"Error triggering AdminQueryEvent: {e.InnerException}");
						}

						if (PluginManager.TriggerCommand(new CommandInfo(sender, pheggPlayer.gameObject, query[0], query))) return;
					}
					else
					{
						if (PluginManager.TriggerConsoleCommand(new CommandInfo(sender, null, query[0], query))) return;
					}
				}
				orig_ProcessQuery(q, sender);
			}
			catch (Exception e)
			{
				Base.Error($"{e.Message}\n{e.StackTrace}\n{e.InnerException.Message}\n{e.InnerException.StackTrace}");
			}
		}
	}
}
