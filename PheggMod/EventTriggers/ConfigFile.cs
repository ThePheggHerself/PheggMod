#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::GameCore.ConfigFile")]
    public static class PMConfigFile
    {
        internal static string webhookUrl, webhookName, webhookAvatar, webhookMessage, reportHeader, reportContent, reportServerName;
        internal static int webhookColour, detonationTimer;

        internal static bool announceChaos, cassieGlitch, cassieGlitchDetonation, stickyRound, targetAnnouncement, mockCommand, randomSizes, tutorialTrigger096, enable008;
        internal static string chaosAnnouncement;

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
            mockCommand = ServerConfig.GetBool("enable_mock_command", true);
            targetAnnouncement = ServerConfig.GetBool("notify_096_target", true);
            tutorialTrigger096 = ServerConfig.GetBool("tutorial_triggers_096", false);
            enable008 = ServerConfig.GetBool("scp_008", true);
            detonationTimer = ServerConfig.GetInt("warhead_tminus_start_duration", 90);

            webhookUrl = ServerConfig.GetString("report_discord_webhook_url", string.Empty);
            webhookName = ServerConfig.GetString("report_username", "Player Report");
            reportServerName = ServerConfig.GetString("report_server_name", "My SCP:SL Server");
            reportHeader = ServerConfig.GetString("report_header", "Player Report");
            reportContent = ServerConfig.GetString("report_content", "Player has just been reported.");
            webhookAvatar = ServerConfig.GetString("report_avatar_url", string.Empty);
            webhookMessage = ServerConfig.GetString("report_message_content", string.Empty);
            webhookColour = ServerConfig.GetInt("report_color", 14423100);

            doorCooldown173 = ServerConfig.GetFloat("scp173_door_cooldown", 25f);

            randomSizes = ServerConfig.GetBool("random_sizes", false);

            #region SmartGuard
            enableSmartGuard = ServerConfig.GetBool("smart_guard", true);
            #endregion
        }
    }
}
