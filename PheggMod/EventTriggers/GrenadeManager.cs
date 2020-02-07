#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Grenades;
using MonoMod;
using PheggMod.API.Events;
using System.Collections.Generic;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::Grenades.GrenadeManager")]
    class PMGrenadeManager : GrenadeManager
    {
        private extern IEnumerator<float> orig__ServerThrowGrenade(GrenadeSettings settings, float forceMultiplier, int itemIndex, float delay);
        private IEnumerator<float> _ServerThrowGrenade(GrenadeSettings settings, float forceMultiplier, int itemIndex, float delay)
        {
            IEnumerator<float> @return = orig__ServerThrowGrenade(settings, forceMultiplier, itemIndex, delay);

            PluginManager.TriggerEvent<IEventHandlerPlayerThrowGrenade>(new PlayerThrowGrenadeEvent(new PheggPlayer(base.gameObject), settings));

            return @return;
        }
    }
}
