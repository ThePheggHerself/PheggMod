#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using Mirror;
using System.Net;
using MEC;
using GameCore;

using PheggMod.API.Events;
using PheggMod.API.Plugin;
using PheggMod.API.Commands;
using PheggMod.Commands;

namespace PheggMod
{
	public class PluginManager
	{
		public static List<Assembly> plugins = new List<Assembly>();
		public static List<Tuple<Type, IEventHandler>> allEvents = new List<Tuple<Type, IEventHandler>>();

		public static Dictionary<string, MethodInfo> allCommands = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
		public static Dictionary<string, ICommand> oldCommands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
		public static Dictionary<MethodInfo, object> commandInstances = new Dictionary<MethodInfo, object>();

		public static void Reload()
		{
			Base.Info($"Reloading plugins. Clearing lists and dictionaries");

			plugins.Clear();
			allEvents.Clear();
			allCommands.Clear();
			commandInstances.Clear();

			PluginPreLoad();
		}

		public static void PluginPreLoad()
		{
			bool universalConfigs = ConfigFile.ServerConfig.GetBool("universal_config_file", false);
			string pluginsFolder = !universalConfigs ? FileManager.GetAppFolder(true, true) + "plugins" : AppDomain.CurrentDomain.BaseDirectory + FileManager.GetPathSeparator() + "plugins" + "/../plugins";

			Base.Info("Plugin folder: " + pluginsFolder + " | "  + universalConfigs);

			AddCommands(Assembly.GetExecutingAssembly());
			CommandManager.RegisterInternalCommands();

			LoadDependencies(pluginsFolder + "/Dependencies");
			LoadPlugins(pluginsFolder);
		}

		private static void LoadDependencies(string pluginsFolder)
		{
			if (Directory.Exists(pluginsFolder))
			{
				Base.Info("Loading dependancies...");

				List<string> files = Directory.GetFiles(pluginsFolder).ToList<string>();
				foreach (string dllfile in files)
				{
					if (dllfile.EndsWith(".dll"))
					{
						Assembly asm = Assembly.LoadFrom(dllfile);
						Base.Info("DEPENDENCY LOADER | Loading dependency " + asm.GetName().Name);
					}
				}
				Base.Info("Dependancies loaded!");
			}
			else
			{
				Base.Info("No dependancies folder detected");
			}
		}

		private static void LoadPlugins(string pluginsFolder)
		{
			if (Directory.Exists(pluginsFolder))
			{
				Base.Info("Loading plugins...");

				List<string> files = Directory.GetFiles(pluginsFolder).ToList<string>();

				foreach (string dllfile in files)
				{
					if (dllfile.EndsWith(".dll"))
					{
						Assembly asm = Assembly.LoadFrom(dllfile);

						Base.Info("PLUGIN LOADER | Loading plugin " + asm.GetName().Name);
						try
						{
							foreach (Type t in asm.GetTypes())
							{
								if (t.IsSubclassOf(typeof(Plugin)) && t != typeof(Plugin))
								{
									plugins.Add(asm);

									object obj = Activator.CreateInstance(t);

									try
									{
										MethodInfo method = t.GetMethod("initializePlugin");
										string aaa = (string)method.Invoke(obj, null);

										AddCommands(asm);
									}
									catch (Exception e)
									{
										Base.Error("PLUGIN LOADER | Failed to load file: " + asm.GetName().Name);
										Base.Error($"PLUGIN LOADER | {e.Message}");
										Base.Error($"PLUGIN LOADER | {e.InnerException}");
									};
								}
							}
						}
						catch (Exception e)
						{
							Base.Error("PLUGIN LOADER | Failed to load file: " + asm.GetName().Name);
							Base.Error($"PLUGIN LOADER | {e.Message}");
							Base.Error($"PLUGIN LOADER | {e.InnerException}");
						}
					}
				}
				Base.Info("Plugins loaded!");
			}
			else
			{
				Base.Info("No plugin folder detected");
			}
		}

		public static void AddEventHandlers(Plugin plugin, IEventHandler handler)
		{
			foreach (Type intface in handler.GetType().GetInterfaces())
			{
				if (typeof(IEventHandler).IsAssignableFrom(intface) && intface != typeof(IEventHandler))
				{
					allEvents.Add(new Tuple<Type, IEventHandler>(intface, handler));
				}
			}
		}

