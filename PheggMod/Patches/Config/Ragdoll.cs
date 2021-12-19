#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using CustomPlayerEffects;
using Mirror;
using MonoMod;
using PlayerStatsSystem;
using System.Collections.Generic;
using UnityEngine;

namespace PheggMod.Patches
{
	[MonoModPatch("global::Ragdoll")]
	class PMRagdoll : Ragdoll
	{
		public static extern void orig_ServerSpawnRagdoll(ReferenceHub hub, DamageHandlerBase handler);
		public static void ServerSpawnRagdoll(ReferenceHub hub, DamageHandlerBase handler)
		{
			if (!NetworkServer.active || hub == null)
				return;

			if (!(handler is UniversalDamageHandler uDH) || uDH.TranslationId != DeathTranslations.Poisoned.Id)
				orig_ServerSpawnRagdoll(hub, handler);
		}

	}

	//[MonoModPatch("global::RagdollManager")]
	//class PMRagdollManager : RagdollManager
	//{
	//	public extern void orig_SpawnRagdoll(Vector3 pos, Quaternion rot, Vector3 velocity, int classId, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick, int playerId, bool scp096Death = false);
	//	public new void SpawnRagdoll(Vector3 pos, Quaternion rot, Vector3 velocity, int classId, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick, int playerId, bool scp096Death = false)
	//	{
	//		var ragdollDT = ragdollInfo.Tool;


	//		if (PMConfigFile.enable008 && (ragdollDT == DamageTypes.Scp0492 || ragdollDT == DamageTypes.Poison))
	//			return;

	//		else if (ragdollDT == DamageTypes.Pocket)
	//		{
	//			var gos = new List<GameObject>(GameObject.FindGameObjectsWithTag("RoomID"));
	//			gos.AddRange(GameObject.FindGameObjectsWithTag("PD_EXIT"));

	//			var list = new List<Vector3>();

	//			foreach (GameObject go in gos)
	//				list.Add(go.transform.position);

	//			foreach (Scp106PlayerScript scp106PlayerScript in FindObjectsOfType<Scp106PlayerScript>())
	//			{
	//				if (scp106PlayerScript.portalPosition != Vector3.zero)
	//				{
	//					list.Add(scp106PlayerScript.portalPosition);
	//				}
	//			}

	//			pos = GetPos(list);
	//			pos.y += 1;

	//			allowRecall = false;
	//		}

	//		orig_SpawnRagdoll(pos, rot, velocity, classId, ragdollInfo, allowRecall, ownerID, ownerNick, playerId);
	//	}

	//	private static Vector3 GetPos(List<Vector3> list)
	//	{
	//		Vector3 pos = list[Random.Range(0, list.Count)];

	//		if (pos == new Vector3(-1, 0, 0))
	//			pos = GetPos(list);

	//		return pos;
	//	}
	//}
}
