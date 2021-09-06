using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;

namespace PheggMod.Commands
{
	public class GrenadeCommand : ICommand, IUsageProvider
	{
		public string Command => "grenade";

		public string[] Aliases => null;

		public string Description => "Spawns a grenade at a player's location";

		public string[] Usage { get; } = { "%player%" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, arguments, new[] { "player" }, out response, out List<ReferenceHub> hubs);
			if (!canRun)
				return false;

			string[] args = arguments.ToArray();

			foreach (ReferenceHub plr in hubs)
			{
				if (plr.characterClassManager.CurClass == RoleType.Spectator)
					continue;

				UnityEngine.Object.Instantiate<ExplosionGrenade>(new ExplosionGrenade()).GetComponent<ThrowableItem>().ServerThrow(10, 10, Vector3.zero);
			}

			response = $"#Spawned grenade on {hubs.Count} {(hubs.Count > 1 ? "players" : "player")}";
			return true;
		}
	}
}