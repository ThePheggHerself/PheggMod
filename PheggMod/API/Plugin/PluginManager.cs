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
using GameCore;

using PheggMod.API.Events;
using PheggMod.API.Plugin;

namespace PheggMod
{
    public class PluginManager
    {
        public static List<Assembly> plugins = new List<Assembly>();
        public static List<Tuple<Type, IEventHandler>> allEvents = new List<Tuple<Type, IEventHandler>>();

        public static void PluginPreLoad()
        {
            bool universalConfigs = ConfigFile.ServerConfig.GetBool("universal_config_file", false);

            Base.AddLog(universalConfigs.ToString());

            LoadPlugins(universalConfigs == false ? FileManager.GetAppFolder(true, true) + "plugins" : AppDomain.CurrentDomain.BaseDirectory + FileManager.GetPathSeparator() + "plugins" + "/../plugins");
        }

        private static void LoadPlugins(string pluginsFolder)
        {
            if (Directory.Exists(pluginsFolder))
            {
                List<string> files = Directory.GetFiles(pluginsFolder).ToList<string>();
                List<string> nondll = new List<string>();


                foreach (string dllfile in files)
                {
                    if (dllfile.EndsWith(".dll"))
                    {
                        Assembly asm = Assembly.LoadFrom(dllfile);

                        Base.Info("PLUGIN LOADER | Loading plugin " + asm.GetName().Name);
                        try
                        {
                            foreach (Type t in asm.GetTypes())
                            {
                                if (t.IsSubclassOf(typeof(API.Plugin.Plugin)) && t != typeof(API.Plugin.Plugin))
                                {
                                    plugins.Add(asm);

                                    MethodInfo method = t.GetMethod("initializePlugin");
                                    object obj = Activator.CreateInstance(t);
                                    string aaa = (string)method.Invoke(obj, null);
                                }
                            }
                        }
                        catch (TargetInvocationException e)
                        {
                            Base.Error("PLUGIN LOADER | Failed to load file: " + asm.GetName().Name);
                            Base.Error($"PLUGIN LOADER | {e.Message}");
                            Base.Error($"PLUGIN LOADER | {e.InnerException}");
                        }
                    }
                }
            }

            //HandlersList(plugins);
        }

        public static void AddEventHandlers(Plugin plugin, IEventHandler handler)
        {
            foreach (Type intface in handler.GetType().GetInterfaces())
            {
                if (typeof(IEventHandler).IsAssignableFrom(intface) && intface != typeof(IEventHandler))
                {
                    allEvents.Add(new Tuple<Type, IEventHandler>(intface, handler));
                }
            }
        }

        internal static void TriggerEvent<t1>(Event ev) where t1 : IEventHandler
        {
            foreach (t1 handler in GetEvents<t1>())
            {
                try
                {
                    if (handler is t1 thandler) ev.ExecuteHandler(thandler);
                }
                catch (Exception e) { Base.Error($"{e.Message}\n{e.StackTrace}"); }
            }
        }

        internal static List<IEventHandler> GetEvents<t1>() where t1 : IEventHandler
        {
            List<IEventHandler> events = new List<IEventHandler>();

            foreach (Tuple<Type, IEventHandler> handler in allEvents)
            {
                if (typeof(t1).IsAssignableFrom(handler.Item1))
                {
                    events.Add(handler.Item2);
                }
            }

            return events;
        }
    }
}
