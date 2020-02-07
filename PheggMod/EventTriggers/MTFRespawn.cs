#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using MonoMod;
using PheggMod.API.Events;

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
