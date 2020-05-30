#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using CustomPlayerEffects;
using GameCore;
using MonoMod;
using PheggMod.API.Events;
using PlayableScps;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::PlayableScps.Scp096")]
    class PMScp096 : Scp096
    {
        List<int> warnedTargets;

        public extern void orig_AddTarget(GameObject target);
        public new void AddTarget(GameObject target)
        {
            orig_AddTarget(target);

            if (warnedTargets == null)
                warnedTargets = new List<int>();

            Base.Debug($"{target.GetComponent<NicknameSync>().MyNick} is now a target for SCP096");

            if (!PMConfigFile.targetAnnouncement)
                return;

            int id = target.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId;
            ReferenceHub.TryGetHub(id, out ReferenceHub hub);

            if (hub == default(ReferenceHub))
                return;

            try
            {
                if (HasTarget(hub) && !warnedTargets.Contains(id))
                {
                    new PheggPlayer(target).PersonalBroadcast(5, "You are now a target for 096!");
                    warnedTargets.Add(id);
                }
            }
            catch(Exception e)
            {
                Base.Error(e.ToString());
            }
        }

        private extern void orig_EndEnrage();
        private void EndEnrage()
        {
            Base.Debug("Clearing warned targets");

            orig_EndEnrage();
            warnedTargets.Clear();
        }
    }
}
