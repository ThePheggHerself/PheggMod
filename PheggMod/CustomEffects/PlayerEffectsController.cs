#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
using CustomPlayerEffects;
using Mirror;
using MonoMod;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PheggMod.CustomEffects
{
	[MonoModPatch("global::PlayerEffectsController")]
	class PMPlayerEffectsController : PlayerEffectsController
	{
		private ReferenceHub hub;

		//[MonoModReplace]
		internal SyncList<byte> syncEffectsIntensity = new SyncList<byte>();
		//[MonoModReplace]
		internal PlayerEffect[] _allEffects = new PlayerEffect[] { };

		private extern void orig_Awake();
#pragma warning disable IDE0051 // Remove unused private members
		private void Awake()
		{
			orig_Awake();

			var effectList = new List<PlayerEffect>(_allEffects);

			try
			{
				var SCP008 = effectsGameObject.AddComponent<SCP008>();
				SCP008.name = "SCP008";

				Instantiate(SCP008);

				AllEffects.Add(SCP008.GetType(), SCP008);
				syncEffectsIntensity.Add(0);

				effectList.Add(SCP008);
			}
			catch (Exception e)
			{
				Base.Error(e.ToString());
			}

			_allEffects = effectList.ToArray();
		}
#pragma warning restore IDE0051 // Remove unused private members
		[Server]
		public bool ChangeByString(string type, byte intensity, float duration = 0f)
		{
			foreach (PlayerEffect item in _allEffects)
			{
				try
				{
					if (!item.ToString().StartsWith(type, StringComparison.InvariantCultureIgnoreCase))
						continue;

					item.Intensity = intensity;

					if (duration > 0)
						item.ServerChangeDuration(duration);
				}
				catch (Exception e)
				{
					Base.Error(e.ToString());
					return false;
				}

				return true;
			}

			return false;
		}

	}
}
