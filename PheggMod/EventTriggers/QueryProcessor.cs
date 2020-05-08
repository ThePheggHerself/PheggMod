#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Linq;

namespace PheggMod.EventTriggers
{
    class QueryProcessor
    {
        [MonoModPatch("global::RemoteAdmin.QueryProcessor")]
        class PMQueryProcessor : RemoteAdmin.QueryProcessor
        {
            public extern void orig_OnDestroy();
            public void OnDestroy()
            {
                orig_OnDestroy();

                CharacterClassManager player = this.GetComponent<CharacterClassManager>();
                NicknameSync name = this.GetComponent<NicknameSync>();

                if (string.IsNullOrEmpty(player.UserId) || !player.UserId.Contains('@')) return;
                else try
                    {
                        PluginManager.TriggerEvent<IEventHandlerPlayerLeave>(new PlayerLeaveEvent(new PheggPlayer(this.gameObject)));
                    }
                    catch (Exception e)
                    {
                        Base.Error($"Error triggering PlayerLeaveEvent: {e.ToString()}");
                    }

                if (PlayerManager.players.Count - 1 < 1 && RoundSummary.RoundInProgress())
                    PMRoundSummary.RoundFix();
            }
        }
    }
}
