using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.Commands.NukeCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PersonalBroadcastCommand : ICommand
    {
        public string Command => "pbc";

        public string[] Aliases { get; } = { "personalbroadcast", "privatebroadcast", "pbcmono", "personalbroadcastmono", "privatebroadcastmono" };

        public string Description => "Sends a private broadcast message to the specified user(s)";

        public bool Execute(ArraySegment<string> arg, ICommandSender sender, out string response)
        {
            bool success = CommandManager.CheckPermission(sender, PlayerPermissions.Broadcasting, out bool isSender, out bool hasPerm);

            if (!isSender)
            {
                response = "No CommandSender found";
            }
            else if(!hasPerm)
                response = $"You don't have permission to execute this command.\nMissing permission: " + PlayerPermissions.Broadcasting;

            else
            {
                if (arg.Count < 4)
                {
                    response = "Command expects 3 or more arguments ([Players], [Seconds], [Message])";
                    success = false;
                }
                else if(!ushort.TryParse(arg.Array[2], out ushort duration) || duration < 1 || duration > 254)
                {
                    response = "Invalid duration given";
                    success = false;
                }
                else
                {
                    List<GameObject> playerList = CustomInternalCommands.GetPlayersFromString(arg.Array[1]);
                    if(playerList.Count < 1)
                    {
                        response = "No valid players found";
                        success = false;
                    }
                    else
                    {
                        for(var i = 0; i < playerList.Count; i++)
                        {
                            GameObject go = playerList[i];
                            go.GetComponent<Broadcast>().TargetAddElement
                                (go.GetComponent<NetworkConnection>(), string.Join(" ", arg.Skip(3)), duration, arg.Array[0].ToLower().Contains("mono") ? Broadcast.BroadcastFlags.Monospaced : Broadcast.BroadcastFlags.Normal);
                        }

                        response = "Broadcast sent";
                    }
                }
            }

            return success;
        }
    }
}
