#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using LightContainmentZoneDecontamination;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Linq;
using UnityEngine;
using Interactables.Interobjects.DoorUtils;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::DecontaminationController")]
    class PMDecontaminationLCZ : DecontaminationController
    {
        public extern void orig_FinishDecontamination();
        public void FinishDecontamination()
        {
            orig_FinishDecontamination();

            try
            {
                Base.Debug("Triggering LczDecontaminateEvent");
                PluginManager.TriggerEvent<IEventHandlerLczDecontaminate>(new LczDecontaminateEvent());
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering LczDecontaminateEvent: {e.InnerException}");
            }
        }
    }
}
