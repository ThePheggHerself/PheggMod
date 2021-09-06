﻿using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
    public class ClearupCommand : ICommand
    {
        public string Command => "clearup";

        public string[] Aliases { get; } = { "cleanup", "deleteragdolls" };

        public string Description => "Deletes all currently active ragdolls";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, out response);
            if (!canRun)
                return false;

            List<Ragdoll> rDs = UnityEngine.Object.FindObjectsOfType<Ragdoll>().ToList();

            for (var p = 0; p < rDs.Count; p++)
            {
                NetworkServer.Destroy(rDs[p].gameObject);
            }

			

			List<Knife.DeferredDecals.Decal> decals = UnityEngine.Object.FindObjectsOfType<Knife.DeferredDecals.Decal>().ToList();

			for (var p = 0; p < rDs.Count; p++)
			{
				Knife.DeferredDecals.DeferredDecalsManager.instance.RemoveDecal(decals[p]);
			}

			response = $"Cleaned up all ragdolls and decals";
            return true;
        }
    }
}
