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

namespace PheggMod
{
    [MonoModPatch("global::ServerConsole")]
    public class Base : ServerConsole
    {
        private static string _serverName = string.Empty;

        public extern static void orig_ReloadServerName();
        public new void ReloadServerName()
        {
            orig_ReloadServerName();
            _serverName += "<color=#ffffff00><size=1>SMPheggMod</size></color>";
        }
        public extern void orig_Start();
        public void Start()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            orig_Start();

            CustomNetworkManager.Modded = true;

            AddLog("[PHEGGMOD] THIS SERVER IS RUNNING PHEGGMOD");

            new Commands.CustomCommandHandler();

            PluginManager.PluginPreLoad();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            Error($"{ex.Message}\n{ex.StackTrace}");
        }

        public static void Error(string m) => Base.AddLog(string.Format("[{0}] {1}LOGTYPE-8", "ERROR", m));
        public static void Warn(string m) => Base.AddLog(string.Format("[{0}] {1}", "WARN", m));
        public static void Info(string m) => Base.AddLog(string.Format("[{0}] {1}", "INFO", m));
    }
}
