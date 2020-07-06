using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
    public class TestCommand : ICommand
    {
        public string Command => "nevergonna";

        public string[] Aliases => null;

        public string Description => "Test command. Try it :)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, null, arguments, new[] { "give", "you", "up" }, out response, out List<ReferenceHub> hubs);
            if (!canRun)
                return false;

            response = $"Never gonna give you up,\nNever gonna let you down.\nNever gonna run around,\nAnd desert you.\nNever gonna make you cry,\nNever gonna say goodbye.\nNever gonna tell a lie,\nAnd hurt you.";
            return true;
        }
    }
}
