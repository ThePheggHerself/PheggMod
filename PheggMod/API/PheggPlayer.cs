﻿using Mirror;
using PheggMod.API;
using RemoteAdmin;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Hints;
using InventorySystem;
using InventorySystem.Disarming;
using PlayerStatsSystem;

namespace PheggMod
{
	public class PheggPlayer
	{
		private readonly static Regex _filterNames = new Regex("[(\\*)|(_)|({)|(})|(@)|(<)|(>)|(\")]");

		public ReferenceHub refHub { get; internal set; }
		public bool isEmpty = true;

		public PheggPlayer(GameObject player)
		{
			if (player == null)
				throw new ArgumentNullException("Can't make PheggPlayer from null");
			else if (player.GetComponent<CharacterClassManager>().isLocalPlayer)
				throw new ArgumentNullException("Can't make PheggPlayer from server");
			else
			{
				isEmpty = false;
				refHub = player.GetComponent<ReferenceHub>();

				_banPlayer = player.GetComponent<BanPlayer>();
				_networkConnection = player.GetComponent<NetworkConnection>();
				_broadcast = player.GetComponent<Broadcast>();
			}
		}

		public PheggPlayer(ReferenceHub player)
		{
			if (player == null)
				throw new ArgumentNullException("Can't make PheggPlayer from null");
			else if (player.GetComponent<CharacterClassManager>().isLocalPlayer)
				throw new ArgumentNullException("Can't make PheggPlayer from server");
			else
			{
				isEmpty = false;
				refHub = player;

				_banPlayer = player.gameObject.GetComponent<BanPlayer>();
				_networkConnection = player.gameObject.GetComponent<NetworkConnection>();
				_broadcast = player.gameObject.GetComponent<Broadcast>();
			}
		}

		//Basic user info
		public string name => refHub.nicknameSync.MyNick;
		public string nameClean => _filterNames.Replace(name, @"\$&");
		public string userId => refHub.characterClassManager.UserId;
		public string domain => refHub.characterClassManager.UserId.Split('@')[1].ToUpper();
		public string ipAddress => refHub.nicknameSync.connectionToClient.address;
		public int playerId => refHub.queryProcessor.PlayerId;
		public GameObject gameObject =>  refHub.gameObject;
		private BanPlayer _banPlayer { get; set; }
		private NetworkConnection _networkConnection { get; set; }
		private Broadcast _broadcast { get; set; }

		public bool Godmode
		{
			get => refHub.characterClassManager.GodMode;
			set => refHub.characterClassManager.GodMode = value;
		}
		public bool Bypass
		{
			get => refHub.serverRoles.BypassMode;
			set => refHub.serverRoles.BypassMode = value;
		}
		public bool Disarmed
		{
			get => refHub.inventory.IsDisarmed();
			set
			{
				if (value)
				{
					refHub.inventory.SetDisarmedStatus(null);
					DisarmedPlayers.Entries.Add(new DisarmedPlayers.DisarmedEntry(refHub.networkIdentity.netId, 0));
				}
			}
		}
		public float Health
		{
			get 
			{
				return refHub.playerStats.GetModule<HealthStat>().CurValue;
			}
			set
			{
				refHub.playerStats.GetModule<HealthStat>().CurValue = value;
			}
		}

		public float AHP
		{
			get => refHub.playerStats.GetModule<AhpStat>().CurValue;		
			set => refHub.playerStats.GetModule<AhpStat>().CurValue = value;			
		}
		public RoleType roleType
		{
			get => refHub.characterClassManager.CurClass;
			set
			{
				refHub.characterClassManager.SetClassID(value, CharacterClassManager.SpawnReason.ForceClass);
				Health = refHub.characterClassManager.Classes.Get(value).maxHP;
			}
		}
		public Team team
		{
			get => refHub.characterClassManager.CurRole.team;
		}
		public Vector3 position
		{
			get => refHub.playerMovementSync.RealModelPosition;
			set => refHub.playerMovementSync.OverridePosition(value, forceGround: true);
		}
		public Role role
		{
			get => refHub.characterClassManager.CurRole;
		}


		public override string ToString() => $"{nameClean} ({userId})";
		
		public void Kill() => refHub.playerStats.DealDamage(new UniversalDamageHandler(StandardDamageHandler.KillValue, DeathTranslations.Crushed));

		public void Ban(int duration, string reason = "No reason provided", string issuer = "SERVER", bool banIP = true)
		{
			_banPlayer.BanUser(gameObject, duration, reason, issuer);
		}
		public void Kick(string reason = "No reason provided", string issuer = "SERVER")
		{
			_banPlayer.KickUser(gameObject, reason, issuer, false);
		}

		public void GiveItems(ItemType[] items)
		{
			foreach (ItemType item in items)
				refHub.inventory.ServerAddItem(item);
		}
		public void GiveItem(ItemType type) => refHub.inventory.ServerAddItem(type);

		public void PersonalBroadcast(ushort duration, string message, Broadcast.BroadcastFlags flag = Broadcast.BroadcastFlags.Normal)
		{
			_broadcast.TargetAddElement(_networkConnection, message, duration, flag);
		}
		public void SendConsoleMessage(string message, string color = "green")
		{
			refHub.characterClassManager.TargetConsolePrint(_networkConnection, message, color);
		}
		public void SetTag(string Text, TagColour colour = TagColour.DEFAULT, ulong permissions = 3)
		{
			if (permissions == 3)
				permissions = refHub.serverRoles.Permissions;

			refHub.serverRoles.SetText(Text);
			if (colour != TagColour.DEFAULT)
				refHub.serverRoles.SetColor(Base.colours[(int)colour]);
			refHub.serverRoles.Permissions = permissions;
		}

		public void SendHintMessage(string message, float duration = 5) => refHub.hints.Show(new TextHint(message, new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), duration));
	}
}
