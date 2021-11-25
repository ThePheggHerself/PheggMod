using CommandSystem;
using InventorySystem.Items.Firearms.Ammo;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;

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

			List<Ragdoll> ragdolls = UnityEngine.Object.FindObjectsOfType<Ragdoll>().ToList();
            for (var p = 0; p < ragdolls.Count; p++)
            {
                NetworkServer.Destroy(ragdolls[p].gameObject);
            }			

			List<AmmoPickup> ammo = UnityEngine.Object.FindObjectsOfType<AmmoPickup>().ToList();
			for (var p = 0; p < ammo.Count; p++)
			{
				NetworkServer.Destroy(ammo[p].gameObject);
			}

			response = $"Facility cleaned up!";
            return true;
        }
    }
}
