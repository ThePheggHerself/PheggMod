using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class PocketCommand : ICommand
    {
        public string Command => "pocket";

        public string[] Aliases => null;

        public string Description => "Teleports the player into the pocket dimention";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, arguments, new[] { "player" }, out response, out List<ReferenceHub> hubs);
            if (!canRun)
                return false;

            foreach (ReferenceHub refhub in hubs)
                refhub.playerMovementSync.OverridePosition(Vector3.down * 1998.5f, 0);

            response = $"Teleported {hubs.Count} {(hubs.Count == 1 ? "player" : "players")} to the pocket dimension";
            return true;
        }
    }
}
