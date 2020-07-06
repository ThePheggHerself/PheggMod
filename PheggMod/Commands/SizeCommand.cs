using CommandSystem;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class SizeCommand : ICommand
    {
        readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;

        public string Command => "size";

        public string[] Aliases => null;

        public string Description => "Sets the scale of a player";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.PlayersManagement, arguments, new[] { "player", "scale" }, out response, out List<ReferenceHub> hubs);
            if (!canRun)
                return false;

            GameObject[] lobbyPlayers = PlayerManager.players.ToArray();

            if (!float.TryParse(arguments.Array[2], out float scale))
            {
                response = $"#Invalid scale given (Use numbers)";
                return false;
            }

            foreach(ReferenceHub refhub in hubs)
            {
                NetworkIdentity nId = refhub.networkIdentity;
                refhub.gameObject.transform.localScale = new Vector3(1 * scale, 1 * scale, 1 * scale);
                ObjectDestroyMessage dMsg = new ObjectDestroyMessage { netId = nId.netId };

                for(int i = 0; i < lobbyPlayers.Length; i++)
                {
                    NetworkConnection conn = lobbyPlayers[i].GetComponent<NetworkIdentity>().connectionToClient;

                    if (lobbyPlayers[i] != refhub.gameObject)
                        conn.Send(dMsg, 0);

                    typeof(NetworkServer).GetMethod("SendSpawnMessage", flags).Invoke(null, new object[] { nId, conn });
                }
            }

            response = $"#Scale of {hubs.Count} {(hubs.Count != 1 ? "players" : "player")} has been set to {scale}";
            return true;
        }
    }
}
