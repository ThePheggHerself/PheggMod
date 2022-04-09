//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using CustomPlayerEffects;
//using Hints;
//using Mirror;
//using PheggMod.CustomEffects;
//using System.Diagnostics.Eventing.Reader;
//using PheggMod.Patches;
//using InventorySystem;
//using InventorySystem.Items;
//using InventorySystem.Disarming;
//using PlayerStatsSystem;

//namespace PheggMod.Patches
//{
//	public class FFDetector
//	{
//		public static Dictionary<string, DateTime> DamageList = new Dictionary<string, DateTime>();
//		public static Dictionary<string, FFPlayer> FFPlayers = new Dictionary<string, FFPlayer>();
//		public static Dictionary<string, FFDGrenadeThrower> FFDGrenadeThrowers = new Dictionary<string, FFDGrenadeThrower>();

//		public static bool DoCheck = false;
//		public static bool DetectorEnabled = true;

//		internal static void CalculateFF(GameObject Victim, AttackerDamageHandler aDH, out float damage)
//		{
//			try
//			{
//				damage = aDH.Damage;

//				//if (!DetectorEnabled || !DoCheck || Victim.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator || (!info.Tool ! && info.GetDamageType() != DamageTypes.Grenade))
//				//	return;

//				FFInfo ffInfo = new FFInfo
//				{
//					//DamageType = info.Tool
//					AttackerDamageHandler = aDH,
//					Target = ReferenceHub.GetHub(Victim),
//					Attacker = aDH.Attacker.Hub,
//				};

//				if (ffInfo.Attacker.playerId == ffInfo.Target.playerId)
//					return;

//				if (ffInfo.Target.characterClassManager.CurClass == RoleType.ClassD && ffInfo.Attacker.characterClassManager.CurClass == RoleType.ClassD)
//				{
//					UpdateLegitDamage(ffInfo.Attacker.characterClassManager.UserId);
//					return;
//				}

//				var isGrenade = aDH is ExplosionDamageHandler;

//				if (isGrenade)
//				{
//					if (FFDGrenadeThrowers.ContainsKey(ffInfo.Attacker.characterClassManager.UserId))
//					{
//						ffInfo.FFDGrenadeThrower = FFDGrenadeThrowers[ffInfo.Attacker.characterClassManager.UserId];

//						//Base.Info("FFInfoFFDGrenadeThrower Set.");
//					}
//					else
//					{
//						ffInfo.FFDGrenadeThrower = new FFDGrenadeThrower
//						{
//							Role = ffInfo.Attacker.characterClassManager.CurClass,
//							Team = ffInfo.Attacker.characterClassManager.CurRole.team,
//							UserId = ffInfo.Attacker.characterClassManager.UserId,
//							DetonatePosition = ffInfo.Target.playerMovementSync.RealModelPosition
//						};

//						//Base.Info("FFInfoFFDGrenadeThrower Default.");
//					}

//					ffInfo.NearbyPlayers = GetNearbyPlayersGrenade(FFDGrenadeThrowers[ffInfo.Attacker.characterClassManager.UserId].DetonatePosition);
//				}
//				else
//					ffInfo.NearbyPlayers = GetNearbyPlayers(ffInfo);

//				if (ffInfo.Attacker.characterClassManager.CurRole.team == Team.RIP && isGrenade)
//					ffInfo.AttackerTeam = ffInfo.FFDGrenadeThrower.Team;

//				foreach (ReferenceHub hub in ffInfo.NearbyPlayers)
//				{
//					if (IsFF(ffInfo, hub))
//						ffInfo.Friendlies.Add(hub);
//					else
//						ffInfo.Hostiles.Add(hub);
//				}

//				bool isFF = IsFF(ffInfo, ffInfo.Target);

//				if (DamageList.ContainsKey(ffInfo.Attacker.characterClassManager.UserId))
//					ffInfo.LastLegitDamage = DamageList[ffInfo.Attacker.characterClassManager.UserId];
//				else
//					ffInfo.LastLegitDamage = new DateTime();

//				if (isFF)
//				{
//					if ((DateTime.Now - ffInfo.LastLegitDamage).TotalSeconds > 10 && (ffInfo.Hostiles.Count < 1 && ffInfo.Friendlies.Count > 0))
//					{
//						damage = 0;
//						HandleList(ffInfo);
//					}
//				}
//				else
//				{
//					if (DamageList.ContainsKey(ffInfo.Attacker.characterClassManager.UserId))
//						ffInfo.LastLegitDamage = DateTime.Now;
//					else
//						DamageList.Add(ffInfo.Attacker.characterClassManager.UserId, DateTime.Now);
//				}
//			}
//			catch (Exception e)
//			{
//				Base.Error(e.ToString());
//				damage = aDH.Damage;
//			}

//			#region E
//			//damage = info.Amount;

//			//if (!DoCheck || Victim.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
//			//	return;

//			//DamageTypes.DamageType damageType = info.GetDamageType();
//			//if (!damageType.isWeapon && damageType != DamageTypes.Grenade)
//			//{
//			//	updateLegitDamage(info.GetPlayerObject().GetComponent<CharacterClassManager>().UserId);
//			//	return;
//			//}

