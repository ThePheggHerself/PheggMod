#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
using Grenades;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;

namespace PheggMod.EventTriggers
{
	[MonoModPatch("global::Grenades.GrenadeManager")]
	class PMGrenadeManager : GrenadeManager
	{
		private extern IEnumerator<float> orig__ServerThrowGrenade(GrenadeSettings settings, float forceMultiplier, int itemIndex, float delay);

		private IEnumerator<float> _ServerThrowGrenade(GrenadeSettings settings, float forceMultiplier, int itemIndex, float delay)
		{
			IEnumerator<float> result = orig__ServerThrowGrenade(settings, forceMultiplier, itemIndex, delay);

			if (FFDetector.FFDetector.DetectorEnabled)
			{

				CharacterClassManager ccm = gameObject.GetComponent<CharacterClassManager>();

				if (FFDetector.FFDetector.GrenadeThrowers.ContainsKey(ccm.UserId))
					FFDetector.FFDetector.GrenadeThrowers.Remove(ccm.UserId);

				FFDetector.FFDetector.GrenadeThrowers.Add(ccm.UserId, new FFDetector.FFDetector.GrenadeThrower { UserId = ccm.UserId, ThrownAt = DateTime.Now, Role = ccm.CurClass, Team = ccm.CurRole.team });
			}

			try
			{
				Base.Debug("Triggering PlayerThrowGrenadeEvent");
				PluginManager.TriggerEvent<IEventHandlerPlayerThrowGrenade>(new PlayerThrowGrenadeEvent(new PheggPlayer(gameObject), settings));
			}
			catch (Exception e)
			{
				Base.Error($"Error triggering PlayerThrowGrenadeEvent: {e.InnerException}");
			}

			return result;
		}
	}
}
