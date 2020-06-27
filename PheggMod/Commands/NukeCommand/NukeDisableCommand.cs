using CommandSystem;
using PheggMod.EventTriggers;
using System;
using UnityEngine;



namespace PheggMod.Commands.NukeCommand
{
    public class NukeDisableCommand : ICommand
    {
        public string Command => "off";

        public string[] Aliases { get; } = { "disable" };

        public string Description => "Turns the nuke lever to the \"OFF\" position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CanRun(sender, PlayerPermissions.WarheadEvents, out response);
            if (!success)
                return false;

            PMAlphaWarheadNukesitePanel.Disable();
            response = $"Warhead has been disabled";

            return success;
        }
    }
}
