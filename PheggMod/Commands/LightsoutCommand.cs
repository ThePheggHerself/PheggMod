using CommandSystem;
using PheggMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem;
using Object = UnityEngine.Object;

namespace PheggMod.Commands
{
	public class LightsoutCommand : ICommand
	{
		public string Command => "lightsout";

		public string[] Aliases => null;

		public string Description => "Disables lights around the facility";

		internal static bool isLightsout = false;
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool canrun = CommandManager.CanRun(sender, PlayerPermissions.FacilityManagement, out response);
			if (!canrun)
				return canrun;

			isLightsout = !isLightsout;
			foreach (FlickerableLightController lightController in Object.FindObjectsOfType<FlickerableLightController>())
			{
				Scp079Interactable interactable = lightController.GetComponent<Scp079Interactable>();
				if (interactable == null || interactable.type != Scp079Interactable.InteractableType.LightController) continue;

				lightController.ServerFlickerLights(isLightsout ? 100000f : 0f);
			}

			if (isLightsout)
				foreach (var player in ReferenceHub.GetAllHubs())
					player.Value.inventory.ServerAddItem(ItemType.Flashlight);

			response = $"Facility lights have been {(isLightsout ? "disabled" : "enabled")}!";
			return canrun;
		}
	}
}
