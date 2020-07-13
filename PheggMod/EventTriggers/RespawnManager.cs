#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using Respawning;
using System;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::RepsawnManager")]
    public class PMRespawnManager : RespawnManager
    {
        public static bool blockRespawn = false;

        //public extern void orig_Spawn();

        //public new void Spawn()
        //{
        //    if (blockRespawn)
        //        return;

        //    bool isCI = NextKnownTeam == SpawnableTeamType.ChaosInsurgency;

        //    orig_Spawn();

        //    try
        //    {
        //        Base.Debug("Triggering RespawnEvent");
        //        PluginManager.TriggerEvent<IEventHandlerRespawn>(new RespawnEvent(isCI));
        //    }
        //    catch (Exception e)
        //    {
        //        Base.Error($"Error triggering RespawnEvent: {e.InnerException}");
        //    }

        //    if (Commands.CustomInternalCommands.isLightsout)
        //    {
        //        foreach (GameObject player in PlayerManager.players)
        //        {
        //            if (player.GetComponent<Inventory>().items.FindIndex(i => i.id == ItemType.Flashlight) < 0)
        //                player.GetComponent<Inventory>().AddNewItem(ItemType.Flashlight);
        //        }
        //    }

        //    if (isCI && PMConfigFile.announceChaos)
        //    {
        //        RespawnEffectsController.PlayCassieAnnouncement(PMConfigFile.chaosAnnouncement, false, true);
        //    }
        //}
    }
}
