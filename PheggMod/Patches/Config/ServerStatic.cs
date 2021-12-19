using System;
using MonoMod;

using PheggMod.API.Events;

namespace PheggMod.Patches
{
	[MonoModPatch("global::ServerStatic")]
	public class PMServerStatic : ServerStatic
	{
		public static ushort ServerPort;
		public static NextRoundAction StopNextRound = NextRoundAction.DoNothing;
		public static YamlConfig RolesConfig, SharedGroupsConfig, SharedGroupsMembersConfig;

		internal static extern void orig_ChangeLevel(bool noShutdownMessage);
		internal static void ChangeLevel(bool noShutdownMessage) => orig_ChangeLevel(noShutdownMessage);
	}
}
