#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::MTFRespawn")]
    class PMMTFRespawn : MTFRespawn
    {
        public extern void orig_RespawnDeadPlayers();
        public void RespawnDeadPlayers()
        {
            orig_RespawnDeadPlayers();

            try
            {
                PluginManager.TriggerEvent<IEventHandlerRespawn>(new RespawnEvent(nextWaveIsCI));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering RespawnEvent: {e.ToString()}");
            }

            if (Commands.CustomInternalCommands.isLightsout)
            {
                foreach (GameObject player in PlayerManager.players)
                {
                    if (player.GetComponent<Inventory>().items.FindIndex(i => i.id == ItemType.Flashlight) < 0)
                        player.GetComponent<Inventory>().AddNewItem(ItemType.Flashlight);
                }
            }

            if (nextWaveIsCI && ConfigFile.ServerConfig.GetBool("announce_chaos_spawn", true))
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement(ConfigFile.ServerConfig.GetString("chaos_announcement", "PITCH_1 ATTENTION ALL PERSONNEL . CHAOS INSURGENCY BREACH IN PROGRESS"), false, true);
            }
        }
    }
}
