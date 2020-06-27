using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Commands
{
    public class SlayCommand : ICommand
    {
        public string Command => "slay";

        public string[] Aliases { get; } = { "kill" };

        public string Description => "Kills the targeted player(s)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool canRun = CommandManager.CanRun(sender, PlayerPermissions.WarheadEvents, arguments, new[] { "Player" }, out response, out List<ReferenceHub> players);
            if (!canRun)
                return false;

            CommandSender cmdSender = sender as CommandSender;

            for (var i = 0; i < players.Count; i++)
                players[i].playerStats.HurtPlayer(new PlayerStats.HitInfo(9999f, cmdSender.Nickname, DamageTypes.Nuke, int.Parse(cmdSender.SenderId)), players[i].gameObject);

            response = $"Killed {players.Count} {(players.Count > 1 ? "players" : "player")}";

            return true;
        }
    }
}
