using CommandSystem;
using Interactables.Interobjects.DoorUtils;
using PheggMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
    public class TestCommand : ICommand, IUsageProvider
    {
        public string Command => "nevergonna";

        public string[] Aliases => null;

        public string Description => "Test command. Try it :)";

		public string[] Usage { get; } = { "give", "you", "up" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
			bool canRun = CommandManager.CanRun(sender, null, arguments, new[] { "give", "you", "up" }, out response);
			if (!canRun)
				return false;

			

			foreach (var user in PMReservedSlots.Users)
				Base.Info(user);

			response = $"Never gonna give you up,\nNever gonna let you down.\nNever gonna run around,\nAnd desert you.\nNever gonna make you cry,\nNever gonna say goodbye.\nNever gonna tell a lie,\nAnd hurt you.";
            return true;
        }
    }
}
