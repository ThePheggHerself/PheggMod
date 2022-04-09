using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using Mirror;
using MonoMod;
using System.Collections.Generic;

namespace PheggMod.Patches
{
	[MonoModPatch("global::RoundRestarting.RoundRestart")]
	internal class PMRoundRestart
	{
		internal static extern void orig_ChangeLevel(bool noShutdownMessage);
		internal static void ChangeLevel(bool noShutdownMessage) => orig_ChangeLevel(noShutdownMessage);
	}
}
