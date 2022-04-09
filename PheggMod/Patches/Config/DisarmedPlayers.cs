#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using MonoMod;
using UnityEngine;
using Mirror;
using PheggMod.API.Events;
using PheggMod.API;
using InventorySystem;
using System.Collections.Generic;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Armor;
using InventorySystem.Configs;
using InventorySystem.Items;
using System.Linq;

namespace PheggMod.Patches
{
	[MonoModPatch("global::InventorySystem.Disarming.DisarmedPlayers")]
	public static class PMDisarmPlayers
	{
		public static extern bool orig_CanDisarm(this ReferenceHub disarmerHub, ReferenceHub targetHub);
		public static bool CanDisarm(this ReferenceHub disarmerHub, ReferenceHub targetHub)
		{
			if (targetHub.characterClassManager.CurClass == RoleType.Tutorial)
				return false;
			else return orig_CanDisarm(disarmerHub, targetHub);
		}
	}
}
