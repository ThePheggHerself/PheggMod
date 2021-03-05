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
