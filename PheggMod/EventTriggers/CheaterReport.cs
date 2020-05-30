#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using GameCore;
using MonoMod;
using Utf8Json;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Net.Http;
using System.Text;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::CheaterReport")]
    class PMCheaterReport : CheaterReport
    {
        private bool _hasLoaded = false;
        private string _webhookUrl;
        private string _webhookName;
        private string _webhookAvatar;
        private string _webhookMessage;
        private int _webhookColour;
        private HashSet<int> reportedPlayers;
        private string _serverAddress;

        private extern void orig_LogReport(GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, ref string reason, int reportedId, bool notifyGm);
        public void LogReport(GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, ref string reason, int reportedId, bool notifyGm)
        {
            if (string.IsNullOrEmpty(_webhookUrl) && !_hasLoaded)
            {
                YamlConfig ServerConfig = ConfigFile.ServerConfig;

                _webhookUrl = ServerConfig.GetString("report_discord_webhook_url", string.Empty);
                _webhookName = ServerConfig.GetString("report_username", "Player Report");
                _webhookAvatar = ServerConfig.GetString("report_avatar_url", string.Empty);
                _webhookMessage = ServerConfig.GetString("report_message_content", string.Empty);
                _webhookColour = ServerConfig.GetInt("report_color", 14423100);

                reportedPlayers = typeof(CheaterReport).GetField("reportedPlayers",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(PlayerManager.localPlayer.GetComponent<CheaterReport>()) as HashSet<int> ?? new HashSet<int>();

                _serverAddress = $"{ServerConsole.Ip}:{ServerConsole.Port}";

                _hasLoaded = true;
            }

            try
            {
                GameObject reporterGO = PlayerManager.players.Find(p => p.GetComponent<CharacterClassManager>().UserId == reporterUserId);
                GameObject reportedGO = PlayerManager.players.Find(p => p.GetComponent<CharacterClassManager>().UserId == reportedUserId);

                if (reportedGO != null && reporterGO != null)
                {
                    string json = JsonSerializer.ToJsonString(new DiscordWebhook($"{_webhookMessage}", _webhookName, _webhookAvatar, tts: false, new DiscordEmbed[1]
                    {
                            new DiscordEmbed("Ingame player report", "rich", "Player has just been reported.", _webhookColour, new DiscordEmbedField[5]
                            {
                                new DiscordEmbedField("Reported User", $"{reportedGO.GetComponent<NicknameSync>().MyNick} ({reportedUserId})", inline: false),
                                new DiscordEmbedField("Reporter", $"{reporterGO.GetComponent<NicknameSync>().MyNick} ({reporterUserId})", inline: false),
                                new DiscordEmbedField("Reason", reason, inline: false),
                                new DiscordEmbedField("Server", _serverAddress, inline: false),
                                new DiscordEmbedField("Reported ID", reportedId.ToString(), inline: false)
                            })
                    }));

                    HttpClient _client;

                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("User-Agent", "SCP SL");
                    _client.DefaultRequestHeaders.Add("Game-Version", CustomNetworkManager.CompatibleVersions[0]);
                    _client.Timeout = TimeSpan.FromSeconds(20.0);

                    _client.PostAsync(_webhookUrl, new StringContent(json, Encoding.UTF8, "application/json"));
                    if (!notifyGm)
                    {
                        reportedPlayers.Add(reportedId);
                        reporter.SendToClient(base.connectionToClient, "[REPORTING] Player report successfully sent to local administrators by webhooks.", "green");
                    }
                }
            }
            catch (Exception ex)
            {
                ServerConsole.AddLog("Failed to send report by webhook: " + ex.Message);
                Debug.LogException(ex);
                reporter.SendToClient(base.connectionToClient, "[REPORTING] Failed to send report to local administrators by webhooks.", "red");
            }
        }
    }
}
