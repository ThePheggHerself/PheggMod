#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Mirror;
using MonoMod;
using PheggMod.API.Events;
using System;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::NicknameSync")]
    class PMNicknameSync : NicknameSync
    {
        public extern void orig_SetNick(string nick);
        public void SetNick(string nick)
        {
            orig_SetNick(nick);

            GameObject go = gameObject;
            CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();

            if (ccm.isLocalPlayer)
                return;

            try
            {
                if (nick != null && !string.IsNullOrEmpty(nick))
                {
                    try
                    {
                        Base.Debug("Triggering PlayerJoinEvent");
                        PluginManager.TriggerEvent<IEventHandlerPlayerJoin>(new PlayerJoinEvent(new PheggPlayer(gameObject)));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerJoinEvent: {e.InnerException}");
                    }
                }
            }
            catch (Exception e)
            {
                Base.Error($"Error: {e.InnerException}");
            }

			go.GetComponent<Broadcast>().TargetAddElement(go.GetComponent<NetworkConnection>(), "Welcome to DragonSCP. We are currently testing an experimental Friendly Fire system on our servers.", 10, Broadcast.BroadcastFlags.Normal);
			go.GetComponent<Broadcast>().TargetAddElement(go.GetComponent<NetworkConnection>(), "If you wish to give feedback, feel free to do so on our discord. discord.gg/NUCDjPn", 10, Broadcast.BroadcastFlags.Normal);
		}
    }
}
