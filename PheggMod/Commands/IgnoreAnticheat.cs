using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
	public class IgnoreAnticheat : ICommand, IUsageProvider
	{
		public string Command { get; } = "ignoreanticheat";

		public string[] Aliases { get; } = { "iac", "ignoreac" };

		public string Description { get; } = "Makes a user bypass the anti-cheat";

		public string[] Usage { get; } = { "%player%" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool canRun = CommandManager.CanRun(sender, PlayerPermissions.Noclip, arguments, new[] { "player" }, out response, out List<ReferenceHub> hubs);
			if (!canRun)
				return false;

			response = "a";
			foreach (var hub in hubs)
				hub.playerMovementSync.NoclipWhitelisted = !hub.playerMovementSync.NoclipWhitelisted;



			response = $"{hubs.Count} players whitelisted against the anti-cheat";

			return true;
		}
	}
}
