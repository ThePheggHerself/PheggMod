using CommandSystem;
using PheggMod.EventTriggers;
using System;
using UnityEngine;

namespace PheggMod.Commands.NukeCommand
{
    [CommandHandler(typeof(NukeParentCommand))]
    public class NukeEnableCommand : ICommand
    {
        public string Command => "on";

        public string[] Aliases { get; } = { "enable" };

        public string Description => "Turns the nuke lever to the \"ON\" position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CheckPermission(sender, PlayerPermissions.WarheadEvents, out bool isSender, out bool hasPerm);

            if (!isSender)
                response = "No CommandSender found";

            else if (!hasPerm)
                response = $"You don't have permission to execute this command.\nMissing permission: " + PlayerPermissions.WarheadEvents;

            else
            {
                PMAlphaWarheadNukesitePanel.Enable();
                response = $"Warhead has been enabled";
            }
            return success;
        }
    }
}
