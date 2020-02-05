using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Harmony;

using PheggMod.API.Plugin;
using PheggMod.API.Events;
using UnityEngine;
using System;

namespace ExamplePlugin
{
    [Plugin.PluginDetails(
    author = "ThePheggHimself",
    name = "TestPlugin",
    description = "Test plugin for Pheggmod",
    version = "1.0_BETA"
    )]

    public class ExamplePlugin : Plugin
    {
        public static Plugin plugin;

        public override void initializePlugin()
        {
            try
            {

                plugin = this;

                Assembly assembly = Assembly.GetCallingAssembly();

                var harmony = HarmonyInstance.Create("com.phegg.pheggmod.test");
                //harmony.PatchAll(assembly);
                harmony.PatchAll();

                this.AddEventHandlers(new Playerhurt(this));

                Info(assembly.FullName);

                //Info($"{Assembly.GetCallingAssembly().GetType("PocketDimensionTeleport").GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance).Name}");

                Info($"Plugin loaded successfully with {harmony.GetPatchedMethods().Count()} patched methods!");
            }
            catch(Exception e)
            {
                Error(e.InnerException.Message);
                Error(e.InnerException.StackTrace);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerStats))]
    [HarmonyPatch("HurtPlayer")]
    public class MyPatchClass
    {
        [HarmonyPrefix]
        public static bool DoStuff(PlayerStats.HitInfo __0, GameObject __1)
        {
            ExamplePlugin.plugin.Info("Prefix");

            return true;
        }

        [HarmonyPostfix]
        public static void DoStuffAfter()
        {
            ExamplePlugin.plugin.Info("Postefix");
        }
    }
}
