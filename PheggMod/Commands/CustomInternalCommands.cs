﻿using CustomPlayerEffects;
using GameCore;
using Grenades;
using MEC;
using Mirror;
using PheggMod.API.Commands;
using PheggMod.EventTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking.Match;
using Object = UnityEngine.Object;

namespace PheggMod.Commands
{
    public class CustomInternalCommands
    {
        internal static char[] validUnits = { 'm', 'h', 'd', 'w', 'M', 'y' };
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

        readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;

        //[PMCommand("help"), PMParameters("command"), PMConsoleRunnable(true), PMCommandSummary("Shows a summary of a given command")]
        public void cmd_help(CommandInfo info)
        {
            string msg;
            string q = info.commandArgs[1];

            if (!PluginManager.allCommands.ContainsKey(q))
                msg = "No command found!";
            else
            {
                MethodInfo cmd = PluginManager.allCommands[q];
                if (cmd == null || cmd.Equals(default(Type)))
                    msg = "No command found!";

                else
                {
                    PMCommandSummary pmSummary = (PMCommandSummary)cmd.GetCustomAttribute(typeof(PMCommandSummary));
                    PMParameters pmParams = (PMParameters)cmd.GetCustomAttribute(typeof(PMParameters));
                    PMPermission pmPerms = (PMPermission)cmd.GetCustomAttribute(typeof(PMPermission));

                    string usage = $"{q} {(pmParams != null ? $"[{string.Join("] [", pmParams.parameters).ToUpper()}]" : "")}";
                    string summary = pmSummary != null ? pmSummary.commandSummary : "No command summary found!";
                    string permission = pmPerms != null ? pmPerms.perm.ToString() : "No specific permissions required";

                    msg = $"Command info for: {q}"
                        + $"\nUsage: {usage}"
                        + $"\nSummary: {summary}"
                        + $"\nPermission: {permission}";
                }
            }

            if (info.gameObject != null)
                info.commandSender.RaReply(info.commandName.ToUpper() + $"#{msg}", true, false, "");
            else
                Base.Info(msg);
        }

        #region RA-Only Commands

        internal static bool isLightsout = false;
        [PMCommand("lightsout"), PMParameters(), PMPermission(PlayerPermissions.FacilityManagement)]
        public void cmd_Lightsout(CommandInfo info) => Timing.RunCoroutine(Lightsout(info));
        private IEnumerator<float> Lightsout(CommandInfo info)
        {
            if (!isLightsout)
            {
                isLightsout = true;

                info.commandSender.RaReply(info.commandName + $"#Facility lights have been disabled!", true, true, "");

                foreach (GameObject player in PlayerManager.players)
                    player.GetComponent<Broadcast>().TargetAddElement(player.GetComponent<NetworkConnection>(), $"Lightsout has been enabled. This will cause occasional rapid flickering of lights throughout HCZ and LCZ", 20, Broadcast.BroadcastFlags.Normal);

                yield return Timing.WaitForSeconds(9f);

                PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement("ERROR IN FACILITY LIGHT CONTROL . SYSTEM TERMINATION IN 3 . 2 . 1", false, true);

                foreach (GameObject player in PlayerManager.players)
                    player.GetComponent<Inventory>().AddNewItem(ItemType.Flashlight);

                yield return Timing.WaitForSeconds(11f);

                Timing.RunCoroutine(CheckLights());
            }
            else if (isLightsout)
            {
                isLightsout = false;
                info.commandSender.RaReply(info.commandName.ToUpper() + $"#Facility lights will be enabled next cycle!", true, true, "");
            }

            yield return 0f;
        }
        private IEnumerator<float> CheckLights()
        {
            yield return Timing.WaitForSeconds(33f);

            if (!isLightsout)
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement("FACILITY LIGHT CONTROL SYSTEM REPAIR COMPLETE . LIGHT SYSTEM ENGAGED", false, true);
                yield return 0f;
            }
            else
            {
                foreach (Generator079 gen in UnityEngine.Object.FindObjectsOfType<Generator079>())
                    gen.RpcCustomOverchargeForOurBeautifulModCreators(30f, false);

                Timing.RunCoroutine(CheckLights());
            }
        }

        internal static bool reloadPlugins = false;
        [PMCommand("pluginreload"), PMAlias("reloadplugins", "plreload"), PMParameters(), PMPermission(PlayerPermissions.ServerConfigs)]
        public void cmd_ReloadPlugins(CommandInfo info)
        {
            if (!reloadPlugins)
            {
                reloadPlugins = true;
                info.commandSender.RaReply(info.commandName + $"#Server plugins will be reloaded upon round restart.", true, true, "");

                Base.Warn($"{info.commandSender.Nickname} ({info.gameObject.GetComponent<CharacterClassManager>().UserId}) has triggered the pluginreload command!\nAll plugins and custom commands will be reloaded upon round restart");
            }
            else
            {
                info.commandSender.RaReply(info.commandName + $"#Server plugins are already set to reload upon round restart.", true, true, "");
            }
        }

