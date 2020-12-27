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
using CloudflareSolverRe;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::CheaterReport")]
    class PMCheaterReport : CheaterReport
    {
        [MonoModReplace]
        internal static bool SubmitReport(string reporterUserId, string reportedUserId, string reason, ref int reportedId, string reporterNickname, string reportedNickname, bool friendlyFire)
        {
            try
            {
                string json = JsonSerializer.ToJsonString(new DiscordWebhook($"{ PMConfigFile.webhookMessage }", PMConfigFile.webhookName, PMConfigFile.webhookAvatar, false, new[] { new DiscordEmbed(
                PMConfigFile.reportHeader, "rich", PMConfigFile.reportContent, PMConfigFile.webhookColour, new[]
                {
                    new DiscordEmbedField("Server Name", PMConfigFile.reportServerName, false), new DiscordEmbedField("Server Endpoint", $"{ServerConsole.Ip}:{ServerConsole.Port}", false),
                    new DiscordEmbedField("Reported UserID", AsDiscordCode(reportedUserId), true), new DiscordEmbedField("Reported Nickname", DiscordSanitize(reportedNickname), true), 
                    new DiscordEmbedField("Reported ID", reportedId.ToString(), true), new DiscordEmbedField("Reporter UserID", AsDiscordCode(reporterUserId), true),
                    new DiscordEmbedField("Reporter Nickname", DiscordSanitize(reporterNickname), true), new DiscordEmbedField("Reason", DiscordSanitize(reason), true),
                    new DiscordEmbedField("Timestamp", TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm zzz"), false), new DiscordEmbedField("UTC Timestamp", TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm", DateTimeOffset.UtcNow), false)
                })}));


                HttpClient _client;

                _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("User-Agent", "SCP SL");
                _client.Timeout = TimeSpan.FromSeconds(20.0);

                _client.PostAsync(PMConfigFile.webhookUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                return true;
            }
            catch (Exception e)
            {
                ServerConsole.AddLog("Failed to send report by webhook: " + e.Message);
                Base.Error(e.ToString());

                return false;
            }
        }

        [MonoModReplace]
        private static string AsDiscordCode(string text) => $"`{text.Replace("`", "'")}`";
        [MonoModReplace]
        private static string DiscordSanitize(string text) => text.Replace("<", "(").Replace(">", ")").Replace("@", "@ ").Replace("`", "'");
    }
}
