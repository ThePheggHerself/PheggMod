using CustomPlayerEffects;
using Mirror;
using PheggMod.EventTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.CustomEffects
{
    public class SCP008 : PlayerEffect, IHealablePlayerEffect
    {
		public SCP008(ReferenceHub hub)
		{
			Hub = hub;
			Slot = ConsumableAndWearableItems.UsableItem.ItemSlot.Unwearable;
			TimeBetweenTicks = 2f;
			TimeLeft = 10f;
		}

		public bool IsHealable(ItemType it) => it == ItemType.SCP500 || it == ItemType.Adrenaline;

		public override void PublicUpdate()
		{
			if (!NetworkServer.active)
				return;
			if (Enabled)
			{
				TimeLeft -= Time.deltaTime;
				if (TimeLeft > 0)
					return;
				TimeLeft += TimeBetweenTicks;

				Hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(2f, "INFECTION", DamageTypes.Poison, 0), Hub.gameObject);
			}
			else
			{
				TimeLeft = TimeBetweenTicks;
			}
		}
	}
}
