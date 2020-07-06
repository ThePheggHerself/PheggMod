using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands.PositionCommand
{
    public class PositionParentCommand : ParentCommand
    {
        public override string Command => "position";

        public override string[] Aliases { get; } = { "pos" };

        public override string Description => "Various position based commands";

        public PositionParentCommand()
        {
            RegisterCommand(new PositionSetCommand());
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
