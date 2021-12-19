#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Dissonance.Extensions;
using MonoMod;
using System;
using System.Linq;

namespace PheggMod.Patches
{
	[MonoModPatch("global::GameCore.ConfigFile")]
	public static class PMConfigFile
	{
		public static bool DebugMode;
		internal static string RestartTime;

		internal static string webhookUrl, webhookName, webhookAvatar, webhookMessage, reportHeader, reportContent, reportServerName;
		internal static int webhookColour, detonationTimer;

		internal static bool announceChaos, cassieGlitch, cassieGlitchDetonation, stickyRound, targetAnnouncement, mockCommand, tutorialTrigger096, enable008;
		internal static string chaosAnnouncement;

		internal static float doorCooldown173;

		public static bool IsConfigLoaded = false;

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

			DebugMode = ServerConfig.GetBool("pheggmod_debug", false);

			///This is the time that the server will check for with the auto-restarting system.
			///Uses 24 hour formatting (16:00 is 4PM), and uses the time relative to the server.
			///Set to 25:00 to disable;
			RestartTime = ServerConfig.GetString("auto_restart_time", "04:30");

			if (DebugMode)
			{
				Base.Debug("Debug mode enabled!");
				Base.Debug("Printing file paths:"
					+ $"\nUserIdBans.txt: {BanHandler.GetPath(BanHandler.BanType.UserId)}"
					+ $"\nIpBans.txt: {BanHandler.GetPath(BanHandler.BanType.IP)}"
					+ $"\nUserIdWhitelist.txt: {GameCore.ConfigSharing.Paths[2] + "UserIDWhitelist.txt"}"
					+ $"\nUserIdReservedSlots.txt: {GameCore.ConfigSharing.Paths[3] + "UserIDReservedSlots.txt"}");


				//FFDetector.FFDetector.DetectorEnabled = ServerConfig.GetBool("pheggmod_friendly_fire_detector_enabled", true);
			}

			if (firstTime && !IsConfigLoaded)
			{
				IsConfigLoaded = true;
			}
		}
	}
}
