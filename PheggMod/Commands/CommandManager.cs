using CommandSystem;
using PheggMod.Commands.NukeCommand;
using PheggMod.Commands.PositionCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.Commands
{
    public class CommandManager
    {
        private static Dictionary<Type, ICommand> commands = new Dictionary<Type, ICommand>();

        public static void RegisterInternalCommands()
        {
            try
            {
                CheckCommand<NukeParentCommand>(typeof(NukeParentCommand));
                CheckCommand<PositionParentCommand>(typeof(PositionParentCommand));



                CheckCommand<ClearupCommand>(typeof(ClearupCommand));
                CheckCommand<DropCommand>(typeof(DropCommand));
                //CheckCommand<LockRespawnCommand>(typeof(LockRespawnCommand));
                CheckCommand<PersonalBroadcastCommand>(typeof(PersonalBroadcastCommand));
                CheckCommand<PocketCommand>(typeof(PocketCommand));
                CheckCommand<SizeCommand>(typeof(SizeCommand));
                CheckCommand<TestCommand>(typeof(TestCommand));
                CheckCommand<Tower2Command>(typeof(Tower2Command));
            }
            catch(Exception e)
            {
                Base.Error(e.ToString());
            }

        }

        private static void CheckCommand<T>(Type type) where T : ICommand
        {
            if (commands.ContainsKey(type))
            {
                PluginManager.AddCommand(commands[type]);
            }
            else
            {
                var cmd = (T)Activator.CreateInstance(typeof(T));
                PluginManager.AddCommand(cmd);
                commands.Add(type, cmd);
            }
        }

        public static bool CanRun(ICommandSender sender, PlayerPermissions? perm, out string response)
        {
            if (sender is CommandSender cmdSender)
            {
                if (perm == null || PermissionsHandler.IsPermitted(cmdSender.Permissions, (PlayerPermissions)perm))
                {
                    response = string.Empty;
                    return true;
                }

                response = "You don't have permission to execute this command.\nMissing permission: " + perm;
                return false;
            }
            else
            {
                response = "No CommandSender found!";
                return false;
            }
        }
        public static bool CanRun(ICommandSender sender, PlayerPermissions? perm, ArraySegment<string> arg, string[] expectedArgs, out string response)
        {
            bool success = CanRun(sender, perm, out response);
            if (!success)
                return false;

            if (arg.Count < expectedArgs.Length)
            {
                response = $"Command expects {expectedArgs.Length} or more arguments ([{string.Join("] [", Array.ConvertAll(expectedArgs, s => s.ToUpper()))}])";
                return false;
            }

            response = string.Empty;
            return true;
        }
        public static bool CanRun(ICommandSender sender, PlayerPermissions? perm, ArraySegment<string> arg, string[] expectedArgs, out string response, out List<ReferenceHub> players)
        {
            players = new List<ReferenceHub>();

            bool success = CanRun(sender, perm, arg, expectedArgs, out response);
            if (!success)
                return false;

            players = GetPlayersFromString(arg.Array[1]);
            if (players.Count < 1)
            {
                response = "No valid players found";
                return false;
            }

            response = string.Empty;
            return true;
        }

        public static List<ReferenceHub> GetPlayersFromString(string users)
        {
            if (users.ToLower() == "*")
                return ReferenceHub.GetAllHubs().Values.ToList();

            string[] playerStrings = users.Split('.');
            List<ReferenceHub> playerList = new List<ReferenceHub>();
            List<ReferenceHub> hubs = ReferenceHub.GetAllHubs().Values.ToList();

            foreach (string player in playerStrings)
            {
                if (int.TryParse(player, out int id) && ReferenceHub.TryGetHub(id, out ReferenceHub hub))
                    playerList.Add(hub);
                else
                {
                    int index = hubs.FindIndex(p => p.queryProcessor.PlayerId.ToString() == player || p.characterClassManager.UserId == player || (player.Length > 2 && p.nicknameSync.MyNick.ToLower().Contains(player)));
                    if (index > -1)
                    {
                        playerList.Add(hubs[index]);
                    }
                }
            }

            return playerList;
        }
        public static bool TryParseVector3(string[] args, out Vector3 vector3)
        {
            vector3 = new Vector3();

            if (!float.TryParse(args[0], out float x))
                return false;
            if (!float.TryParse(args[0], out float y))
                return false;
            if (!float.TryParse(args[0], out float z))
                return false;

            vector3 = new Vector3(x, y, z);
            return true;
        }
    }
}
