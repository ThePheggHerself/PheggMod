#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using Mirror;
using System.Net;
using MEC;

using PheggMod.API;

namespace PheggMod
{
    [MonoModPatch("global::PlayerStats")]
    class PMPlayerStats : PlayerStats
    {
        public extern bool orig_HurtPlayer(PlayerStats.HitInfo info, GameObject go);
        public new bool HurtPlayer(PlayerStats.HitInfo info, GameObject Player)
        {
            PlayerStats Pstats = Player.GetComponent<PlayerStats>();

            PheggPlayer pPlayer = new PheggPlayer(Player);

            if (Pstats.isLocalPlayer || info.Amount < 0 || !pPlayer.GetGodMode())
            {
                if (Pstats.health - info.Amount < 1) PlayerDie.OnPlayerDie(info, pPlayer, info.GetPlayerObject() == null ? null : new PheggPlayer(info.GetPlayerObject()));
                else PlayerHurt.OnPlayerHurt(info, pPlayer, info.GetPlayerObject() == null ? null : new PheggPlayer(info.GetPlayerObject()));
            }

            orig_HurtPlayer(info, Player);
            return false;
        }
    }

    class PlayerDie
    {
        internal static void OnPlayerDie(PlayerStats.HitInfo hitInfo, PheggPlayer player, PheggPlayer attacker = null)
        {
            

            if (RoundSummary.RoundInProgress())
            {
                BotMessage(hitInfo, player, attacker, RoundSummary.RoundInProgress());
            }
        }

        private static void BotMessage(PlayerStats.HitInfo hitInfo, PheggPlayer player, PheggPlayer attacker = null, bool inProgress)
        {
            if (attacker == null)
            {
                BotWorker.NewMessage($"{hitInfo.Attacker} killed {player.name} using {hitInfo.GetDamageName()}");
            }
            if (inProgress)
            {
                if (player.userId == attacker.userId)
                    BotWorker.NewMessage($"{player.name} committed scuicide using {hitInfo.GetDamageName()}");
                else if (player.teamRole.team == attacker.teamRole.team)
                    BotWorker.NewMessage($"**Teamkill** \n```autohotkey\nPlayer: {attacker.teamRole.role} {attacker.name} ({attacker.userId})\nKilled: {player.teamRole.role} {player.name} ({player.userId})\nUsing: {hitInfo.GetDamageName()}```");
                else if (player.GetDisarmed() > -1)
                    BotWorker.NewMessage($"__Disarmed Kill__ \n```autohotkey\nPlayer: {attacker.teamRole.role} {attacker.name} ({attacker.userId})\nKilled: {player.teamRole.role} {player.name} ({player.userId})\nUsing: {hitInfo.GetDamageName()}```");
            }

            else
                BotWorker.NewMessage($"{attacker.teamRole.role} {attacker.name} killed {player.teamRole.role} {player.name} using {hitInfo.GetDamageName()}");
        }
    }

    class PlayerHurt
    {
        public static List<int> InfectedPlayers = new List<int>();

        internal static void OnPlayerHurt(PlayerStats.HitInfo hitInfo, PheggPlayer player, PheggPlayer attacker = null)
        {
            BotMessage(hitInfo, player, attacker);
        }

        private static void BotMessage(PlayerStats.HitInfo hitInfo, PheggPlayer player, PheggPlayer attacker = null)
        {
            if (attacker == null)
            {
                BotWorker.NewMessage($"{hitInfo.Attacker} damaged {player.name} using {hitInfo.Amount}");
            }

            if (player.userId == attacker.userId)
                BotWorker.NewMessage($"{player.name} self harmed with {hitInfo.GetDamageName()} for ");
            else if (player.teamRole.team == attacker.teamRole.team)
                BotWorker.NewMessage($"**{attacker.teamRole.role} {attacker.name} ({attacker.userId}) attacked {attacker.teamRole.role} {player.name} ({player.userId}) for {Math.Round(hitInfo.Amount)} using {hitInfo.GetDamageName()}**");
            else if (player.GetDisarmed() > -1)
                BotWorker.NewMessage($"__{attacker.teamRole.role} {attacker.name} ({attacker.userId}) injured disarmed {player.teamRole.role} {player.name} ({player.userId}) for {Math.Round(hitInfo.Amount)} using {hitInfo.GetDamageName()}__");
            else
                BotWorker.NewMessage($"{attacker.name} -> {player.name} -> {Math.Round(hitInfo.Amount)} ({hitInfo.GetDamageName()})");
        }
    }
}