		internal static void TriggerEvent<t1>(EventStage st, Event ev) where t1 : IEventHandler
		{
			try
			{
				ev.Stage = st;

				Base.Debug($"Triggering {st} {ev.GetType().Name}");
				PluginManager.TriggerEvent<t1>(ev);
			}
			catch (Exception e)
			{
				Base.Error($"Error {st} {typeof(Event).Name}: {e.InnerException}");
			}
		}

		internal static void TriggerEvent<t1>(Event ev) where t1 : IEventHandler
		{
			foreach (t1 handler in GetEvents<t1>())
			{
				try
				{
					if (handler is t1 thandler) ev.ExecuteHandler(thandler);
				}
				catch (Exception e) { Base.Error($"{e.Message}\n{e.StackTrace}"); }
			}
		}
		internal static List<IEventHandler> GetEvents<t1>() where t1 : IEventHandler
		{
			List<IEventHandler> events = new List<IEventHandler>();

			foreach (Tuple<Type, IEventHandler> handler in allEvents)
			{
				if (typeof(t1).IsAssignableFrom(handler.Item1))
				{
					events.Add(handler.Item2);
				}
			}

			return events;
		}

		internal static void AddCommands(Assembly assembly)
		{
			List<MethodInfo> commands = assembly.GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes().OfType<PMCommand>().Any()).ToList();

