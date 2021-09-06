using CommandSystem;
using PheggMod.Patches;
using System;

namespace PheggMod.Commands.NukeCommand
{
    public class NukeLockCommand : ICommand
    {
        public string Command => "lock";

        public string[] Aliases => new string[] { "nlock", "locknuke" };

        public string Description => "Prevents the nuke from being enables/disabled, and prevents the control panel switch from changing state";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CanRun(sender, PlayerPermissions.WarheadEvents, out response);
            if (!success)
                return false;

            PMAlphaWarheadController.nukeLock = !PMAlphaWarheadController.nukeLock;
            response = $"Warhead lock has been {(PMAlphaWarheadController.nukeLock ? "enabled" : "disabled")}";

            return success;
        }
    }
}
