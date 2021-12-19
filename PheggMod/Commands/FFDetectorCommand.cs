using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
	class FFDetectorCommand : ICommand
	{
		public string Command => "ffdetector";

		public string[] Aliases { get; } = { "antiff", "aff", "rff" };

		public string Description => "Enables/Disables the anti-FF system";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool canRun = CommandManager.CanRun(sender, PlayerPermissions.FriendlyFireDetectorTempDisable, out response);
			if (!canRun)
				return false;

			//FFDetector.FFDetector.DoCheck = !FFDetector.FFDetector.DoCheck;

			//response = $"FriendlyFireDetector has been {(FFDetector.FFDetector.DoCheck ? "enabled" : "disabled")}";
			return true;
		}
	}
}
