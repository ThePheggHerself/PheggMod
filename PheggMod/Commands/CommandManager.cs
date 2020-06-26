using CommandSystem;
using PheggMod.Commands.NukeCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class CommandManager
    {
        public CommandManager()
        {
            PluginManager.AddCommand(new NukeParentCommand());
            PluginManager.AddCommand(new PersonalBroadcastCommand());
        }

        public static bool CheckPermission(ICommandSender sender, PlayerPermissions perm, out bool IsSender, out bool HasPermission)
        {
            if (sender is CommandSender cmdSender)
            {
                IsSender = true;
                if (PermissionsHandler.IsPermitted(cmdSender.Permissions, perm))
                {
                    HasPermission = true;
                    return true;
                }

                else
                {
                    HasPermission = false;
                    return false;
                }
            }
            else
            {
                IsSender = false;
                HasPermission = false;
                return false;
            }
        }
        public static List<GameObject> GetPlayersFromString(string users)
        {
            if (users.ToLower() == "*")
                return PlayerManager.players;

            string[] playerStrings = users.Split('.');
            List<GameObject> playerList = new List<GameObject>();

            foreach (string player in playerStrings)
            {
                GameObject go = PlayerManager.players.Where(p => p.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString() == player
                || p.GetComponent<NicknameSync>().MyNick.ToLower() == player || p.GetComponent<CharacterClassManager>().UserId2 == player).FirstOrDefault();
                if (go.Equals(default(GameObject)) || go == null) continue;
                else
                {
                    playerList.Add(go);
                }
            }

            return playerList;
        }
    }
}
