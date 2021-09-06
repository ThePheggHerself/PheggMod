#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Linq;
using MonoMod;
using UnityEngine;
using PheggMod.API.Events;

namespace PheggMod.Patches.Config
{
	[MonoModPatch("global::BanHandler")]
	class PMBanHandler
	{
		public extern static bool orig_IssueBan(BanDetails ban, BanHandler.BanType banType);
		public static bool IssueBan(BanDetails ban, BanHandler.BanType banType)
		{
			var a = ReferenceHub.GetAllHubs().Where(r => r.Value.networkIdentity.connectionToClient.address == ban.Id);

			if (a.Any())
			{
				Base.Info(a.First().Value.characterClassManager.Asn);
				if (a.First().Value.characterClassManager.Asn == "50889")
				{
					Base.Info($"IP ban of user {ban.OriginalName} ({a.First().Value.characterClassManager.UserId}) has been blocked due to IP address belonging to ASN {a.First().Value.characterClassManager.Asn}");
					return false;
				}
			}

			return orig_IssueBan(ban, banType);
		}

	}
}
