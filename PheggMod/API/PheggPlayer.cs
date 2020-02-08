using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using RemoteAdmin;

namespace PheggMod
{
    public class PheggPlayer
    {
        public string name { get; internal set; }
        public string userId { get; internal set; }
        public string domain { get; internal set; }
        public string ipAddress { get; internal set; }
        public int playerId { get; internal set; }

        private CharacterClassManager _CharacterClassManager { get; set; }
        private ServerRoles _serverRoles { get; set; }
        private NicknameSync _nicknameSync { get; set; }
        private QueryProcessor _queryProcessor { get; set; }
        private Handcuffs _handcuffs { get; set; }
        private PlayerStats _playerStats { get; set; }
        private AmmoBox _ammoBox { get; set; }
        private Inventory _inventory { get; set; }
        private PlyMovementSync _plyMovementSync { get; set; }
        private BanPlayer _banPlayer { get; set; }


        public GameObject gameObject { get; internal set; }

        public class TeamClass
        {
            public RoleType role { get; internal set; }
            public Team team { get; internal set; }
        }

        public PheggPlayer(GameObject player)
        {
            if (player != null)
            {
                _CharacterClassManager = player.GetComponent<CharacterClassManager>();
                _serverRoles = player.GetComponent<ServerRoles>();
                _nicknameSync = player.GetComponent<NicknameSync>();
                _queryProcessor = player.GetComponent<RemoteAdmin.QueryProcessor>();
                _handcuffs = player.GetComponent<Handcuffs>();
                _playerStats = player.GetComponent<PlayerStats>();
                _ammoBox = player.GetComponent<AmmoBox>();
                _inventory = player.GetComponent<Inventory>();
                _plyMovementSync = player.GetComponent<PlyMovementSync>();
                _banPlayer = player.GetComponent<BanPlayer>();

                name = _nicknameSync.MyNick;
                userId = _CharacterClassManager.UserId;
                domain = _CharacterClassManager.UserId.Split('@')[1].ToUpper();
                ipAddress = _nicknameSync.connectionToClient.address;
                playerId = _queryProcessor.PlayerId;

                gameObject = player;
            }
            else throw new Exception("Cannot create PheggPlayer from null game object");
        }

        public override string ToString()
        {
            return $"{name} ({userId})";
        }

        public bool GodMode()
        {
            return _CharacterClassManager.GodMode;
        }
        public void GodMode(bool godmode)
        {
            _CharacterClassManager.GodMode = (bool)godmode;
        }

        public bool Bypass()
        {
            return gameObject.GetComponent<ServerRoles>().BypassMode;
        }
        public void Bypass(bool bypass)
        {
            gameObject.GetComponent<ServerRoles>().BypassMode = bypass;
        }

        public int Disarmed()
        {
            return _handcuffs.CufferId;
        }
        public void Disarmed(int playerid)
        {
            _handcuffs.CufferId = this.playerId;
        }

        public float Health()
        {
            return _playerStats.health;
        }
        public void Health(float hp)
        {
            _playerStats.health = hp;
        }

        public TeamClass Teamclass()
        {
            return new TeamClass { role = _CharacterClassManager.CurClass, team = _CharacterClassManager.Classes.SafeGet(_CharacterClassManager.CurClass).team };
        }
        public void Teamclass(RoleType role)
        {
            _CharacterClassManager.SetClassID(role);
        }

        public void SetAmmo(int type, int ammount)
        {
            _ammoBox.SetOneAmount(type, ammount.ToString());
        }

        public void Kill()
        {
            _playerStats.HurtPlayer(new PlayerStats.HitInfo(5555, "WORLD", DamageTypes.Nuke, playerId), gameObject);
        }

        public void Ban(int duration, string reason = "No reason provided", string issuer = "SERVER", bool banIP = true)
        {
            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = name,
                Id = userId,
                Issuer = issuer,
                IssuanceTime = DateTime.UtcNow.Ticks,
                Expires = DateTimeOffset.UtcNow.AddMinutes(duration).Ticks,
                Reason = reason
            }, BanHandler.BanType.UserId);

            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = name,
                Id = ipAddress,
                Issuer = issuer,
                IssuanceTime = DateTime.UtcNow.Ticks,
                Expires = DateTimeOffset.UtcNow.AddMinutes(duration).Ticks,
                Reason = reason
            }, BanHandler.BanType.IP);
        }

        public void Kick(string reason = "No reason provided", string issuer = "SERVER")
        {
            _banPlayer.KickUser(gameObject, reason, issuer, false);
        }

        public void GiveItem(ItemType type)
        {
            _inventory.AddNewItem(type);
        }
        public void GiveItems(ItemType[] items)
        {
            foreach (ItemType item in items)
                _inventory.AddNewItem(item);
        }
        public void ClearItems()
        {
            _inventory.Clear();
        }

        public void Teleport(Vector3 vector3, float rotation = 0, bool forcegound = true )
        {
            _plyMovementSync.OverridePosition(vector3, rotation, forcegound);
        }
    }
}
