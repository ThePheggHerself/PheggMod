using Harmony;
using PheggMod.API.Commands;
using PheggMod.API.Events;
using PheggMod.API.Plugin;
using PheggMod.Commands;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ExamplePlugin
{
    /// <summary>
    /// Details of the plugin.
    /// </summary>
    [Plugin.PluginDetails(
    author = "ThePheggHimself",
    name = "TestPlugin",
    description = "Test plugin for Pheggmod",
    version = "1.0_BETA"
    )]

    public class ExamplePlugin : Plugin
    {
        /// <summary>
        /// This is the very first thing called when a plugin is loaded. Without this method, the plugin will do NOTHING.
        /// </summary>
        public override void initializePlugin()
        {
            var harmony = HarmonyInstance.Create("com.phegg.pheggmod.test");
            harmony.PatchAll();

            this.AddEventHandlers(new Playerhurt());
            this.AddCommand(new TestCommand(), "Test", new string[] { "apples", "pears" });

            Info($"Plugin loaded successfully with {harmony.GetPatchedMethods().Count()} patched methods!");
        }
    }

    internal class TestCommand : ICommand
    {
        public void HandleCommand(string command, GameObject admin, CommandSender sender)
        {
            sender.RaReply(command.Split(' ')[0] + "#You don't have permissions to execute this command.\nMissing permission: ", false, true, "");
        }
    }

    #region Harmony Example
    /// <summary>
    /// Here is an example of how to use Harmony with Pheggmod.
    /// The following code below hooks onto the "HurtPlayer" event from the PlayerStats class
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats))]
    [HarmonyPatch("HurtPlayer")]
    public class MyPatchClass
    {
        /// <summary>
        /// The code below will trigger BEFORE the original HurtPlayer method is run.
        /// In this example, it simply prints "Prefix" to the server console.
        /// </summary>
        [HarmonyPrefix]
        public static bool DoStuff(PlayerStats.HitInfo __0, GameObject __1)
        {
            Plugin.Info("Prefix");

            return true;
        }

        /// <summary>
        /// The code below will trigger AFTER the original HurtPlayer method is run.
        /// In this example, it simply prints "Postfix" to the server console.
        /// </summary>
        [HarmonyPostfix]
        public static void DoStuffAfter()
        {
            Plugin.Info("Postfix");
        }
    }
    #endregion

    #region PheggEvents Example
    /// <summary>
    /// Here is an example of PheggEvents. These are pre-made events that will trigger given the correct conditions within the code are met.
    /// They simply exist so that you don't need to keep using Harmony hooks for the main events that people will want/need.
    /// </summary>
    internal class Playerhurt : IEventHandlerPlayerHurt, IEventHandlerAdminQuery
    {
        /// <summary>
        /// OnPlayerHurt is triggered when a player is injured by another player ingame (does NOT trigger for environmental damage)
        /// </summary>
        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            Plugin.Info($"{ev.Player.name} was injured {ev.DamageType.name}");
        }

        /// <summary>
        /// OnAdminQuery is triggered when a member of staff (global or server) runs a command via RemoteAdmin
        /// </summary>
        public void OnAdminQuery(AdminQueryEvent ev)
        {
            Plugin.Info($"{ev.Admin.name} ran command {ev.Query}");
        }
    }
    #endregion

    #region Command Example
    /// <summary>
    /// Here is an example of how to create custom command with Pheggmod.
    /// All commands MUST have the following 2 attributes. Without them, the command will *NOT* be run.
    /// 
    /// PMCommand - Sets the name (and default trigger) for the command. Without this, the command will not be registed.
    /// PMParameters - Sets the parameters for the given command. If you want no parameters for the command, simply leave empty (like testcommand below)
    /// 
    /// </summary>
    internal class myCommand
    {
        /// <summary>
        /// Simply prints a string to the server console whenever you type "testcommand".
        /// </summary>
        [PMCommand("testcommand"), PMParameters()]
        public void cmd_TestCommand(CommandInfo info)
        {
            Plugin.Info(info.commandSender.Nickname + " ran testcommand");
        }

        /// <summary>
        /// Requires a user to type "nevergonna give you up" in RA. If you provide more or less parameters, it will simply print a help message to the user in RA "nevergonna [give] [you] [up]"
        /// </summary>
        [PMCommand("nevergonna"), PMParameters("give", "you", "up")]
        public void cmd_RickRoll(CommandInfo info)
        {
            info.commandSender.RaReply(info.commandName + $"#Give you up,\nNever gonna let you down.\nNever gonna run around,\nDesert you.", true, true, "");
            Plugin.Info("Never gonna make you cry,\nNever gonna say goodbye.\nNever gonna tell a lie,\nAnd hurt you.");
        }
    }
    #endregion
}
