using System;
using MonoMod;

using PheggMod.API.Events;

namespace PheggMod.Patches
{
	[MonoModPatch("global::ServerStatic")]
	public class PMServerStatic : ServerStatic
	{
		public static NextRoundAction StopNextRound = NextRoundAction.DoNothing;
		public static YamlConfig RolesConfig, SharedGroupsConfig, SharedGroupsMembersConfig;
	}
}
