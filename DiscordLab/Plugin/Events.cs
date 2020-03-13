using PheggMod.API.Events;
using System;

namespace DiscordLab
{
    internal class Events : IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandlerPlayerLeave, IEventHandlerAdminQuery, IEventHandlerGlobalBan, IEventHandlerLczDecontaminate, IEventHandlerPlayerBan,
        IEventHandlerPlayerDeath, IEventHandlerPlayerEscape, IEventHandlerPlayerHurt, IEventHandlerPlayerKick, IEventHandlerPlayerSpawn, IEventHandlerPlayerThrowGrenade, IEventHandlerRespawn, IEventHandlerRoundEnd,
        IEventHandlerRoundStart, IEventHandlerWarheadCancel, IEventHandlerWarheadDetonate, IEventHandlerWarheadStart
    {
        internal static DateTime RoundEnded;

        public void OnAdminQuery(AdminQueryEvent ev) => DiscordLab.bot.NewMessage($"```yaml\nAdmin: {ev.Admin.name}\nExecuted: {ev.Query.ToUpper()}```");

        public void OnGlobalBan(GlobalBanEvent ev) => DiscordLab.bot.NewMessage($"{ev.Player.ToString()} was globally banned for cheating");

        public void OnLczDecontaminate(LczDecontaminateEvent ev) => DiscordLab.bot.NewMessage($"Light containment zone decontamination has begun!");

        public void OnPlayerBan(PlayerBanEvent ev) =>
            DiscordLab.bot.NewMessage($"**New Ban!**```autohotkey\nUser: {ev.Player.ToString()}\nAdmin: {ev.Admin.ToString()}\nDuration: {ev.Duration} {(ev.Duration > 1 ? "minutes" : "minute")}```");

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (!RoundSummary.RoundInProgress() || ev.Attacker == null) return;

            if (ev.Attacker == ev.Player)
                DiscordLab.bot.NewMessage($"{ev.Player.name} committed suicide with {ev.DamageType.name}");
            else if (ev.Attacker.Teamclass().team == ev.Player.Teamclass().team)
                DiscordLab.bot.NewMessage($"**Teamkill** \n```autohotkey\nPlayer: {ev.Attacker.Teamclass().role} {ev.Attacker.ToString()}"
                    + $"\nKilled: {ev.Player.Teamclass().role} {ev.Player.ToString()}\nUsing: {ev.DamageType.name}```");
            else if (ev.Player.Disarmed() > 0 && !ev.Attacker.gameObject.GetComponent<CharacterClassManager>().IsAnyScp())
                DiscordLab.bot.NewMessage($"__Disarmed Kill__\n```autohotkey\nPlayer: {ev.Attacker.Teamclass().role} {ev.Attacker.ToString()}"
                    + $"\nKilled: {ev.Player    .Teamclass().role} {ev.Player.ToString()}\nUsing: {ev.DamageType.name}```");
            else
                DiscordLab.bot.NewMessage($"{ev.Attacker.Teamclass().role} {ev.Attacker.name} killed {ev.Player.Teamclass().role} {ev.Player.name} with {ev.DamageType.name}");

        }

        public void OnPlayerEscape(PlayerEscapeEvent ev) => DiscordLab.bot.NewMessage($"{ev.Player.name} escaped the facility and became {ev.newRole}");

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (!RoundSummary.RoundInProgress() || ev.Attacker == null || ev.Attacker.gameObject.GetComponent<CharacterClassManager>().SpawnProtected) return;

            if (ev.Attacker.Teamclass().team == ev.Player.Teamclass().team)
                DiscordLab.bot.NewMessage($"**{ev.Attacker.Teamclass().role} {ev.Attacker.ToString()} attacked {ev.Player.Teamclass().role} {ev.Player.ToString()} for {Math.Round(ev.Damage)} with {ev.DamageType.name}**");
            else if (ev.Player.Disarmed() > 0 && !ev.Attacker.gameObject.GetComponent<CharacterClassManager>().IsAnyScp())
                DiscordLab.bot.NewMessage($"__{ev.Attacker.Teamclass().role} {ev.Attacker.ToString()} attacked {ev.Player.Teamclass().role} {ev.Player.ToString()} for {Math.Round(ev.Damage)} with {ev.DamageType.name}__");
            else
                DiscordLab.bot.NewMessage($"{ev.Attacker.name} -> {ev.Player.name} -> {Math.Round(ev.Damage)} ({ev.DamageType.name})");
        }

        public void OnPlayerJoin(PlayerJoinEvent ev) => DiscordLab.bot.NewMessage($"**{ev.Player.name} ({ev.Player.userId} from ||~~{ev.Player.ipAddress}~~||) has joined the server**");

        public void OnPlayerKick(PlayerKickEvent ev) => DiscordLab.bot.NewMessage($"**Player kicked!**```autohotkey\nUser: {ev.Player.ToString()}\nAdmin: {(ev.Admin.ToString() == " ()" ? "Server console" : ev.Admin.ToString())}```");

        public void OnPlayerLeave(PlayerLeaveEvent ev) => DiscordLab.bot.NewMessage($"{ev.Player.ToString()} disconnected from the server.");

        private string _lastSpawnMessage = "a";
        public void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            string newMsg = $"{ev.Player.name} spawned as {ev.Role}";
            if (_lastSpawnMessage == newMsg) return;

            DiscordLab.bot.NewMessage(newMsg);
            _lastSpawnMessage = newMsg;
        }

        public void OnRespawn(RespawnEvent ev) => DiscordLab.bot.NewMessage(ev.IsCI ? "**Attention all personnel: Chaos insurgency breach in progress**" : "**Mobile Task Force unit Epsilon 11 has entered the facility!**");

        public void OnRoundEnd(RoundEndEvent ev)
        {
            DiscordLab.bot.NewMessage($"**Round Ended**\n```Round Time: {ev.RoundTime}"
                + $"\nEscaped Class-D: {ev.Class_D.Escaped_ClassD}/{ev.Class_D.Starting_ClassD}"
                + $"\nRescued Scientists: {ev.Scientist.Escaped_Scientists}/{ev.Scientist.Starting_Scientists}"
                + $"\nTerminated SCPs: {ev.SCP.Terminated_SCPs}/{ev.SCP.Starting_SCPs}"
                + $"\nWarhead Status: {(AlphaWarheadController.Host.detonated == false ? "Not Detonated" : $"Detonated")}```");

            Events.RoundEnded = DateTime.Now;
        }

        public void OnRoundStart(RoundStartEvent ev) => DiscordLab.bot.NewMessage($"**A new round has begun**");

        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev) => DiscordLab.bot.NewMessage($"{ev.Player.name} threw {ev.Grenade}");

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev) => DiscordLab.bot.NewMessage("Waiting for players...");

        public void OnWarheadCancel(WarheadCancelEvent ev) => DiscordLab.bot.NewMessage($"{ev.Disablier.name} canceled the warhead detonation");

        public void OnWarheadDetonate(WarheadDetonateEvent ev) => DiscordLab.bot.NewMessage("The alpha warhead has been detonated");

        public void OnWarheadStart(WarheadStartEvent ev) => DiscordLab.bot.NewMessage(
            ev.InitialStart ? $"**Alpha warhead detonation sequence engaged! The underground ection of the facility will be detonated in T-minus {ev.TimeToDetonation} seconds!**" :
            $"**Alpha warhead detonation sequence resumed! {ev.TimeToDetonation} seconds to detonation!**");
    }
}