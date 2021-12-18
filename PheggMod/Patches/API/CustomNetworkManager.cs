#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Mirror;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Linq;

using Dissonance.Integrations.MirrorIgnorance;
using GameCore;
using System.Collections.Generic;

namespace PheggMod.Patches.API
{
	[MonoModPatch("global::CustomNetworkManager")]
	class PMCustomNetworkManager : CustomNetworkManager
	{
		//public override void OnServerDisconnect(NetworkConnection conn)
		//{
		//	try
		//	{
		//		if (ConfigFile.ServerConfig.GetBool("disconnect_drop", true))
		//			conn.identity.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(-1, "DISCONNECT", DamageTypes.Wall, 0, false), conn.identity.gameObject);
		//		base.OnServerDisconnect(conn);
		//		MirrorIgnoranceServer.ForceDisconnectClient(conn);

		//		conn.Disconnect();
		//	}
		//	catch (Exception e)
		//	{
		//		Base.Error(e.ToString() + "\n" + e.InnerException.ToString());
		//	}
		//}


		private extern IEnumerator<float> orig__CreateLobby();
		private IEnumerator<float> _CreateLobby()
		{
			var result = orig__CreateLobby();

			new Commands.CustomInternalCommands();
			PluginManager.PluginPreLoad();

			return result;
		}

	}
}
