using CommandSystem;
using PheggMod.EventTriggers;
using System;

namespace PheggMod.Commands.NukeCommand
{
    [CommandHandler(typeof(NukeParentCommand))]
    public class NukeLockCommand : ICommand
    {
        public string Command => "lock";

        public string[] Aliases => new string[] { "nlock", "locknuke" };

        public string Description => "Prevents the nuke from being enables/disabled, and prevents the control panel switch from changing state";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CheckPermission(sender, PlayerPermissions.WarheadEvents, out bool isSender, out bool hasPerm);

            if (!isSender)        
                response = "No CommandSender found";

            else if (!hasPerm)     
                response = $"You don't have permission to execute this command.\nMissing permission: " + PlayerPermissions.WarheadEvents;

            else
            {
                PMAlphaWarheadController.nukeLock = !PMAlphaWarheadController.nukeLock;
                response = $"Warhead lock has been {(PMAlphaWarheadController.nukeLock ? "enabled" : "disabled")}";
                
            }
            return success;
        }
    }
}
