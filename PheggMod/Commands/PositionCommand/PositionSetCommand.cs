using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands.PositionCommand
{
    public class PositionSetCommand : ICommand, IUsageProvider
    {
        public string Command => "set";

        public string[] Aliases => null;

        public string Description => "Sets a player's position";

		public string[] Usage { get; } = { "%player%", "X", "Y", "Z" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, arguments, new[] { "player", "X", "Y", "Z" }, out response, out List<ReferenceHub> hubs);
            if (!canRun)
                return false;

            string[] args = arguments.ToArray();

            if (!CommandManager.TryParseVector3(new string[] { args[2], args[3], args[4] }, out Vector3 pos))

            foreach (ReferenceHub refhub in hubs)
            {
                refhub.playerMovementSync.OverridePosition(pos, 0);
            }

            response = $"#Teleported {hubs.Count} {(hubs.Count == 1 ? "player" : "players")} to the specified position";
            return true;
        }
    }
}
