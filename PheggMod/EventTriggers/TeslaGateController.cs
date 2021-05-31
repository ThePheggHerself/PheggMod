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

namespace PheggMod.EventTriggers
{
	[MonoModPatch("global::TeslaGateController")]
	public class PMTeslaGateController : TeslaGateController
	{
		private List<TeslaGate> TeslaGates = new List<TeslaGate>();

		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				using (Dictionary<GameObject, ReferenceHub>.Enumerator enumerator = ReferenceHub.GetAllHubs().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<GameObject, ReferenceHub> keyValuePair = enumerator.Current;
						ReferenceHub hub = keyValuePair.Value;

						if (hub.characterClassManager.CurClass != RoleType.Spectator && hub.characterClassManager.CurClass != RoleType.Tutorial && !hub.isDedicatedServer && hub.Ready)
						{
							foreach (TeslaGate teslaGate in this.TeslaGates)
							{
								if (teslaGate.PlayerInRange(hub) && !teslaGate.InProgress)
								{
									teslaGate.ServerSideCode();
								}
							}
						}
					}
					return;
				}
			}
			foreach (TeslaGate teslaGate2 in this.TeslaGates)
			{
				teslaGate2.ClientSideCode();
			}
		}
	}
}
