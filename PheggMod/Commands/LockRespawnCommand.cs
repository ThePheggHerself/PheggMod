using CommandSystem;
using PheggMod.EventTriggers;
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

            PMMTFRespawn.blockRespawn = !PMMTFRespawn.blockRespawn;
            response = $"Respawn lock has been {(PMMTFRespawn.blockRespawn ? "enabled" : "disabled")}";

            return true;
        }
    }
}
