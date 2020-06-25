#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
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
        private HashSet<int> reportedPlayers;
        private string _serverAddress;

        [MonoModReplace]
        public void LogReport(GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, ref string reason, int reportedId, bool notifyGm)
        {
            if (string.IsNullOrEmpty(_serverAddress) && !_hasLoaded)
            {
                reportedPlayers = typeof(CheaterReport).GetField("reportedPlayers",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(PlayerManager.localPlayer.GetComponent<CheaterReport>()) as HashSet<int> ?? new HashSet<int>();

                _serverAddress = $"{ServerConsole.Ip}:{ServerConsole.Port}";

                _hasLoaded = true;
            }

            try
            {
                GameObject reporterGO = PlayerManager.players.Find(p => p.GetComponent<CharacterClassManager>().UserId == reporterUserId);
                GameObject reportedGO = PlayerManager.players.Find(p => p.GetComponent<CharacterClassManager>().UserId == reportedUserId);

                try
                {
                    Base.Debug("Triggering PlayerReportEvent");
                    PluginManager.TriggerEvent<IEventHandlerPlayerReport>(new PlayerReportEvent(new PheggPlayer(reporterGO), new PheggPlayer(reportedGO), reason));
                }
                catch (Exception e)
                {
                    Base.Error($"Error triggering PlayerReportEvent {e.InnerException}");
                }

                if (!notifyGm)
                {
                    reportedPlayers.Add(reportedId);
                    reporter.SendToClient(base.connectionToClient, "[REPORTING] Player report successfully sent to local administrators by webhooks.", "green");
                }

                if (reportedGO != null && reporterGO != null)
                {
                    string json = JsonSerializer.ToJsonString(new DiscordWebhook($"{PMConfigFile.webhookMessage}", PMConfigFile.webhookName, PMConfigFile.webhookAvatar, tts: false, new DiscordEmbed[1]
                    {
                            new DiscordEmbed("Ingame player report", "rich", "Player has just been reported.", PMConfigFile.webhookColour, new DiscordEmbedField[5]
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

                    _client.PostAsync(PMConfigFile.webhookUrl, new StringContent(json, Encoding.UTF8, "application/json"));
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
