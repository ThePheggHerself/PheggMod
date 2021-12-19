#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using Respawning;
using System;
using System.Diagnostics;
using UnityEngine;
using InventorySystem;
using System.Linq;

namespace PheggMod.Patches
{
    [MonoModPatch("global::Respawning.RespawnManager")]
    public class PMRespawnManager : RespawnManager
    {
		public extern void orig_Spawn();
        public new void Spawn()
        {
            if (RespawnManagerCrap.blockRespawns)
                return;

            RespawnManagerCrap.isCI = NextKnownTeam == SpawnableTeamType.ChaosInsurgency;

            orig_Spawn();
            try
            {
                Base.Debug("Triggering RespawnEvent");
                PluginManager.TriggerEvent<IEventHandlerRespawn>(new RespawnEvent(RespawnManagerCrap.isCI));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering RespawnEvent: {e.InnerException}");
            }

            if (Commands.LightsoutCommand.isLightsout)
                RespawnManagerCrap.TorchSpawnerReplacementDueToShittyCode();

            if (RespawnManagerCrap.isCI && PMConfigFile.announceChaos)
                RespawnEffectsController.PlayCassieAnnouncement(PMConfigFile.chaosAnnouncement, false, true);

			PMPlayerStats.LastRespawn = DateTime.Now;

        }
    }

    public static class RespawnManagerCrap
    {
        //Honestly, fuck this whole new respawning system. fucking useless piece of shit ffs

        public static bool blockRespawns = false;
        internal static bool isCI = false;

        internal static void TorchSpawnerReplacementDueToShittyCode()
        {
            foreach (var player in ReferenceHub.GetAllHubs())
            {
				if (player.Value.isDedicatedServer || player.Value.characterClassManager.CurClass == RoleType.Spectator)
					continue;


				if (!player.Value.inventory.UserInventory.Items.Where(r => r.Value.ItemTypeId == ItemType.Flashlight).Any())
					player.Value.inventory.ServerAddItem(ItemType.Flashlight);
            }
        }
    }
}
