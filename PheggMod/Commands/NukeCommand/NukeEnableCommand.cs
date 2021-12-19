using CommandSystem;
using PheggMod.Patches;
using System;
using UnityEngine;

namespace PheggMod.Commands.NukeCommand
{
    public class NukeEnableCommand : ICommand
    {
        public string Command => "on";

        public string[] Aliases { get; } = { "enable" };

        public string Description => "Turns the nuke lever to the \"ON\" position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CanRun(sender, PlayerPermissions.WarheadEvents, out response);
            if (!success)
                return false;


            //PMAlphaWarheadNukesitePanel.Enable();
            response = $"Warhead has been enabled";

            return success;
        }
    }
}
