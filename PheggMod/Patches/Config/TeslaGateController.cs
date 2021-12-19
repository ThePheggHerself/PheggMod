#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using Hints;
using MEC;
using Mirror;
using MonoMod;
using PheggMod.API;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PheggMod.Patches
{
	[MonoModPatch("global::TeslaGateController")]
	public class PMTeslaGateController : TeslaGateController
	{
		private List<TeslaGate> TeslaGates = new List<TeslaGate>();
		private static RoleType[] ignoreRoles = new RoleType[] { RoleType.Tutorial };
    
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				Dictionary<GameObject, ReferenceHub> allHubs = ReferenceHub.GetAllHubs();
				using (List<TeslaGate>.Enumerator enumerator = TeslaGates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TeslaGate teslaGate = enumerator.Current;
						if (teslaGate.isActiveAndEnabled)
						{
							bool playersInIdleRange = false;
							bool playersInActivationRange = false;
							foreach (var hub in allHubs)
							{
								if (!hub.Value.isDedicatedServer && !ignoreRoles.Contains(hub.Value.characterClassManager.CurClass))
								{
									if (!playersInIdleRange)
									{
										playersInIdleRange = teslaGate.PlayerInIdleRange(hub.Value);
									}
									if (hub.Value.characterClassManager.CurClass != RoleType.Spectator && !playersInActivationRange && teslaGate.PlayerInRange(hub.Value) && !teslaGate.InProgress)
									{
										playersInActivationRange = true;
									}
								}
							}
							if (playersInActivationRange)
							{
								teslaGate.ServerSideCode();
							}
							if (playersInIdleRange != teslaGate.isIdling)
							{
								teslaGate.ServerSideIdle(playersInIdleRange);
							}
						}
					}
					return;
				}
			}
			foreach (TeslaGate teslaGate2 in TeslaGates)
			{
				teslaGate2.ClientSideCode();
			}



			//try
			//{
			//	if (NetworkServer.active)
			//	{
			//		Dictionary<GameObject, ReferenceHub> hubs = ReferenceHub.GetAllHubs();

			//		foreach (TeslaGate tesla in TeslaGates)
			//		{
			//			Dictionary<ReferenceHub, bool> PlayersInRange = new Dictionary<ReferenceHub, bool>();

			//			foreach (var player in ReferenceHub.GetAllHubs())
			//			{
			//				if (ReferenceHub.HostHub == null ||  !player.Value.isDedicatedServer || !ReferenceHub.HostHub.characterClassManager.RoundStarted || ignoreRoles.Contains(player.Value.characterClassManager.CurClass))
			//					continue;

			//				//Base.Info(ignoreRoles.Where(r => r == player.Value.characterClassManager.CurClass).Any() + " " + ignoreRoles.Contains(player.Value.characterClassManager.CurClass));

			//				//Base.Info(player.Value.characterClassManager.CurClass.ToString() + " " + ignoreRoles.Contains(player.Value.characterClassManager.CurClass));


			//				if (tesla.PlayerInRange(player.Value))
			//					PlayersInRange.Add(player.Value, true);						
			//				else if (tesla.PlayerInIdleRange(player.Value))
			//					PlayersInRange.Add(player.Value, false);


			//				if (PlayersInRange.Where(r => r.Value == true).Any() && !tesla.InProgress)
			//					tesla.ServerSideCode();

			//				if (PlayersInRange.Where(r => r.Value == false).Any() != tesla.isIdling)
			//					tesla.ServerSideIdle(PlayersInRange.Where(r => r.Value == false).Any());

			//			}
			//		}
			//	}
			//	foreach (TeslaGate teslaGate2 in this.TeslaGates)
			//	{
			//		teslaGate2.ClientSideCode();
			//	}
			//}
			//catch(Exception e)
			//{
			//	Base.Error(e.ToString());
			//}
		}
	}
}