//			//ReferenceHub vicHub = ReferenceHub.GetHub(Victim);
//			//ReferenceHub attHub = ReferenceHub.GetHub(info.GetPlayerObject());

//			//if (vicHub.characterClassManager.CurClass == RoleType.ClassD && attHub.characterClassManager.CurClass == RoleType.ClassD)
//			//{
//			//	updateLegitDamage(info.GetPlayerObject().GetComponent<CharacterClassManager>().UserId);
//			//	return;
//			//}

//			//if (vicHub.playerId == attHub.playerId)
//			//{
//			//	updateLegitDamage(info.GetPlayerObject().GetComponent<CharacterClassManager>().UserId);
//			//	return;
//			//}

//			//List<ReferenceHub> Hubs = new List<ReferenceHub>();
//			//List<ReferenceHub> Hostiles = new List<ReferenceHub>();
//			//List<ReferenceHub> Friendlies = new List<ReferenceHub>();

//			//if (info.GetDamageType() == DamageTypes.Grenade)
//			//{
//			//	if (FFDGrenadeThrowers.ContainsKey(attHub.characterClassManager.UserId))
//			//		Hubs = GetNearbyPlayersGrenade(FFDGrenadeThrowers[attHub.characterClassManager.UserId].DetonatePosition);
//			//}
//			//else
//			//	Hubs = GetNearbyPlayers(attHub, Victim);

//			//Team attackerTeam = Team.RIP;

//			//if (attHub.characterClassManager.CurClass == RoleType.Spectator && info.GetDamageType() == DamageTypes.Grenade)
//			//{
//			//	if (FFDGrenadeThrowers.ContainsKey(attHub.characterClassManager.UserId))
//			//	{
//			//		attackerTeam = FFDGrenadeThrowers[attHub.characterClassManager.UserId].Team;
//			//	}
//			//}
//			//else
//			//	attackerTeam = attHub.characterClassManager.CurRole.team;

//			//foreach (ReferenceHub hub in Hubs)
//			//{
//			//	if (!IsFF(attHub, hub, attackerTeam))
//			//		Hostiles.Add(hub);
//			//	else
//			//		Friendlies.Add(hub);
//			//}


//			//if (!DamageList.ContainsKey(attHub.characterClassManager.UserId) || (DateTime.Now - DamageList[attHub.characterClassManager.UserId]).TotalSeconds > 9)
//			//{
//			//	if (Friendlies.Count > 0 && Hostiles.Count < 1)
//			//	{
//			//		damage = 0;

//			//		HandleList(attHub, info, Victim, Friendlies, Hostiles);
//			//	}
//			//}
//			//else
//			//{
//			//AddLegitDamage:
//			//	updateLegitDamage(info.GetPlayerObject().GetComponent<CharacterClassManager>().UserId);
//			//}
//			#endregion
//		}

//		private static void HandleList(FFInfo ffInfo)
//		{
//			if (FFPlayers.ContainsKey(ffInfo.Attacker.characterClassManager.UserId))
//			{
//				FFPlayer fFPlayer = FFPlayers[ffInfo.Attacker.characterClassManager.UserId];

//				if ((DateTime.Now - fFPlayer.LastTrigger).TotalSeconds < 120)
//				{
//					fFPlayer.Triggers++;
//				}
//				else
//				{
//					FFPlayers.Remove(ffInfo.Attacker.characterClassManager.UserId);
//					FFPlayers.Add(ffInfo.Attacker.characterClassManager.UserId, new FFPlayer { UserID = ffInfo.Attacker.characterClassManager.UserId, Triggers = 1, LastTrigger = DateTime.Now });
//				}
//			}

//			else FFPlayers.Add(ffInfo.Attacker.characterClassManager.UserId, new FFPlayer { UserID = ffInfo.Attacker.characterClassManager.UserId, Triggers = 1, LastTrigger = DateTime.Now });

//			ffInfo.FFPlayer = FFPlayers[ffInfo.Attacker.characterClassManager.UserId];

//			AddPunishment(ffInfo);
//		}
//		private static void AddPunishment(FFInfo ffInfo)
//		{
//			ffInfo.Attacker.hints.Show(new TextHint("I Shouldn't Shoot Friendlies...", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));

//			float damage = Mathf.Clamp(ffInfo.AttackerDamageHandler.Damage / 4, 5, 50);
//			damage = Mathf.Clamp(damage, 0, ffInfo.AttackerDamageHandler.Attacker.Hub.characterClassManager. - 1);

//			ffInfo.AttackerDamageHandler.Attacker.Hub.playerStats.DealDamage(new PlayerStats.HitInfo(damage, ffInfo.HitInfo.Attacker, ffInfo.HitInfo.Tool, ffInfo.Attacker.playerId, false), ffInfo.Attacker.gameObject);
//			//ffInfo.Attacker.inventory.

//			if (ffInfo.FFPlayer.Triggers > 2)
//				//ffInfo.Attacker.inventory.ServerDropAll();

