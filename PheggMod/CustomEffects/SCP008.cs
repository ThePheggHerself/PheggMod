using CustomPlayerEffects;
using Hints;
using Mirror;
using Telepathy;
using UnityEngine;

namespace PheggMod.CustomEffects
{
    public class SCP008 : PlayerEffect, IHealablePlayerEffect, IConsumableItemSensitivePlayerEffect, IDisplayablePlayerEffect
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
		public override void PublicOnIntensityChange(byte prevState, byte newState) => intensity = newState;

		public override void PublicUpdate()
		{
			if (!NetworkServer.active)
				return;
			if (Enabled)
			{
				if (!Hub.characterClassManager.IsHuman())
				{
					ServerDisable();
					return;
				}

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

		public override void PublicOnClassChange(RoleType previousClass, RoleType newClass)
		{
			if (newClass != RoleType.Scp0492)
				return;
			Hub.hints.Show(new TextHint("You have succumbed to your infection!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
			ServerDisable();
		}

		public void OnConsumableItemUse(ItemType item)
		{
			if (IsHealable(item))
			{
				Hub.hints.Show(new TextHint("Your infection has been cured!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
				ServerDisable();
			}
		}

		public bool GetSpectatorText(out string s)
		{
			s = "Infected";
			return true;
		}
	}
}
