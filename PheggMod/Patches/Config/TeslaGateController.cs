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

				foreach (var gate in TeslaGates)
				{
					if (gate.isActiveAndEnabled)
						continue;
					if (gate.InactiveTime > 0)
					{
						gate.InactiveTime = Mathf.Max(0f, gate.InactiveTime - Time.fixedDeltaTime);
						continue;
					}

					bool playersInIdleRange = false;
					bool playersInActivationRange = false;
					foreach (var plr in allHubs)
					{
						if (plr.Value.isDedicatedServer || ignoreRoles.Contains(plr.Value.characterClassManager.CurClass))
							continue;

						if (!playersInIdleRange)
							playersInIdleRange = gate.PlayerInIdleRange(plr.Value);

						if (!playersInActivationRange && gate.PlayerInRange(plr.Value) && !gate.InProgress)
							playersInActivationRange = true;
					}

					if (playersInActivationRange)
						gate.ServerSideCode();

					if (playersInIdleRange != gate.isIdling)
						gate.ServerSideIdle(playersInIdleRange);
				}
			}
			else
			{
				foreach (TeslaGate teslaGate2 in TeslaGates)
				{
					teslaGate2.ClientSideCode();
				}
			}
		}
	}
}
