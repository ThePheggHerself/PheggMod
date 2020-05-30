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
    [MonoModPatch("global::Scp096")]
    class PMScp096 : Scp096
    {
        public extern void orig_AddTarget(GameObject target);
        public new void AddTarget(GameObject target)
        {
            orig_AddTarget(target);
            if (!this.Calming && RemainingEnrageCooldown > 0)
            {
                if (PMConfigFile.targetAnnouncement)
                    new PheggPlayer(target).PersonalBroadcast(5, "You are a target for 096!");
            }
        }
    }
}
