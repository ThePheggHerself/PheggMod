#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using Respawning;
using System;
using System.Diagnostics;
using UnityEngine;

namespace PheggMod.Patches
{
	[MonoModPatch("global::Respawning.RespawnTickets")]
	public class PMRespawnTickets : RespawnTickets
	{
		//public new const SpawnableTeamType DefaultTeam = SpawnableTeamType.NineTailedFox;
		public new const int DefaultTeamAmount = 30;
	}
}
