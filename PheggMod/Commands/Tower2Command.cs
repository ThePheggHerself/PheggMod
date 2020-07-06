using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class Tower2Command : ICommand
    {
        public string Command => "tower2";

        public string[] Aliases => null;

        public string Description => "Teleports the player to a second tower on the surface";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, arguments, new[] { "player" }, out response, out List<ReferenceHub> hubs);
            if (!canRun)
                return false;

            foreach (ReferenceHub refhub in hubs)
                refhub.playerMovementSync.OverridePosition(new Vector3(223, 1026, -18), 0);

            response = $"#Teleported {hubs.Count} {(hubs.Count == 1 ? "player" : "players")} to tower 2";
            return true;
        }
    }
}