			foreach (MethodInfo command in commands)
			{
				if (((PMDisabled)command.GetCustomAttribute(typeof(PMDisabled))) != null && ((PMDisabled)command.GetCustomAttribute(typeof(PMDisabled))).disabled)
					continue;

				PMCommand pmCommand = (PMCommand)command.GetCustomAttribute(typeof(PMCommand));

				if (allCommands.ContainsKey(pmCommand.name) || oldCommands.ContainsKey(pmCommand.name))
				{
					Base.Warn($"Plugin {assembly.GetName().Name} tried to register pre-existing command {pmCommand.name}");
				}
				else
				{
					allCommands.Add(pmCommand.name, command);
				}

				PMAlias pmAlias = (PMAlias)command.GetCustomAttribute(typeof(PMAlias));
				if (pmAlias != null)
				{
					foreach (string alias in pmAlias.alias)
					{
						if (!allCommands.ContainsKey(alias) && !oldCommands.ContainsKey(alias))
						{
							allCommands.Add(alias, command);
						}
						else
						{
							Base.Warn($"Plugin {assembly.GetName().Name} tried to register pre-existing command {alias}");
						}
					}
				}
			}
		}
		internal static bool TriggerCommand(CommandInfo cInfo)
		{
			if (!allCommands.ContainsKey(cInfo.commandName)) return false;

			MethodInfo cmd = allCommands[cInfo.commandName];
			if (cmd == null || cmd.Equals(default(Type)))
				return false;

			UserGroup uGroup = ServerStatic.GetPermissionsHandler().GetUserGroup(cInfo.gameObject.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString());

			PMOverrideRanks overrideRanks = (PMOverrideRanks)cmd.GetCustomAttribute(typeof(PMOverrideRanks));
			if (overrideRanks == null || !overrideRanks.ranks.Contains(uGroup.ToString()))
			{
				PMPermission perm = (PMPermission)cmd.GetCustomAttribute(typeof(PMPermission));
				if (perm != null && !perm.CheckPermissions(cInfo.commandSender, perm.perm))
				{
					cInfo.commandSender.RaReply(cInfo.commandName + "#You don't have permission to execute this command.\nMissing permission: " + perm.perm, false, true, "");
					return true;
				}

				PMPermissions perms = (PMPermissions)cmd.GetCustomAttribute(typeof(PMPermissions));
				if (perms != null)
				{
					PlayerPermissions[] permList = perms.CheckPermissions(cInfo.commandSender, perms.perms).ToArray();
					if (permList != null)
					{
						if (perms.type == RequirementType.Single)
							cInfo.commandSender.RaReply(cInfo.commandName + $"#You don't have permission to execute this command.\nYou must have one of the following: {string.Join(", ", permList)}", false, true, "");
						else if (perms.type == RequirementType.All)
							cInfo.commandSender.RaReply(cInfo.commandName + $"#You must have one of the following permmissions to run this command.\nMissing permissions: {string.Join(", ", permList)}", false, true, "");

						return true;
					}
				}

				PMRankWhitelist whitelist = (PMRankWhitelist)cmd.GetCustomAttribute(typeof(PMRankWhitelist));
				if (whitelist != null && !whitelist.ranks.Contains(uGroup.ToString()))
				{
					cInfo.commandSender.RaReply(cInfo.commandName + "#You are not whitelisted to run this command.", false, true, "");
					return true;
				}

				PMRankBlacklist blacklist = (PMRankBlacklist)cmd.GetCustomAttribute(typeof(PMRankBlacklist));
				if (blacklist != null && blacklist.ranks.Contains(uGroup.ToString()))
				{
					cInfo.commandSender.RaReply(cInfo.commandName + "#You are not whitelisted to run this command.", false, true, "");
					return true;
				}
			}

			PMParameters parameters = (PMParameters)cmd.GetCustomAttribute(typeof(PMParameters));
			PMCanExtend pmCanExtend = (PMCanExtend)cmd.GetCustomAttribute(typeof(PMCanExtend));

			bool canExtend = (pmCanExtend != null ? pmCanExtend.canExtend : false);
			if (parameters == null)
			{
				throw new Exception($"PMParameters is null for command: {cInfo.commandName}");
			}

			if (cInfo.commandArgs.Length - 1 < parameters.parameters.Length)
			{
				cInfo.commandSender.RaReply(cInfo.commandName + $"#{cInfo.commandName.ToUpper()} [{string.Join("] [", parameters.parameters).ToUpper()}]", false, true, "");
				return true;
			}
			if (!canExtend && cInfo.commandArgs.Length - 1 > parameters.parameters.Length)
			{
				cInfo.commandSender.RaReply(cInfo.commandName + $"#{cInfo.commandName.ToUpper()} [{string.Join("] [", parameters.parameters).ToUpper()}]", false, true, "");
				return true;
			}

			object instance;

			if (commandInstances.ContainsKey(cmd))
				instance = commandInstances[cmd];
			else
			{
				instance = Activator.CreateInstance(cmd.DeclaringType);

				commandInstances.Add(cmd, instance);
			}

			cmd.Invoke(instance, new object[] { cInfo });
			return true;
		}
		internal static bool TriggerConsoleCommand(CommandInfo cInfo)
		{
			if (!allCommands.ContainsKey(cInfo.commandName)) return false;

			MethodInfo cmd = allCommands[cInfo.commandName];
			if (cmd == null || cmd.Equals(default(Type)))
				return false;

			PMConsoleRunnable cR = (PMConsoleRunnable)cmd.GetCustomAttribute(typeof(PMConsoleRunnable));
			if (cR == null || !cR.consoleRunnable)
			{
				Base.Info($"{cInfo.commandName.ToUpper()} is not runnable via the server console");
				return false;
			}

			object instance;

			if (commandInstances.ContainsKey(cmd))
				instance = commandInstances[cmd];
			else
			{
				instance = Activator.CreateInstance(cmd.DeclaringType);

				commandInstances.Add(cmd, instance);
			}

			cmd.Invoke(instance, new object[] { cInfo });
			return true;
		}

		public static void AddCommand(CommandSystem.ICommand command) => RemoteAdmin.CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);




		[Obsolete("Use the newer attribute based command system. This was only (re)added for backwards compatability!")]
		public static void AddCommand(Plugin plugin, ICommand command, string name, string[] alias)
		{
			//Base.Info(name);

			if (allCommands.ContainsKey(name) || oldCommands.ContainsKey(name))
			{
				Base.Error($"{plugin.Details.name} tried to register a pre-existing command: {name.ToUpper()}");
			}
			else
			{
				oldCommands.Add(name.ToUpper(), command);
			}

			if (alias != null)
			{
				foreach (string cmdalias in alias)
				{
					if (!allCommands.ContainsKey(cmdalias) && !oldCommands.ContainsKey(cmdalias))
					{
						oldCommands.Add(cmdalias.ToUpper(), command);
					}
				}
			}
		}
		[Obsolete("Use the newer attribute based command system. This was only (re)added for backwards compatability!")]
		internal static void TriggerCommand(ICommand iCmd, string command, GameObject admin, CommandSender sender)
		{
			iCmd.HandleCommand(command, admin, sender);
		}
	}
}
