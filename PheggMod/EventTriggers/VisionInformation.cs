#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Hints;
using MonoMod;
using PlayableScps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PheggMod.EventTriggers
{
	[MonoModPatch("global::PlayableScps.VisionInformation")]
	public readonly struct PMVisionInformation
	{

		public static extern VisionInformation orig_GetVisionInformation(ReferenceHub source, Vector3 target, float targetRadius = 0, float visionTriggerDistance = 0, bool checkFog = true, bool checkLineOfSight = true, LocalCurrentRoomEffects targetLightCheck = null);
		public static VisionInformation GetVisionInformation(ReferenceHub source, Vector3 target, float targetRadius = 0, float visionTriggerDistance = 0, bool checkFog = true, bool checkLineOfSight = true, LocalCurrentRoomEffects targetLightCheck = null)
		{
			if (source.characterClassManager.CurClass == RoleType.Tutorial)
				return new VisionInformation(source, target, false, true, 0, 0, false, false, false);
			else return orig_GetVisionInformation(source, target, targetRadius, visionTriggerDistance, checkFog, checkLineOfSight, targetLightCheck);
		}
	}
}
