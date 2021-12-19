#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using Dissonance.Extensions;
using Mirror;
using MonoMod;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PheggMod.Patches
{
	[MonoModPatch("global::ServerConfigSynchronizer")]
	public class PMServerConfigSynchronizer : ServerConfigSynchronizer 
	{
		[Server]
		private void RefreshRAConfigs()
		{
			EnableRemoteAdminPredefinedBanTemplates = PMServerStatic.RolesConfig.GetBool("enable_predefined_ban_templates", true);

			RemoteAdminExternalPlayerLookupMode = PMServerStatic.RolesConfig.GetString("external_player_lookup_mode", "disabled");
			if (RemoteAdminExternalPlayerLookupMode.Equals("disabled", StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(RemoteAdminExternalPlayerLookupMode))
				RemoteAdminExternalPlayerLookupMode = PMServerStatic.SharedGroupsConfig.GetString("external_player_lookup_mode", "disabled");

			RemoteAdminExternalPlayerLookupURL = PMServerStatic.RolesConfig.GetString("external_player_lookup_url");
			if (string.IsNullOrEmpty(RemoteAdminExternalPlayerLookupURL))
				RemoteAdminExternalPlayerLookupURL = PMServerStatic.SharedGroupsConfig.GetString("external_player_lookup_url");
			

			RemoteAdminExternalPlayerLookupToken = PMServerStatic.RolesConfig.GetString("external_player_lookup_token");
			if (string.IsNullOrEmpty(RemoteAdminExternalPlayerLookupToken))
				RemoteAdminExternalPlayerLookupToken = PMServerStatic.SharedGroupsConfig.GetString("external_player_lookup_token");

			if (!EnableRemoteAdminPredefinedBanTemplates)
				return;

			List<string> banReasons = new List<string>();
			banReasons.AddRange(PMServerStatic.RolesConfig.GetStringList("PredefinedBanTemplates"));
			banReasons.AddRange(PMServerStatic.SharedGroupsConfig.GetStringList("PredefinedBanTemplates"));

			if (banReasons != null)
			{
				foreach (string template in banReasons)
				{
					string[] data = YamlConfig.ParseCommaSeparatedString(template);

					if (data.Length != 2)
					{
						ServerConsole.AddLog($"Invalid ban template in RA Config file! Template: {template}");
						continue;
					}

					if (!int.TryParse(data[0], out int timeInSeconds) || timeInSeconds < 0)
					{
						ServerConsole.AddLog($"Invalid ban template in RA Config file - duration must be a non-negative integer. Ban template name: {template}");
						continue;
					}

					PredefinedBanTemplate newTemplate;
					newTemplate.Reason = data[1];

					// Nice duration display for RA.
					TimeSpan ts = TimeSpan.FromSeconds(timeInSeconds);
					newTemplate.Duration = (int)ts.TotalMinutes;

					int years = ts.Days / 365;
					if (years > 0)
						newTemplate.DurationNice = $"{years}y";
					else if (ts.Days > 0)
						newTemplate.DurationNice = $"{ts.Days}d";
					else if (ts.Hours > 0)
						newTemplate.DurationNice = $"{ts.Hours}h";
					else if (ts.Minutes > 0)
						newTemplate.DurationNice = $"{ts.Minutes}m";
					else
						newTemplate.DurationNice = $"{ts.Seconds}s";

					RemoteAdminPredefinedBanTemplates.Add(newTemplate);
				}

				if (RemoteAdminPredefinedBanTemplates.Count == 0)
					EnableRemoteAdminPredefinedBanTemplates = false;
			}
			else
				EnableRemoteAdminPredefinedBanTemplates = false;
		}
	}
}
