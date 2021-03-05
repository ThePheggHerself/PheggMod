#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using MEC;
using System.Threading;
using System.Diagnostics;
using GameCore;
using UnityEngine.Networking;
using CommandSystem;
using PheggMod.EventTriggers;

namespace PheggMod
{
    [MonoModPatch("global::ServerConsole")]
    public class Base : ServerConsole
    {

        private static string _serverName = string.Empty;
        private bool _quitting = false;
        private static bool _debug;
        private string _restartTime;
        private int[] _restartTimeClean;
        public static int roundCount = 0;
        public static DateTime? roundStartTime = null;

        public static readonly List<string> colours = new List<string>
        {
            "RANDOMSTUPIDTEXTTOFORCEDEFAULT",
            "pink",
            "red",
            "brown",
            "silver",
            "light_green",
            "crimson",
            "cyan",
            "aqua",
            "deep_pink",
            "tomato",
            "yellow",
            "magenta",
            "blue_green",
            "orange",
            "lime",
            "green",
            "emerald",
            "carmine",
            "nickel",
            "mint",
            "army_green",
            "pumpkin"
        };

        private extern void orig_FixedUpdate();

        private new void FixedUpdate()
        {
            if (!_quitting && !string.IsNullOrEmpty(_restartTime))
            {
                if(_restartTimeClean == null || _restartTimeClean.Length == 0)
                {
                    string [] array = _restartTime.Split(':');
                    array[1].Replace(":", string.Empty);

                    _restartTimeClean = array.Select(x => Int32.Parse(x)).ToArray();
                }

                DateTime dTime = DateTime.Now;

                ///Checking for the startup time is simply a safety feature to ensure that the server isn't stuck in a boot loop for the while minute duration specified for the restart
                ///It's set for 60 seconds to ensure that the whole minute passes, meaning that regardless of server specs, it is impossible to start looping :D
                if (dTime.Hour == _restartTimeClean[0] && dTime.Minute == _restartTimeClean[1] && Time.realtimeSinceStartup > 60)
                {
					bool RoundOngoing = ReferenceHub.HostHub.characterClassManager.RoundStarted;

                    AddLog($"[PHEGGMOD] Server time has reached the specified restart time ({dTime.ToShortTimeString()}). The server will restart {(RoundOngoing ? "at the end of the current round" : "now")}");
					PMServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;

					if (!RoundOngoing)
						PMPlayerStats.StaticChangeLevel(true);

					_quitting = true;
                }
            }

            orig_FixedUpdate();
        }

        public extern static void orig_ReloadServerName();
        public new void ReloadServerName()
        {
            orig_ReloadServerName();
            _serverName += "<color=#ffffff00><size=1>PheggMod</size></color>";
        }
        public extern void orig_Start();
        public void Start()
        {
            orig_Start();

            CustomNetworkManager.Modded = true;
            AddLog("[PHEGGMOD] THIS SERVER IS RUNNING PHEGGMOD");

            //Commented out until SmartGuard is fixed
            //new SmartGuard();

            _debug = ConfigFile.ServerConfig.GetBool("pheggmod_debug", false);

            ///This is the time that the server will check for with the auto-restarting system.
            ///Uses 24 hour formatting (16:00 is 4PM), and uses the time relative to the server.
            ///Set to 25:00 to disable;
            _restartTime = ConfigFile.ServerConfig.GetString("auto_restart_time", "04:30");

            if (_debug)
            {
                Debug("Debug mode enabled!");
                Debug("Printing file paths:"
                    + $"\nUserIdBans.txt: {BanHandler.GetPath(BanHandler.BanType.UserId)}"
                    + $"\nIpBans.txt: {BanHandler.GetPath(BanHandler.BanType.IP)}"
                    + $"\nUserIdWhitelist.txt: {ConfigSharing.Paths[2] + "UserIDWhitelist.txt"}"
                    + $"\nUserIdReservedSlots.txt: {ConfigSharing.Paths[3] + "UserIDReservedSlots.txt"}");
            }

            new Commands.CustomInternalCommands();
            PluginManager.PluginPreLoad();
        }

        public static void Error(string m) => AddLog(string.Format("[{0}] {1}LOGTYPE-8", "ERROR", m));
        public static void Debug(string m)
        {
            if(_debug)
                AddLog(string.Format("[{0}] {1}", "DEBUG", m));
        }
        public static void Warn(string m) => AddLog(string.Format("[{0}] {1}", "WARN", m));
        public static void Info(string m) => AddLog(string.Format("[{0}] {1}", "INFO", m));
        public static void SmartGuard(string m) => AddLog(string.Format("[{0}] {1}", "SMART GUARD", m));
    }
}
