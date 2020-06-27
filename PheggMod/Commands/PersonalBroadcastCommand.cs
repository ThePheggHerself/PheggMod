using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.Commands
{
    public class PersonalBroadcastCommand : ICommand
    {
        public string Command => "pbc";

        public string[] Aliases { get; } = { "personalbroadcast", "privatebroadcast", "pbcmono", "personalbroadcastmono", "privatebroadcastmono" };

        public string Description => "Sends a private broadcast message to the specified player(s)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.WarheadEvents, arguments, new[] { "Player" }, out response, out List<ReferenceHub> players);
            if (!canRun)
                return false;

            if (!ushort.TryParse(arguments.Array[2], out ushort duration) || duration < 1 || duration > 254)
            {
                response = "Invalid duration given";
                return false;
            }

            for (var i = 0; i < players.Count; i++)
            {
                GameObject go = players[i].gameObject;
                go.GetComponent<Broadcast>().TargetAddElement
                    (go.GetComponent<NetworkConnection>(), string.Join(" ", arguments.Skip(3)), duration, arguments.Array[0].ToLower().Contains("mono") ? Broadcast.BroadcastFlags.Monospaced : Broadcast.BroadcastFlags.Normal);
            }

            response = "Broadcast sent";
            return canRun;
        }
    }
}
