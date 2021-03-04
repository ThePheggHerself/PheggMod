#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using MonoMod;

using PheggMod.API.Events;

namespace PheggMod.EventTriggers
{
	[MonoModPatch("global::ServerStatic")]
	public class PMServerStatic : ServerStatic
	{
		public static NextRoundAction StopNextRound = NextRoundAction.DoNothing;
	}
}
