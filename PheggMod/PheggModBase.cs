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
using MEC;
using System.Threading;
using System.Diagnostics;
using GameCore;

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
                    AddLog($"[PHEGGMOD] Server time has reached {dTime.ToShortTimeString()}, so the server will now restart!");

                    _quitting = true;
                    GameCore.Console.singleton.TypeCommand("QUIT");
                }
            }

            orig_FixedUpdate();
        }

        public extern static void orig_ReloadServerName();
        public new void ReloadServerName()
        {
            orig_ReloadServerName();
            _serverName += "<color=#ffffff00><size=1>SMPheggMod</size></color>";
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

            if (_debug)
            {
                Base.Debug("Debug mode enabled!");
                Base.Debug("Printing file paths:"
                    + $"\nUserIdBans.txt: {BanHandler.GetPath(BanHandler.BanType.UserId)}"
                    + $"\nIpBans.txt: {BanHandler.GetPath(BanHandler.BanType.IP)}"
                    + $"\nUserIdWhitelist.txt: {ConfigSharing.Paths[2] + "UserIDWhitelist.txt"}"
                    + $"\nUserIdReservedSlots.txt: {ConfigSharing.Paths[3] + "UserIDReservedSlots.txt"}");
            }

            ///This is the time that the server will check for with the auto-restarting system.
            ///Uses 24 hour formatting (16:00 is 4PM), and uses the time relative to the server.
            ///Keeping as null will disable;
            _restartTime = ConfigFile.ServerConfig.GetString("auto_restart_time", null);

            new Commands.CustomInternalCommands();
            PluginManager.PluginPreLoad();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            Error($"{ex.Message}\n{ex.StackTrace}\n\n{new StackTrace(ex, true).GetFrame(0).GetFileLineNumber()}");
        }

        public static void Error(string m) => Base.AddLog(string.Format("[{0}] {1}LOGTYPE-8", "ERROR", m));
        public static void Debug(string m)
        {
            if(_debug)
                Base.AddLog(string.Format("[{0}] {1}", "DEBUG", m));
        }
        public static void Warn(string m) => Base.AddLog(string.Format("[{0}] {1}", "WARN", m));
        public static void Info(string m) => Base.AddLog(string.Format("[{0}] {1}", "INFO", m));
        public static void SmartGuard(string m) => Base.AddLog(string.Format("[{0}] {1}", "SMART GUARD", m));
    }
}
