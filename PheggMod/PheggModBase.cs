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

        private extern void orig_FixedUpdate();

        private new void FixedUpdate()
        {
            //if(!_quitting && DateTime.Now.Hour == 5 && DateTime.Now.Minute == 30 && Time.realtimeSinceStartup > 60)
            //{
            //    AddLog("[PHEGGMOD] Restarting server!");

            //    _quitting = true;
            //    //Application.Quit();
            //    GameCore.Console.singleton.TypeCommand("QUIT");
            //}

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

            new SmartGuard();

            AddLog("[PHEGGMOD] THIS SERVER IS RUNNING PHEGGMOD");

            _debug = ConfigFile.ServerConfig.GetBool("pheggmod_debug", true);

            if (_debug)
            {
                Base.Debug("Debug mode enabled!");
                Base.Debug("Printing file paths:"
                    + $"\nUserIdBans.txt: {BanHandler.GetPath(BanHandler.BanType.UserId)}"
                    + $"\nIpBans.txt: {BanHandler.GetPath(BanHandler.BanType.IP)}"
                    + $"\nUserIdWhitelist.txt: {ConfigSharing.Paths[2] + "UserIDWhitelist.txt"}"
                    + $"\nUserIdReservedSlots.txt: {ConfigSharing.Paths[3] + "UserIDReservedSlots.txt"}");
            }

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
