using CustomPlayerEffects;
using Hints;
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
		public bool hasNotified = false;
		public int intensity = 1;
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
				if (!hasNotified)
				{
					Hub.hints.Show(new TextHint("You have been infected!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
					hasNotified = true;
				}

				TimeLeft -= Time.deltaTime;
				if (TimeLeft > 0)
					return;
				TimeLeft += TimeBetweenTicks;

				Hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(2f * intensity, "INFECTION", DamageTypes.Poison, 0), Hub.gameObject);
			}
			else
			{
				TimeLeft = TimeBetweenTicks;
				hasNotified = false;
			}
		}

		public override void PublicOnIntensityChange(byte prevState, byte newState)
		{
			if(newState == 0)
				Hub.hints.Show(new TextHint("Your infection has been cured!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
			intensity = newState;
		}
	}
}
