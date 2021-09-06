using CommandSystem;
using PheggMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
    public class LockRespawnCommand : ICommand
    {
        public string Command => "spawnlock";

        public string[] Aliases { get; } = { "blockrespawn" };

        public string Description => "Prevents a respawn from happening";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CanRun(sender, PlayerPermissions.RespawnEvents, out response);
            if (!success)
                return false;

            RespawnManagerCrap.blockRespawns = !RespawnManagerCrap.blockRespawns;
            response = $"Respawn lock has been {(RespawnManagerCrap.blockRespawns ? "enabled" : "disabled")}";

            return true;
        }
    }
}
