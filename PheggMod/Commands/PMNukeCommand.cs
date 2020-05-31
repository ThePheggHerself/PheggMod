using CommandSystem;
using CommandSystem.Commands.Config;
using PheggMod.EventTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PMNukeCommand : ParentCommand
    {
        public override string Command => "nuke";
        public override string[] Aliases { get; }
        public override string Description => "Various miscalanious nuke commands";

        public override void LoadGeneratedCommands()
        {
            throw new NotImplementedException();
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please specify a valid subcommand!";
            return false;
        }
    }

    [CommandHandler(typeof(PMNukeCommand))]
    public class PMNukeOn : ICommand
    {
        public string Command => "enable";

        public string[] Aliases => new string[] { "on", "activate" };

        public string Description => "Turns the nuke lever to the \"ON\" position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            //This would be where a permission check for PlayerPermissions.WarheadEvents would go

            PMAlphaWarheadNukesitePanel.Enable();
            response = "Warhead has been enabled";
            return true;
        }
    }

    [CommandHandler(typeof(PMNukeCommand))]
    public class PMNukeOff : ICommand
    {
        public string Command => "disable";

        public string[] Aliases => new string[] { "of", "deactivate" };

        public string Description => "Turns the nuke lever to the \"OFF\" position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            //This would be where a permission check for PlayerPermissions.WarheadEvents would go

            PMAlphaWarheadNukesitePanel.Disable();
            response = "Warhead has been disabled";
            return true;
        }
    }

    [CommandHandler(typeof(PMNukeCommand))]
    public class PMNukeLock : ICommand
    {
        public string Command => "lock";

        public string[] Aliases => new string[] { "nlock", "locknuke" };

        public string Description => "Turns the nuke lever to the \"ON\" position";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            //This would be where a permission check for PlayerPermissions.WarheadEvents would go

            PMAlphaWarheadController.nukeLock = !PMAlphaWarheadController.nukeLock;
            response = $"#Warhead lock has been {(PMAlphaWarheadController.nukeLock ? "enabled" : "disabled")}";
            return true;
        }
    }
}
