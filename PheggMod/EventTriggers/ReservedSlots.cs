#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
using CustomPlayerEffects;
using Mirror;
using MonoMod;
using System.Collections.Generic;

namespace PheggMod.EventTriggers
{
	[MonoModPatch("global::ReservedSlot")]
	public class PMReservedSlots
	{
		public static HashSet<string> Users = new HashSet<string>();

		public static void AddReservedSlot(string userId)
		{
			if (!Users.Contains(userId))
				Users.Add(userId);
		}
	}
}
