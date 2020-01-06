#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using Mirror;
using System.Net;
using MEC;

using PheggMod.API.Events;
using GameCore;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::MTFRespawn")]
    class PMMTFRespawn : MTFRespawn
    {
        public extern void orig_RespawnDeadPlayers();
        public new void RespawnDeadPlayers()
        {
            orig_RespawnDeadPlayers();

            PluginManager.TriggerEvent<IEventHandlerRespawn>(new RespawnEvent(nextWaveIsCI));

            if (ConfigFile.ServerConfig.GetBool("announce_chaos_spawn", true))
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement(ConfigFile.ServerConfig.GetString("chaos_announcement", "ATTENTION ALL PERSONNEL . CHAOS INSURGENCY BREACH IN PROGRESS"), false, true);
            }
        }
    }
}
