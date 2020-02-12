using Harmony;
using PheggMod.API.Commands;
using PheggMod.API.Events;
using PheggMod.API.Plugin;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
        public override void initializePlugin()
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            var harmony = HarmonyInstance.Create("com.phegg.pheggmod.test");
            harmony.PatchAll();

            this.AddEventHandlers(new Playerhurt());

            this.AddCommand(new myCommand(), "testing", new string[] { "test", "apples", "pears" });

            Info($"Plugin loaded successfully with {harmony.GetPatchedMethods().Count()} patched methods!");
        }
    }

    [HarmonyPatch(typeof(PlayerStats))]
    [HarmonyPatch("HurtPlayer")]
    public class MyPatchClass
    {
        [HarmonyPrefix]
        public static bool DoStuff(PlayerStats.HitInfo __0, GameObject __1)
        {
            Plugin.Info("Prefix");

            return true;
        }

        [HarmonyPostfix]
        public static void DoStuffAfter()
        {
            Plugin.Info("Postfix");
        }
    }
    internal class Playerhurt : IEventHandlerPlayerHurt, IEventHandlerAdminQuery
    {
        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            Plugin.Info($"{ev.Player.name} was injured {ev.DamageType.name}");
        }

        public void OnAdminQuery(AdminQueryEvent ev)
        {
            Plugin.Info($"{ev.Admin.name} ran command {ev.Query}");
        }
    }

    internal class myCommand : ICommand
    {
        public void HandleCommand(string command, GameObject admin, CommandSender sender)
        {
            sender.RaReply(command.Split(' ')[0].ToUpper() + "#What if mama said BOO!", true, true, "");

            admin.GetComponent<CharacterClassManager>().SetClassID(RoleType.Spectator);

            return;
        }
    }
}
