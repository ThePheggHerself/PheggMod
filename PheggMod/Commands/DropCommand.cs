﻿using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InventorySystem;

namespace PheggMod.Commands
{
    public class DropCommand : ICommand, IUsageProvider
    {
        public string Command => "drop";

        public string[] Aliases { get; } = { "dropall", "dropinv", "strip" };

        public string Description => "Drops all items and ammo from the specified player(s)";

		public string[] Usage { get; } = { "%player%" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, arguments, new[] { "player" }, out response, out List<ReferenceHub> hubs);
            if (!canRun)
                return false;

			foreach (ReferenceHub refhub in hubs)
				refhub.inventory.ServerDropEverything();


            response = $"Player {(hubs.Count > 1 ? "inventories" : "inventory")} dropped";


            return true;
        }
    }
}
