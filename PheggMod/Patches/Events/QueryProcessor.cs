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
        public class SCPData
        {
            public RoleType roleType;
            public float health;
            public float artHealth;
            public Vector3 position;
            public float exp;
            public float mana;
        }

        public static Dictionary<RoleType, SCPData> SCP = new Dictionary<RoleType, SCPData>();



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

            //if (refHub.characterClassManager.IsAnyScp() && refHub.characterClassManager.CurClass != RoleType.Scp0492)
            //{
            //    bool HasBeenReplaced = false;

            //    foreach(var p in ReferenceHub.GetAllHubs())
            //    {
            //        if(p.Value.characterClassManager.CurClass == RoleType.Spectator && p.Value.serverRoles.over)
            //    }

            //    if (!HasBeenReplaced && SCP.ContainsKey(refHub.characterClassManager.CurClass))
            //    {
            //        SCPData data = new SCPData
            //        {
            //            roleType = refHub.characterClassManager.CurClass,
            //            health = refHub.playerStats.Health,
            //            artHealth = refHub.playerStats.syncArtificialHealth,
            //            position = refHub.playerMovementSync.RealModelPosition,

            //        };

            //        if(data.roleType == RoleType.Scp079)
            //        {
            //            data.exp = refHub.scp079PlayerScript.Exp;
            //            data.mana = refHub.scp079PlayerScript.Mana;
            //        }

            //        SCP.Add(data.roleType, data);
            //    }
            //}




            if (PlayerManager.players.Count - 1 < 1 && RoundSummary.RoundInProgress())
                PMRoundSummary.RoundFix();
        }
    }
}
