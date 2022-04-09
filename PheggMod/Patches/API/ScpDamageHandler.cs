#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
using CustomPlayerEffects;
using Mirror;
using MonoMod;
using System.Collections.Generic;

namespace PheggMod.Patches
{
	[MonoModPatch("global::PlayerStatsSystem.ScpDamageHandler")]
	internal class PMScpDamageHandler : PlayerStatsSystem.ScpDamageHandler
	{
		public readonly byte _translationId;
	}
}
