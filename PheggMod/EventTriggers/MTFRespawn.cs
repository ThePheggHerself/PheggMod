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

            Base.Debug($"CITickets: {this.ChaosRespawnTickets} NTFTickets: {this.MtfRespawnTickets} RespawnTickets: {NextWaveRespawnTickets}");

            try
            {
                Base.Debug("Triggering RespawnEvent");
                PluginManager.TriggerEvent<IEventHandlerRespawn>(new RespawnEvent(nextWaveIsCI));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering RespawnEvent: {e.InnerException.ToString()}");
            }

            if (Commands.CustomInternalCommands.isLightsout)
            {
                foreach (GameObject player in PlayerManager.players)
                {
                    if (player.GetComponent<Inventory>().items.FindIndex(i => i.id == ItemType.Flashlight) < 0)
                        player.GetComponent<Inventory>().AddNewItem(ItemType.Flashlight);
                }
            }

            if (nextWaveIsCI && PMConfigFile.announceChaos)
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement(PMConfigFile.chaosAnnouncement, false, true);
            }
        }
    }
}
