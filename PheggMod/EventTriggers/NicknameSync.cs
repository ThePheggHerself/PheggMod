#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;

using PheggMod.API.Events;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::NicknameSync")]
    class PMNicknameSync : NicknameSync
    {
        public extern void orig_SetNick(string nick);
        public void SetNick(string nick)
        {
            orig_SetNick(nick);

            if (nick != null)
            {
                PluginManager.TriggerEvent<IEventHandlerPlayerJoin>(new PlayerJoinEvent(new PheggPlayer(base.gameObject)));
            }
        }
    }
}
