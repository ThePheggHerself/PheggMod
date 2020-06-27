using CommandSystem;
using PheggMod.Commands.NukeCommand;
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
            CheckCommand<NukeParentCommand>(typeof(NukeParentCommand));
            CheckCommand<PersonalBroadcastCommand>(typeof(PersonalBroadcastCommand));
            CheckCommand<DropCommand>(typeof(DropCommand));
            CheckCommand<SlayCommand>(typeof(SlayCommand));
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

        public static bool CanRun(ICommandSender sender, PlayerPermissions perm, out string response)
        {
            if (sender is CommandSender cmdSender)
            {
                if (PermissionsHandler.IsPermitted(cmdSender.Permissions, perm))
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

        public static bool CanRun(ICommandSender sender, PlayerPermissions perm, ArraySegment<string> arg, string[] expectedArgs, out string response)
        {
            if (sender is CommandSender cmdSender)
            {
                if (PermissionsHandler.IsPermitted(cmdSender.Permissions, perm))
                {
                    if (arg.Count < expectedArgs.Length)
                    {
                        response = $"Command expects {expectedArgs.Length} or more arguments ([{string.Join("] [", Array.ConvertAll(expectedArgs, s => s.ToUpper()))}])";
                        return false;
                    }

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

        public static bool CanRun(ICommandSender sender, PlayerPermissions perm, ArraySegment<string> arg, string[] expectedArgs, out string response, out List<ReferenceHub> players)
        {
            players = new List<ReferenceHub>();
            if (sender is CommandSender cmdSender)
            {
                if (PermissionsHandler.IsPermitted(cmdSender.Permissions, perm))
                {
                    if (arg.Count < expectedArgs.Length)
                    {
                        response = $"Command expects {expectedArgs.Length} or more arguments ([{string.Join("] [", Array.ConvertAll(expectedArgs, s => s.ToUpper()))}])";
                        return false;
                    }

                    players = GetPlayersFromString(arg.Array[1]);
                    if(players.Count < 1)
                    {
                        response = "No valid players found";
                        return false;
                    }

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

        public static List<ReferenceHub> GetPlayersFromString(string users)
        {
            if (users.ToLower() == "*")
                return ReferenceHub.GetAllHubs().Values.ToList();

            string[] playerStrings = users.Split('.');
            List<ReferenceHub> playerList = new List<ReferenceHub>();
            List<ReferenceHub> hubs = ReferenceHub.GetAllHubs().Values.ToList();

            foreach (string player in playerStrings)
            {
                int index = hubs.FindIndex(p => p.queryProcessor.PlayerId.ToString() == player || p.nicknameSync.MyNick.ToLower().Contains(player) || p.characterClassManager.UserId == player);
                if(index > -1)
                {
                    playerList.Add(hubs[index]);
                }
            }

            return playerList;
        }
    }
}
