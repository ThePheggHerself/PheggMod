using CommandSystem;
using System;

namespace PheggMod.Commands.NukeCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class NukeParentCommand : ParentCommand
    {
        public override string Command => "nuke";
        public override string[] Aliases { get; }
        public override string Description => "Various misc. nuke commands";

        public NukeParentCommand()
        {
            RegisterCommand(new NukeLockCommand());
            RegisterCommand(new NukeEnableCommand());
            RegisterCommand(new NukeDisableCommand());
        }

        public override void LoadGeneratedCommands()
        {
            Base.Error("Command loading has not been patched!");
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please specify a valid subcommand!";
            return false;
        }
    }
}
