#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required

using LightContainmentZoneDecontamination;
using Mirror;
using MonoMod;
using PheggMod.API.Events;
using System;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::DecontaminationController")]
    class PMDecontaminationLCZ : DecontaminationController
    {
        public extern void orig_FinishDecontamination();
        public new void FinishDecontamination()
        {

            try
            {
                Base.Debug("Triggering LczDecontaminateEvent");
                PluginManager.TriggerEvent<IEventHandlerLczDecontaminate>(new LczDecontaminateEvent());
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering LczDecontaminateEvent: {e.InnerException.ToString()}");
            }

            orig_FinishDecontamination();
        }
    }
}
