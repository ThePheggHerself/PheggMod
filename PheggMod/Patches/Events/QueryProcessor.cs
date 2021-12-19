#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Mirror;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.Patches
{
    [MonoModPatch("global::RemoteAdmin.QueryProcessor")]
    class PMQueryProcessor : RemoteAdmin.QueryProcessor
    {
        public extern void orig_OnDestroy();
        public void OnDestroy()
        {
            orig_OnDestroy();

            ReferenceHub refHub = ReferenceHub.GetHub(gameObject);

            if (string.IsNullOrEmpty(refHub.characterClassManager.UserId) || !refHub.characterClassManager.UserId.Contains('@')) return;
            else try
                {
                    Base.Debug("Triggering PlayerLeaveEvent");
                    PluginManager.TriggerEvent<IEventHandlerPlayerLeave>(new PlayerLeaveEvent(new PheggPlayer(this.gameObject)));
                }
                catch (Exception e)
                {
                    Base.Error($"Error triggering PlayerLeaveEvent: {e.InnerException}");
                }

            if (PlayerManager.players.Count - 1 < 1 && RoundSummary.RoundInProgress())
                PMRoundSummary.RoundFix();
        }
    }
}
