#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PheggMod.EventTriggers
{
	[MonoModPatch("global::NicknameSync")]
	class PMNicknameSync : NicknameSync
	{
		readonly static string[] filteredwords =
		{
			"niggers", "niggas", "faggots", "trannies"
		};


		public extern void orig_SetNick(string nick);
		public void SetNick(string nick)
		{
			orig_SetNick(nick);

			GameObject go = gameObject;
			CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();

			if (ccm.isLocalPlayer)
				return;

			try
			{
				if (nick != null && !string.IsNullOrEmpty(nick))
				{
					try
					{
						Base.Debug("Triggering PlayerJoinEvent");
						PluginManager.TriggerEvent<IEventHandlerPlayerJoin>(new PlayerJoinEvent(new PheggPlayer(gameObject)));
					}
					catch (Exception e)
					{
						Base.Error($"Error triggering PlayerJoinEvent: {e.InnerException}");
					}
				}

				string NewNick = nick;

				foreach (var MatchedWord in filteredwords)
				{
					Base.Info(MatchedWord);

					if (StringContains(NewNick, MatchedWord, StringComparison.OrdinalIgnoreCase))
					{
						NewNick = Regex.Replace(NewNick, MatchedWord, "duck", RegexOptions.IgnoreCase);
					}
				}

				var newFilter = new List<string>();
				var newFilterWord = string.Empty;

				foreach (var singularWord in filteredwords)
				{
					newFilterWord = singularWord.Replace("ies", "y");
					if (newFilterWord.EndsWith("s"))
						newFilterWord = newFilterWord.Remove(newFilterWord.Length - 1, 1);

					newFilter.Add(newFilterWord);
				}

				foreach (var MatchedWord in newFilter.Where(w => StringContains(NewNick, w, StringComparison.OrdinalIgnoreCase)))
				{
					NewNick = Regex.Replace(NewNick, MatchedWord, "duck", RegexOptions.IgnoreCase);
				}

				var RefHub = ReferenceHub.GetHub(gameObject);

				RefHub.nicknameSync.DisplayName = NewNick;
			}
			catch (Exception e)
			{
				Base.Error($"Error: {e}");
			}
		}

		public static bool StringContains(string source, string toCheck, StringComparison comp)
		{
			return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
		}
	}
}
