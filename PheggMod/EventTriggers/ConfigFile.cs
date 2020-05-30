#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Commands;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::GameCore.ConfigFile")]
    public static class PMConfigFile
    {
        internal static string webhookUrl;
        internal static string webhookName;
        internal static string webhookAvatar;
        internal static string webhookMessage;
        internal static int webhookColour;

        internal static bool announceChaos;
        internal static string chaosAnnouncement;

        internal static bool cassieGlitch;
        internal static bool cassieGlitchDetonation;

        internal static bool stickyRound;

        internal static bool targetAnnouncement;

        internal static float doorCooldown173;

        #region SmartGuard;
        internal static bool enableSmartGuard;
        #endregion



        public static extern void orig_ReloadGameConfigs(bool firstTime = false);
        public static void ReloadGameConfigs(bool firstTime = false)
        {
            orig_ReloadGameConfigs(firstTime);

            YamlConfig ServerConfig = GameCore.ConfigFile.ServerConfig;

            announceChaos = ServerConfig.GetBool("announce_chaos_spawn", true);
            chaosAnnouncement = ServerConfig.GetString("chaos_announcement", "PITCH_1 ATTENTION ALL PERSONNEL . CHAOS INSURGENCY BREACH IN PROGRESS");
            cassieGlitch = ServerConfig.GetBool("cassie_glitch", false);
            cassieGlitchDetonation = ServerConfig.GetBool("cassie_glitch_post_detonation", false);
            stickyRound = ServerConfig.GetBool("fix_sticky_round", true);

            webhookUrl = ServerConfig.GetString("report_discord_webhook_url", string.Empty);
            webhookName = ServerConfig.GetString("report_username", "Player Report");
            webhookAvatar = ServerConfig.GetString("report_avatar_url", string.Empty);
            webhookMessage = ServerConfig.GetString("report_message_content", string.Empty);
            webhookColour = ServerConfig.GetInt("report_color", 14423100);

            targetAnnouncement = ServerConfig.GetBool("notify_096_target", true);

            doorCooldown173 = ServerConfig.GetFloat("scp173_door_cooldown", 25f);

            #region SmartGuard
            enableSmartGuard = ServerConfig.GetBool("smart_guard", true);
            #endregion


        }
    }
}
