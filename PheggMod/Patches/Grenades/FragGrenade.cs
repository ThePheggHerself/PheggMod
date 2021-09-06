#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Grenades;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;

namespace PheggMod.Patches
{
	[MonoModPatch("global::Grenades.FragGrenade")]
	class PMFragGrenade : Grenades.FragGrenade
	{
		public extern bool orig_ServersideExplosion();
		public override bool ServersideExplosion()
		{
			if (FFDetector.FFDetector.DetectorEnabled)
			{
				if (FFDetector.FFDetector.GrenadeThrowers.ContainsKey(thrower.hub.characterClassManager.UserId))
				{
					FFDetector.FFDetector.GrenadeThrowers[thrower.hub.characterClassManager.UserId].DetonatePosition = transform.position;
				}
			}

			return orig_ServersideExplosion();
		}
	}
}
