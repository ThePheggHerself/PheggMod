using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod
{
    public class PheggPlayer
    {
        public string name { get; internal set; }
        public string userId { get; internal set; }
        public string domain { get; internal set; }
        public string ipAddress { get; internal set; }
        public int playerId { get; internal set; }

        public TeamRole teamRole { get; internal set; }

        private CharacterClassManager cmm { get; set; }
        private ServerRoles sroles { get; set; }
        private NicknameSync nsync { get; set; }
        private RemoteAdmin.QueryProcessor qproc { get; set; }
        private Handcuffs cuffs { get; set; }
        private PlayerStats pstats { get; set; }

        public GameObject gameObject { get; internal set; }

        public class TeamRole
        {
            public RoleType role { get; internal set; }
            public Team team { get; internal set; }
        }

        public PheggPlayer(GameObject player)
        {
            #region Components
            cmm = player.GetComponent<CharacterClassManager>();
            sroles = player.GetComponent<ServerRoles>();
            nsync = player.GetComponent<NicknameSync>();
            qproc = player.GetComponent<RemoteAdmin.QueryProcessor>();
            cuffs = player.GetComponent<Handcuffs>();
            pstats = player.GetComponent<PlayerStats>();
            #endregion

            name = nsync.MyNick;
            userId = cmm.UserId;
            domain = cmm.UserId.Split('@')[1].ToUpper();
            ipAddress = nsync.connectionToClient.address;
            playerId = qproc.PlayerId;

            teamRole = new TeamRole { role = cmm.CurClass, team = cmm.Classes.SafeGet(cmm.CurClass).team };

            gameObject = player;
        }

        #region Godmode
        public bool GetGodMode()
        {
            return cmm.GodMode;
        }

        public void SetGodMode(bool godmode)
        {
            cmm.GodMode = (bool)godmode;
        }
        #endregion
        #region Bypass Mode
        public bool GetBypass()
        {
            return gameObject.GetComponent<ServerRoles>().BypassMode;
        }

        public void SetBypass(bool bypass)
        {
            gameObject.GetComponent<ServerRoles>().BypassMode = bypass;
        }
        #endregion
        #region Cuffs
        public int GetDisarmed()
        {
            return cuffs.CufferId;
        }

        public void SetDisarmed(int playerid)
        {
            cuffs.CufferId = this.playerId;
        }
        #endregion
    }
}
