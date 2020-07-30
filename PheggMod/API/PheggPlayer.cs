using Mirror;
using PheggMod.API;
using RemoteAdmin;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Hints;

namespace PheggMod
{
    public class PheggPlayer
    {
        private readonly static Regex _filterNames = new Regex("[(\\*)|(_)|({)|(})|(@)|(<)|(>)|(\")]");

        public ReferenceHub refHub { get; internal set; }
        public bool isEmpty = true;

        //Basic user info
        public string name { get; internal set; }
        public string nameClean { get; internal set; }
        public string userId { get; internal set; }
        public string domain { get; internal set; }
        public string ipAddress { get; internal set; }
        public int playerId { get; internal set; }

        public GameObject gameObject { get; internal set; }

        private BanPlayer _banPlayer { get; set; }
        private NetworkConnection _networkConnection { get; set; }
        private Broadcast _broadcast { get; set; }

        [Obsolete]
        public class Components
        {
            public CharacterClassManager ccm;
            public ServerRoles sr;
            public NicknameSync ns;
            public QueryProcessor qp;
            public Handcuffs hc;
            public PlayerStats ps;
            public AmmoBox ab;
            public Inventory inv;
            public PlayerMovementSync pms;
            public BanPlayer bp;
            public NetworkConnection nc;
            public Broadcast bc;
        }

        public PheggPlayer(GameObject player)
        {
            if (player == null)
                throw new ArgumentNullException("Can't make PheggPlayer from null");
            else if (player.GetComponent<CharacterClassManager>().isLocalPlayer)
            {
                throw new ArgumentNullException("Can't make PheggPlayer from server");
            }
            else
            {
                isEmpty = false;

                refHub = player.GetComponent<ReferenceHub>();

                _banPlayer = player.GetComponent<BanPlayer>();
                _networkConnection = player.GetComponent<NetworkConnection>();
                _broadcast = player.GetComponent<Broadcast>();

                name = refHub.nicknameSync.MyNick;
                nameClean = _filterNames.Replace(name, string.Empty);
                userId = refHub.characterClassManager.UserId;
                domain = refHub.characterClassManager.UserId.Split('@')[1].ToUpper();
                ipAddress = refHub.nicknameSync.connectionToClient.address;
                playerId = refHub.queryProcessor.PlayerId;

                gameObject = player;

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete
                commonComponents = new Components
                {
                    ccm = _characterClassManager,
                    sr = _serverRoles,
                    ns = _nicknameSync,
                    qp = _queryProcessor,
                    hc = _handcuffs,
                    ps = _playerStats,
                    ab = _ammoBox,
                    inv = _inventory,
                    pms = _plyMovementSync,
                    bp = _banPlayer,
                    nc = _networkConnection,
                    bc = _broadcast
                };

                #region components
                _characterClassManager = refHub.characterClassManager;
                _serverRoles = refHub.serverRoles;
                _nicknameSync = refHub.nicknameSync;
                _queryProcessor = refHub.queryProcessor;
                _handcuffs = refHub.handcuffs;
                _playerStats = refHub.playerStats;
                _ammoBox = refHub.ammoBox;
                _inventory = refHub.inventory;
                _plyMovementSync = refHub.playerMovementSync;
                _banPlayer = player.GetComponent<BanPlayer>();
                _networkConnection = player.GetComponent<NetworkConnection>();
                _broadcast = player.GetComponent<Broadcast>();

                #endregion
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete

            }
        }

        public bool godmode
        {
            get => refHub.characterClassManager.GodMode;
            set => refHub.characterClassManager.GodMode = value;
        }
        public bool bypass
        {
            get => refHub.serverRoles.BypassMode;
            set => refHub.serverRoles.BypassMode = value;
        }
        public int disarmed
        {
            get => refHub.handcuffs.CufferId;
            set => refHub.handcuffs.CufferId = value;
        }
        public float health
        {
            get => refHub.playerStats.Health;
            set => refHub.playerStats.Health = value;
        }
        public RoleType roleType
        {
            get => refHub.characterClassManager.CurClass;
            set
            {
                refHub.characterClassManager.SetClassID(value);
                refHub.playerStats.Health = refHub.characterClassManager.Classes.Get(value).maxHP;
            }
        }
        public Team team
        {
            get => refHub.characterClassManager.CurRole.team;
        }
        public Vector3 position
        {
            get => refHub.playerMovementSync.RealModelPosition;
            set => refHub.playerMovementSync.OverridePosition(value, 0, true);
        }
        public Role role
        {
            get => refHub.characterClassManager.CurRole;
        }


        public override string ToString()
        {
            return $"{_filterNames.Replace(name, string.Empty)} ({userId})";
        }
        public void Kill() => refHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(10000, "WORLD", DamageTypes.Wall, playerId), gameObject);
        
