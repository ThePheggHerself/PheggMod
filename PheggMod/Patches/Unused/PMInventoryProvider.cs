//#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
//using System;
//using MonoMod;
//using UnityEngine;
//using Mirror;
//using PheggMod.API.Events;
//using PheggMod.API;
//using InventorySystem;
//using System.Collections.Generic;
//using InventorySystem.Items.Pickups;
//using InventorySystem.Items.Armor;
//using InventorySystem.Configs;
//using InventorySystem.Items;
//using System.Linq;

//namespace PheggMod.Patches
//{
//	[MonoModPatch("global::InventorySystem.InventoryItemProvider")]
//	internal class PMInventoryProvider
//	{
//		public static Action<ReferenceHub, ItemBase> OnItemProvided;

//		private static readonly Dictionary<ReferenceHub, List<ItemPickupBase>> PreviousInventoryPickups = new Dictionary<ReferenceHub, List<ItemPickupBase>>();

//		private static void RoleChanged(ReferenceHub ply, RoleType prevRole, RoleType newRole, bool lite, CharacterClassManager.SpawnReason spawnReason)
//		{
//			try
//			{
//				if (!NetworkServer.active) return;

//				Inventory inv = ply.inventory;
//				HashSet<ushort> oldItems = new HashSet<ushort>(inv.UserInventory.Items.Keys);

//				bool HasBall = false;
//				bool HasLight = false;

//				if (spawnReason == CharacterClassManager.SpawnReason.Escaped && prevRole != newRole)
//				{
//					List<ItemPickupBase> prevInventory = new List<ItemPickupBase>();

//					if (inv.TryGetBodyArmor(out BodyArmor currentArmor))
//						currentArmor.DontRemoveExcessOnDrop = true;

//					while (inv.UserInventory.Items.Count > 0)
//					{
//						var item = inv.UserInventory.Items.ElementAt(0);

//						if (item.Value.ItemTypeId == ItemType.SCP2176)
//						{
//							HasLight = true;
//							inv.ServerRemoveItem(item.Key, null);
//						}
//						else if (item.Value.ItemTypeId == ItemType.SCP018)
//						{
//							HasBall = true;
//							inv.ServerRemoveItem(item.Key, null);
//						}
//						else
//						{
//							var ipb = inv.ServerDropItem(inv.UserInventory.Items.ElementAt(0).Key);
//							prevInventory.Add(inv.ServerDropItem(item.Key));
//						}
//					}

//					PreviousInventoryPickups[ply] = prevInventory;
//				}
//				else
//				{
//					while (inv.UserInventory.Items.Count > 0)
//						inv.ServerRemoveItem(inv.UserInventory.Items.ElementAt(0).Key, null);

//					inv.UserInventory.ReserveAmmo.Clear();
//					inv.SendAmmoNextFrame = true;
//				}

//				if (StartingInventories.DefinedInventories.TryGetValue(newRole, out InventoryRoleInfo info))
//				{
//					var items = info.Items.ToList();

//					if (HasBall)
//						items.Add(ItemType.SCP018);
//					if (HasLight)
//						items.Add(ItemType.SCP2176);

//					foreach (KeyValuePair<ItemType, ushort> ammo in info.Ammo)
//						inv.ServerAddAmmo(ammo.Key, ammo.Value);

//					foreach (var i in items)
//					{
//						ItemBase ib = inv.ServerAddItem(i, 0, null);
//						OnItemProvided?.Invoke(ply, ib);
//					}
//				}

//			}
//			catch (Exception e)
//			{
//				Base.Error(e.ToString());
//			}
//		}
//	}
//}
