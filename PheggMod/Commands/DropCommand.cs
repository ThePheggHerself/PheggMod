using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class DropCommand : ICommand
    {
        public string Command => "drop";

        public string[] Aliases { get; } = { "dropall", "dropinv", "strip", "clear" };

        public string Description => "Drops all items and ammo from the specified player(s)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.WarheadEvents, arguments, new[] { "Player" }, out response, out List<ReferenceHub> players);
            if (!canRun)
                return false;

            for (var i = 0; i < players.Count; i++)
                players[i].inventory.ServerDropAll();

            response = $"Player {(players.Count > 1 ? "inventories" : "inventory")} dropped";


            return true;
        }
    }
}