        public void Ban(int duration, string reason = "No reason provided", string issuer = "SERVER", bool banIP = true)
        {
            if (duration < 1)
                Kick(reason, issuer);

            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = name,
                Id = userId,
                Issuer = issuer,
                IssuanceTime = DateTime.UtcNow.Ticks,
                Expires = DateTimeOffset.UtcNow.AddMinutes(duration).Ticks,
                Reason = reason
            }, BanHandler.BanType.UserId);
            if (banIP)
            {
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
        }
        public void Kick(string reason = "No reason provided", string issuer = "SERVER")
        {
            _banPlayer.KickUser(gameObject, reason, issuer, false);
        }
  
        public void GiveItems(ItemType[] items)
        {
            foreach (ItemType item in items)
                refHub.inventory.AddNewItem(item);
        }
        public void GiveItem(ItemType type) => refHub.inventory.AddNewItem(type);
        public void ClearItems() => refHub.inventory.Clear();

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



        //Obsolete stuff
        [Obsolete("Use PheggPlayer.godmode")]
        public bool GodMode()
        {
            return _characterClassManager.GodMode;
        }
        [Obsolete("Use PheggPlayer.godmode")]
        public void GodMode(bool godmode)
        {
            _characterClassManager.GodMode = (bool)godmode;
        }

        [Obsolete("Use PheggPlayer.bypass")]
        public bool Bypass()
        {
            return gameObject.GetComponent<ServerRoles>().BypassMode;
        }
        [Obsolete("Use PheggPlayer.bypass")]
        public void Bypass(bool bypass)
        {
            gameObject.GetComponent<ServerRoles>().BypassMode = bypass;
        }

        [Obsolete("Use PheggPlayer.disarmed")]
        public int Disarmed()
        {
            return _handcuffs.CufferId;
        }
        [Obsolete("Use PheggPlayer.disarmed")]
        public void Disarmed(int playerid)
        {
            _handcuffs.CufferId = this.playerId;
        }

        [Obsolete("Use PheggPlayer.health")]
        public float Health()
        {
            return _playerStats.Health;
        }
        [Obsolete("Use PheggPlayer.health")]
        public void Health(float hp)
        {
            _playerStats.Health = hp;
        }


        [Obsolete("Use PheggPlayer.team")]
        public TeamClass Teamclass()
        {
            RoleType role = _characterClassManager.CurClass;
            Team team = _characterClassManager.Classes.SafeGet(_characterClassManager.CurClass).team;
            CleanTeam cTeam;

            if (team == Team.MTF || team == Team.RSC)
                cTeam = CleanTeam.NineTailedFox;
            else if (team == Team.CHI || team == Team.CDP)
                cTeam = CleanTeam.ChaosInsurgency;
            else if (team == Team.SCP)
                cTeam = CleanTeam.SCP;
            else if (team == Team.TUT)
                cTeam = CleanTeam.Tutorial;
            else cTeam = CleanTeam.Spectator;

            return new TeamClass
            {
                role = role,
                team = team,
                cleanTeam = cTeam
            };
        }
        [Obsolete("Use PheggPlayer.team")]
        public void Teamclass(RoleType role)
        {
            _characterClassManager.SetClassID(role);
        }

        //Components
        [Obsolete("Use RefHub instead")]
        public Components commonComponents;
        #region components
        [Obsolete("Use RefHub instead")]
        private CharacterClassManager _characterClassManager { get; set; }
        [Obsolete("Use RefHub instead")]
        private ServerRoles _serverRoles { get; set; }
        [Obsolete("Use RefHub instead")]
        private NicknameSync _nicknameSync { get; set; }
        [Obsolete("Use RefHub instead")]
        private QueryProcessor _queryProcessor { get; set; }
        [Obsolete("Use RefHub instead")]
        private Handcuffs _handcuffs { get; set; }
        [Obsolete("Use RefHub instead")]
        private PlayerStats _playerStats { get; set; }
        [Obsolete("Use RefHub instead")]
        private AmmoBox _ammoBox { get; set; }
        [Obsolete("Use RefHub instead")]
        private Inventory _inventory { get; set; }
        [Obsolete("Use RefHub instead")]
        private PlayerMovementSync _plyMovementSync { get; set; }
        #endregion

        [Obsolete]
        public class TeamClass
        {
            public RoleType role { get; internal set; }
            public Team team { get; internal set; }
            public CleanTeam cleanTeam { get; internal set; }

        }

        [Obsolete("Use PheggPlayer.position")]
        public void Teleport(Vector3 vector3, float rotation = 0, bool forcegound = true)
        {
            _plyMovementSync.OverridePosition(vector3, rotation, forcegound);
        }
    }
}
