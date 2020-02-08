using Mirror;
using PheggMod.API.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class CustomCommandHandler
    {
        internal static char[] validUnits = { 'm', 'h', 'd', 'w', 'M', 'y' };

        internal CustomCommandHandler()
        {
            PluginManager.AddInternalCommand(new obanCommand(), "oban", new string[] { "offlineban", "dcban" });
            PluginManager.AddInternalCommand(new nukeLock(), "nuke", new string[] { });
            PluginManager.AddInternalCommand(new personalBroadcast(), "pbc", new string[] { "personalbroadcast", "privatebroadcast" });
            PluginManager.AddInternalCommand(new dropItems(), "drop", new string[] { "dropall", "dropinv" });
        }

        internal static TimeSpan GetBanDuration(char unit, int amount)
        {
            switch (unit)
            {
                default:
                    return new TimeSpan(0, 0, amount, 0);
                case 'h':
                    return new TimeSpan(0, amount, 0, 0);
                case 'd':
                    return new TimeSpan(amount, 0, 0, 0);
                case 'w':
                    return new TimeSpan(7 * amount, 0, 0, 0);
                case 'M':
                    return new TimeSpan(30 * amount, 0, 0, 0);
                case 'y':
                    return new TimeSpan(365 * amount, 0, 0, 0);
            }
        }

        public static bool CheckPermissions(CommandSender sender, string queryZero, PlayerPermissions perm)
        {
            if (ServerStatic.IsDedicated && sender.FullPermissions)
            {
                return true;
            }
            if (PermissionsHandler.IsPermitted(sender.Permissions, perm))
            {
                return true;
            }

            sender.RaReply(queryZero.ToUpper() + "#You don't have permissions to execute this command.\nMissing permission: " + perm, false, true, "");
            return false;
        }
    }

    internal class dropItems : ICommand
    {
        public void HandleCommand(string command, GameObject admin, CommandSender sender)
        {
            string[] arg = command.Split(' ');

            if (!CustomCommandHandler.CheckPermissions(sender, arg[0], PlayerPermissions.PlayersManagement))
                return;

            string[] playerStrings = arg[1].Split('.');
            List<GameObject> playerList = new List<GameObject>();

            foreach (string player in playerStrings)
            {
                GameObject go = PlayerManager.players.Where(p => p.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString() == player || p.GetComponent<NicknameSync>().MyNick == player).FirstOrDefault();
                if (go.Equals(default(GameObject)) || go == null) continue;
                else
                {
                    playerList.Add(go);
                }
            }

            foreach (GameObject player in playerList)
                player.GetComponent<Inventory>().ServerDropAll();
        }
    }
    internal class personalBroadcast : ICommand
    {
        public void HandleCommand(string command, GameObject admin, CommandSender sender)
        {
            string[] arg = command.Split(' ');

            if (!CustomCommandHandler.CheckPermissions(sender, arg[0], PlayerPermissions.Broadcasting))
                return;

            bool success = uint.TryParse(arg[3], out uint duration);

            if (arg.Count() < 4)
            {
                sender.RaReply(arg[0].ToUpper() + "#Command expects 3 or more arguments ([Players], [Seconds], [Message])", false, true, "");
                return;
            }
            else if (!success || duration < 1 || duration > 255)
            {
                sender.RaReply(arg[0].ToUpper() + "#Invalid duration given", false, true, "");
                return;
            }

            string[] playerStrings = arg[1].Split('.');
            List<GameObject> playerList = new List<GameObject>();

            foreach (string player in playerStrings)
            {
                GameObject go = PlayerManager.players.Where(p => p.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString() == player || p.GetComponent<NicknameSync>().MyNick == player).FirstOrDefault();
                if (go.Equals(default(GameObject)) || go == null) continue;
                else
                {
                    playerList.Add(go);
                }
            }

            string message = string.Join(" ", arg.Skip(3));

            foreach (GameObject player in playerList)
                player.GetComponent<Broadcast>().TargetAddElement(player.GetComponent<NetworkConnection>(), message, duration, false);
        }
    }
    internal class nukeLock : ICommand
    {
        public void HandleCommand(string command, GameObject admin, CommandSender sender)
        {
            string[] args = command.Split(' ');

            if (!CustomCommandHandler.CheckPermissions(sender, args[0], PlayerPermissions.WarheadEvents))
                return;

            switch (args[1].ToUpper())
            {
                case "LOCK":
                    Lock(args[0], sender);
                    return;
                case "ON":
                case "ENABLE":
                    Enable(args[0], sender);
                    return;
                case "OFF":
                case "DISABLE":
                    Disable(args[0], sender);
                    return;
                default:
                    sender.RaReply(args[1].ToUpper() + $"#{args[1]} <- Invalid argument [ON/OFF | LOCK]", true, true, "");
                    return;
            }            
        }

        public void Lock(string queryZero, CommandSender sender) {
            EventTriggers.PMAlphaWarheadController.nukeLock = !EventTriggers.PMAlphaWarheadController.nukeLock;

            sender.RaReply(queryZero.ToUpper() + $"#Warhead lock has been {(EventTriggers.PMAlphaWarheadController.nukeLock ? "enabled" : "disabled")}", true, true, "");
        }
        private void Enable(string queryZero, CommandSender sender)
        {
            EventTriggers.PMAlphaWarheadNukesitePanel.Enable();

            sender.RaReply(queryZero.ToUpper() + $"#Warhead has been turned on", true, true, "");
        }

        private void Disable(string queryZero, CommandSender sender)
        {
            EventTriggers.PMAlphaWarheadNukesitePanel.Disable();

            sender.RaReply(queryZero.ToUpper() + $"#Warhead has been turned off", true, true, "");
        }
    }
    internal class obanCommand : ICommand
    {
        public void HandleCommand(string command, GameObject admin, CommandSender sender)
        {
            string[] arg = command.Split(' ');

            if (arg.Count() < 4)
            {
                sender.RaReply(arg[0].ToUpper() + "#Command expects 3 or more arguments ([UserID], [Minutes], [Reason])", false, true, "");
                return;
            }
            else if (!arg[1].Contains('@'))
            {
                sender.RaReply(arg[0].ToUpper() + "#Invalid UserID given", false, true, "");
                return;
            }
            char unit = arg[2].ToString().Where(Char.IsLetter).ToArray()[0];

            if (!int.TryParse(new string(arg[2].Where(Char.IsDigit).ToArray()), out int amount) || !CustomCommandHandler.validUnits.Contains(unit) || amount < 1)
            {
                sender.RaReply(arg[0].ToUpper() + "#Invalid duration", false, true, "");
                return;
            }

            TimeSpan duration = CustomCommandHandler.GetBanDuration(unit, amount);
            string reason = string.Join(" ", arg.Skip(3));

            if (duration.Minutes > 60 && !CustomCommandHandler.CheckPermissions(sender, arg[0], PlayerPermissions.KickingAndShortTermBanning))
                return;
            else if (duration.Minutes > 1440 && !CustomCommandHandler.CheckPermissions(sender, arg[0], PlayerPermissions.BanningUpToDay))
                return;

            BanHandler.IssueBan(new BanDetails
            {
                OriginalName = "Offline player",
                Id = arg[2],
                Issuer = admin.GetComponent<NicknameSync>().MyNick,
                IssuanceTime = DateTime.UtcNow.Ticks,
                Expires = DateTime.UtcNow.Add(duration).Ticks,
                Reason = reason
            }, BanHandler.BanType.UserId);

            sender.RaReply(arg[0].ToUpper() + $"#{arg[1]} was offline banned for {arg[2]}", true, true, "");
        }
    }
}
