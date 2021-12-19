#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PheggMod.Patches
{
	[MonoModPatch("global::NicknameSync")]
	class PMNicknameSync : NicknameSync
	{
		readonly static string[] filteredwords =
		{
			"niggers", "niggas", "faggots", "trannies", "hitlers", "nazis"
		};

		readonly static string[] animals =
		{
			"duck", "frog", "bear", "lion", "unicorn", "boar", "kangaroo", "elephant", "goose", "turkey", "platypus", "pig", "cow", "horse"
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

				if (AntiIdName(nick))
					return;

				NicknameFilter(nick);
			}
			catch (Exception e)
			{
				Base.Error($"Error: {e}");
			}
		}

		private bool AntiIdName(string Nick)
		{
			var hub = ReferenceHub.GetHub(gameObject);

			var UserId = hub.characterClassManager.UserId.Split('@');

			if (UserId[1].ToLowerInvariant() == "northwood")
				return false;

			if(Nick.ToLowerInvariant() == UserId[0].ToLowerInvariant())
			{
				ServerConsole.Disconnect(hub.gameObject, "You must have a nickname to play on this server");
				return true;
			}
			return false;
		}

		private void NicknameFilter(string nick)
		{
			string NewNick = nick;
			foreach (var MatchedWord in filteredwords)
			{
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
				NewNick = Regex.Replace(NewNick, MatchedWord, animals[new System.Random().Next(0, animals.Length - 1)], RegexOptions.IgnoreCase);
			}

			if (!string.Equals(nick, NewNick, StringComparison.OrdinalIgnoreCase))
			{
				var RefHub = ReferenceHub.GetHub(gameObject);

				RefHub.nicknameSync.DisplayName = NewNick;
			}
		}

		public static bool StringContains(string source, string toCheck, StringComparison comp)
		{
			return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
		}
	}
}