        [PMCommand("curpos"), PMParameters("PlayerID"), PMCommandSummary("Tells you your current position")]
        public void cmd_pos(CommandInfo info)
        {
            Vector3 pos = info.gameObject.GetComponent<PlayerMovementSync>().RealModelPosition;

            info.commandSender.RaReply(info.commandName.ToUpper() + $"#Current player position: x={pos.x} y={pos.y} z={pos.z}", true, true, "");
        }

        [PMCommand("getsize"), PMAlias("getscale"), PMParameters("playerid"), PMCommandSummary("Sets the scale of a player"), PMPermission(PlayerPermissions.PlayersManagement), /*PMDisabled(true)*/]
        public void cmd_getsize(CommandInfo info)
        {
            List<GameObject> pList = GetPlayersFromString(info.commandArgs[1]);
            if (pList.Count < 1)
                info.commandSender.RaReply(info.commandName.ToUpper() + $"#No player found", false, true, "");

            else
                info.commandSender.RaReply(info.commandName.ToUpper() + $"#Scale of {pList[0].GetComponent<NicknameSync>().MyNick} is {pList[0].transform.localScale}", true, true, "");
        }

        // The code for the 3 commands was kindly donated to PheggMod by KrypTheBear#3301
        // Many thanks for the code <3

        [PMCommand("grenade"), PMParameters("player"), PMCommandSummary("Spawns a grenade at a player")]
        public void cmd_grenade(CommandInfo info)
        {
            Vector3 NULL_VECTOR = new Vector3(0, 0, 0);

            List<GameObject> playerList = GetPlayersFromString(info.commandArgs[1]);

            foreach (GameObject plr in playerList)
            {
                if (plr.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
                    continue;

                PheggPlayer pp = new PheggPlayer(plr);

                // Initialization
                GrenadeManager gm = pp.gameObject.GetComponent<GrenadeManager>();

                // Finalize creation & Spawn
                Grenade nade = Object.Instantiate<GameObject>(gm.availableGrenades[0].grenadeInstance).GetComponent<Grenade>();
                (nade).InitData(gm, NULL_VECTOR, NULL_VECTOR);
                NetworkServer.Spawn(nade.gameObject);
            }

            info.commandSender.RaReply(info.commandName.ToUpper() + $"#Spawned grenade on {(info.commandArgs[1] == "*" ? "All" : playerList.Count.ToString())} players", true, true, "");
        }
        [PMCommand("flash"), PMParameters("player"), PMCommandSummary("Spawns a flashbang at a player")]
        public void cmd_flash(CommandInfo info)
        {
            Vector3 NULL_VECTOR = new Vector3(0, 0, 0);

            List<GameObject> playerList = GetPlayersFromString(info.commandArgs[1]);

            foreach (GameObject plr in playerList)
            {
                if (plr.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
                    continue;

                PheggPlayer pp = new PheggPlayer(plr);

                // Initialization
                GrenadeManager gm = pp.gameObject.GetComponent<GrenadeManager>();

                // Finalize creation & Spawn
                Grenade nade = Object.Instantiate<GameObject>(gm.availableGrenades[1].grenadeInstance).GetComponent<Grenade>();
                (nade).InitData(gm, NULL_VECTOR, NULL_VECTOR);
                NetworkServer.Spawn(nade.gameObject);
            }

            info.commandSender.RaReply(info.commandName.ToUpper() + $"#Spawned flash on {(info.commandArgs[1] == "*" ? "All" : playerList.Count.ToString())} players", true, true, "");
        }
        [PMCommand("ball"), PMParameters("player"), PMCommandSummary("Spawns 018 at a player")]
        public void cmd_ball(CommandInfo info)
        {
            Vector3 NULL_VECTOR = new Vector3(0, 0, 0);

            List<GameObject> playerList = GetPlayersFromString(info.commandArgs[1]);

            foreach (GameObject plr in playerList)
            {
                if (plr.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
                    continue;

                PheggPlayer pp = new PheggPlayer(plr);

                // Initialization
                GrenadeManager gm = pp.gameObject.GetComponent<GrenadeManager>();

                // Finalize creation & Spawn
                Grenade nade = Object.Instantiate<GameObject>(gm.availableGrenades[2].grenadeInstance).GetComponent<Grenade>();
                (nade).InitData(gm, NULL_VECTOR, NULL_VECTOR);
                NetworkServer.Spawn(nade.gameObject);
            }

            info.commandSender.RaReply(info.commandName.ToUpper() + $"#Spawned 018 on {(info.commandArgs[1] == "*" ? "All" : playerList.Count.ToString())} players", true, true, "");
        }

        #endregion

        #region Server Console Commands

        [PMCommand("status"), PMAlias("serverstatus", "serverinfo", "sinfo"), PMParameters(), PMConsoleRunnable(true)]
        public void cmd_sinfo(CommandInfo info)
        {
            string playerCount = $"{PlayerManager.players.Count} / {ConfigFile.ServerConfig.GetInt("max_players", 20)}";
            string roundCount = Base.roundCount.ToString();
            string roundDuration = RoundSummary.RoundInProgress() == true ? $"{new DateTime(TimeSpan.FromSeconds((DateTime.Now - (DateTime)Base.roundStartTime).TotalSeconds).Ticks):HH:mm:ss}" : "Round not started";
            string timeSinceStart = $"{new DateTime(TimeSpan.FromSeconds((double)(new decimal(Time.realtimeSinceStartup))).Ticks):HH:mm:ss}";
            string curPlayerID = PlayerManager.localPlayer.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId.ToString();
            string memory = $"{ ((GC.GetTotalMemory(false) / 1024) / 1024) } MB";


            //(double)(new decimal(Time.realtimeSinceStartup))

            string status = "Server status:"
                    + $"\nPlayer count: {playerCount}"
                    + $"\nRound count: {roundCount}"
                    + $"\nRound duration: {roundDuration}"
                    + $"\nTime since startup: {timeSinceStart}"
                    + $"\nCurrent PlayerID: {curPlayerID}"
                    + $"\nMemory usage: {memory}"
                    ;


            if (info.gameObject != null)
                info.commandSender.RaReply(info.commandName.ToUpper() + $"#{status}", true, true, "");
            else
                Base.Info(status);

            //+ $"\nMemory usage: {GC.GetTotalMemory(false)}"
        }

        #endregion




        //[PMCommand("race"), PMParameters(), PMPermission(PlayerPermissions.PlayersManagement), PMCommandSummary("Starts an escape race event")]
        //public void cmd_race(CommandInfo info)
        //{
        //    if ((DateTime.Now - (DateTime)Base.roundStartTime).TotalSeconds > 30d)
        //    {
        //        info.commandSender.RaReply(info.commandName.ToUpper() + $"#You must run this command within the first 30 seconds of the round starting", false, true, "");
        //        return;
        //    }

        //    Timing.RunCoroutine(RaceEvent(info));
        //}
        //private IEnumerator<float> RaceEvent(CommandInfo info)
        //{
        //    RoundSummary.RoundLock = true;

        //    List<Door> doors = Object.FindObjectsOfType<Door>().ToList();

        //    foreach (Door d in doors)
        //    {
        //        d.lockdown = true;
        //        d.isOpen = false;
        //    }

        //    List<GameObject> players = PlayerManager.players;

        //    foreach (GameObject go in players)
        //    {
        //        go.GetComponent<CharacterClassManager>().SetClassID(RoleType.Scp173);

        //        Broadcast bc = go.GetComponent<Broadcast>();
        //        NetworkConnection nc = go.GetComponent<NetworkConnection>();



        //        bc.TargetAddElement(nc, "Welcome to peanut race", 5, Broadcast.BroadcastFlags.Normal);
        //        bc.TargetAddElement(nc, "Your goal is to reach the surface before the nuke detonates", 6, Broadcast.BroadcastFlags.Normal);
        //        bc.TargetAddElement(nc, "The nuke has been locked, so you are unable to disable it", 6, Broadcast.BroadcastFlags.Normal);
        //    }

        //    yield return Timing.WaitForSeconds(18);

        //    PlayerManager.localPlayer.GetComponent<PMAlphaWarheadController>().InstantPrepare();
        //    PlayerManager.localPlayer.GetComponent<PMAlphaWarheadController>().StartDetonation();


        //    EventTriggers.PMAlphaWarheadController.nukeLock = true;

        //    foreach (Door d in doors)
        //    {
        //        d.lockdown = false;
        //    }

        //    yield return Timing.WaitForSeconds(ConfigFile.ServerConfig.GetInt("warhead_tminus_start_duration", 90) + 5);

        //    RoundSummary.RoundLock = false;

        //    yield return 0f;
        //}

    }
}