//				if (ffInfo.FFPlayer.Triggers == 4)
//					ffInfo.Attacker.playerEffectsController.EnableEffect<Amnesia>(10);
//				else if (ffInfo.FFPlayer.Triggers == 5)
//					ffInfo.Attacker.playerEffectsController.EnableEffect<Concussed>(45);
//				else if (ffInfo.FFPlayer.Triggers > 5)
//				{
//					ffInfo.Attacker.playerEffectsController.EnableEffect<Blinded>(20 * (ffInfo.FFPlayer.Triggers - 5));
//					ffInfo.Attacker.playerEffectsController.EnableEffect<Deafened>(20 * (ffInfo.FFPlayer.Triggers - 5));
//					ffInfo.Attacker.playerEffectsController.EnableEffect<Ensnared>(5 * (ffInfo.FFPlayer.Triggers - 5));
//				}

//			Base.Debug($"{ffInfo.Attacker.nicknameSync.DisplayName} ({ffInfo.Attacker.characterClassManager.UserId}) was punished by FFDetector for Friendly Fire against {ffInfo.Target.nicknameSync.DisplayName} ({ffInfo.Target.characterClassManager.UserId})" +
//				$"\nPlayer Information: {ffInfo.Attacker.characterClassManager.CurClass} ({ffInfo.Attacker.characterClassManager.CurRole.team})" +
//				$"\nTarget Information: {ffInfo.Target.characterClassManager.CurClass} ({ffInfo.Target.characterClassManager.CurRole.team}) {(ffInfo.Target.inventory.IsDisarmed() ? "Disarmed" : "Not disarmed")} {(ffInfo.Target.playerEffectsController.GetEffect<SCP008>().IsEnabled ? "Infected" : "Not infected")}" +
//				$"\nDistance: {Vector3.Distance(ffInfo.Attacker.playerMovementSync.RealModelPosition, ffInfo.Target.playerMovementSync.RealModelPosition)}m Angle: {Vector3.Angle(ffInfo.Attacker.playerMovementSync.transform.forward, ffInfo.Attacker.playerMovementSync.transform.position - ffInfo.Target.GetComponent<PlayerMovementSync>().transform.position)} DamageType: {ffInfo.HitInfo.Tool.ToString()}" +
//				$"\nHostiles: {ffInfo.Hostiles.Count} Friendlies: {ffInfo.Friendlies.Count} Total: {ffInfo.NearbyPlayers.Count}");
//		}
//		private static List<ReferenceHub> GetNearbyPlayers(FFInfo ffInfo)
//		{
//			List<ReferenceHub> nearbyPlayers = new List<ReferenceHub>();

//			PlayerMovementSync pms = ffInfo.Attacker.playerMovementSync;
//			float distanceCheck = ffInfo.Attacker.playerMovementSync.RealModelPosition.y > 900 ? 90 : 40;

//			foreach (var hub in ReferenceHub.GetAllHubs().Values)
//			{
//				float angle = Vector3.Angle(pms.transform.forward, pms.transform.position - hub.playerMovementSync.transform.position);
//				float distance = Vector3.Distance(pms.RealModelPosition, hub.playerMovementSync.RealModelPosition);

//				if (distance <= distanceCheck && angle > 130 || distance < 5)
//					nearbyPlayers.Add(hub);
//			}

//			return nearbyPlayers;
//		}
//		private static List<ReferenceHub> GetNearbyPlayersGrenade(Vector3 position)
//		{
//			List<ReferenceHub> nearbyPlayers = new List<ReferenceHub>();

//			foreach (var hub in ReferenceHub.GetAllHubs().Values)
//			{
//				float distance = Vector3.Distance(position, hub.playerMovementSync.RealModelPosition);

//				if (distance < 8.5)
//					nearbyPlayers.Add(hub);
//			}

//			return nearbyPlayers;
//		}

//		private static bool IsFF(FFInfo ffInfo, ReferenceHub Target)
//		{
//			//if (Target.playerEffectsController.GetEffect<SCP008>().Enabled)
//			//	return false;

//			Team AttackerTeam = ffInfo.Attacker.characterClassManager.CurRole.team == Team.RIP ? ffInfo.AttackerTeam : ffInfo.Attacker.characterClassManager.CurRole.team;
//			Team TargetTeam = Target.characterClassManager.CurRole.team;

//			if ((AttackerTeam == Team.CDP || AttackerTeam == Team.CHI) && (TargetTeam == Team.CDP || TargetTeam == Team.CHI))
//				return true;
//			else if ((AttackerTeam == Team.RSC || AttackerTeam == Team.MTF) && (TargetTeam == Team.RSC || TargetTeam == Team.MTF))
//				return true;
//			else return false;
//		}

//		private static void UpdateLegitDamage(string Attacker)
//		{
//			if (DamageList.ContainsKey(Attacker))
//				DamageList[Attacker] = DateTime.Now;
//			else
//				DamageList.Add(Attacker, DateTime.Now);
//		}
//	}
//}
