//#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
//using CommandSystem;
//using CustomPlayerEffects;
//using GameCore;
//using Hints;
//using LightContainmentZoneDecontamination;
//using MapGeneration;
//using Mirror;
//using MonoMod;
//using PheggMod.API.Commands;
//using PheggMod.API.Events;
//using PheggMod.Commands;
//using RemoteAdmin;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace PheggMod.Patches
//{
//	//[MonoModPatch("global::PocketDimensionTeleport")]
//	public class PMPocketDimensionTeleport : PocketDimensionTeleport
//	{
//		private readonly List<Vector3> tpPositions;
//		private PDTeleportType type;

//		private extern void orig_OnTriggerEnter(Collider other);
//		private void OnTriggerEnter(Collider other)
//		{
//			if (!NetworkServer.active)
//				return;

//			var networkIdentity = other.GetComponent<NetworkIdentity>();

//			if (networkIdentity == null)
//				return;

//			ReferenceHub hub = ReferenceHub.GetHub(other.gameObject);

//			System.Random rand = new System.Random();
//			var PlayerStats = hub.playerStats;
//			var PMS = hub.playerMovementSync;



//			if (BlastDoor.OneDoor.isClosed)
//			{
//				PlayerStats.HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0, false), other.gameObject, true, true);
//				return;
//			}

//			if (type == PDTeleportType.Killer || rand.Next(0, 100) < 40)
//			{
//				float dmg = Mathf.Clamp(((PlayerStats.Health + (int)PlayerStats.ArtificialHealth) / 5), 5, 50);
//				PlayerStats.HurtPlayer(new PlayerStats.HitInfo(dmg, "POCKET", DamageTypes.Pocket, 0, false), other.gameObject, true, true);
//				PMS.OverridePosition(Vector3.down * 1998.5f, rand.Next(0, 356));

//				hub.hints.Show(new TextHint("You chose the wrong door...", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
//			}
//			else
//			{
//				tpPositions.Clear();
//				DecontaminationController.DecontaminationPhase[] deconPhases = DecontaminationController.Singleton.DecontaminationPhases;
//				List<string> roomList = ConfigFile.ServerConfig.GetStringList(DecontaminationController.GetServerTime > deconPhases[deconPhases.Length - 2].TimeTrigger ? "pd_random_exit_rids_after_decontamination" : "pd_random_exit_rids");

//				if (roomList.Count > 0)
//				{
//					foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("RoomID"))
//					{
//						Rid component2 = gameObject.GetComponent<Rid>();
//						if (component2 != null && roomList.Contains(component2.id, StringComparison.Ordinal))
//						{
//							tpPositions.Add(gameObject.transform.position);
//						}
//					}
//					if (roomList.Contains("PORTAL"))
//					{
//						foreach (Scp106PlayerScript scp106PlayerScript in UnityEngine.Object.FindObjectsOfType<Scp106PlayerScript>())
//						{
//							if (scp106PlayerScript.portalPosition != Vector3.zero)
//							{
//								this.tpPositions.Add(scp106PlayerScript.portalPosition);
//							}
//						}
//					}
//				}
//				if (tpPositions == null || tpPositions.Count == 0)
//				{
//					foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("PD_EXIT"))
//					{
//						tpPositions.Add(gameObject2.transform.position);
//					}
//				}

//				Vector3 pos = tpPositions[UnityEngine.Random.Range(0, tpPositions.Count)];
//				pos.y += 2f;
//				PlayerMovementSync component3 = other.GetComponent<PlayerMovementSync>();
//				component3.AddSafeTime(2f, false);
//				component3.OverridePosition(pos, 0f, false);
//				this.RemoveCorrosionEffect(other.gameObject);
//				PlayerManager.localPlayer.GetComponent<PlayerStats>().TargetAchieve(networkIdentity.connectionToClient, "larryisyourfriend");
//			}

//			if (PocketDimensionTeleport.RefreshExit)
//			{
//				ImageGenerator.pocketDimensionGenerator.GenerateRandom();
//			}
//		}

//		private extern void orig_RemoveCorrosionEffect(GameObject escapee);
//		private void RemoveCorrosionEffect(GameObject escapee)
//		{
//			if (!NetworkServer.active)
//			{
//				Debug.LogWarning("[Server] function 'System.Void PocketDimensionTeleport::RemoveCorrosionEffect(UnityEngine.GameObject)' called on client");
//				return;
//			}
//			escapee.GetComponentInParent<PlayerEffectsController>().GetEffect<Corroding>().Intensity = 0;
//		}
//	}
//}
