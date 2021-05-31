using CustomPlayerEffects;
using Hints;
using Mirror;
using Telepathy;
using UnityEngine;

namespace PheggMod.CustomEffects
{
	public class SCP008 : PlayerEffect, IDisplayablePlayerEffect, IHealablePlayerEffect
	{
		public bool hasNotified = false;
		public int intensity = 1;

		public bool HasUsedPainkiller = false;

		public SCP008(ReferenceHub hub)
		{
			Hub = hub;
			Slot = ConsumableAndWearableItems.UsableItem.ItemSlot.Unwearable;
			TimeBetweenTicks = 4f;
			TimeLeft = 20f;
			HasUsedPainkiller = false;
		}

		public bool IsHealable(ItemType it)
		{
			if (!NetworkServer.active || !Enabled)
				return false;

			if (it == ItemType.SCP500 || it == ItemType.Adrenaline)
			{
				CureInfection();
				return true;
			}
			else if (it == ItemType.Painkillers)
			{
				if (HasUsedPainkiller || new System.Random().Next(0, 100) > 40)
				{
					NoEffect("Painkillers");
					return false;
				}

				HasUsedPainkiller = true;
				DecreaseIntensity(intensity - 1);

				return false;
			}
			else if (it == ItemType.Medkit)
			{
				int chance = new System.Random().Next(0, 100);

				if (chance > 50)
				{
					NoEffect("Medkit");
					return false;
				}

				if (chance % 2 == 1)
				{
					CureInfection();
					return true;
				}
				else
				{
					DecreaseIntensity(Mathf.Clamp(intensity - 1, 1, 20));
					return false;
				}
			}
			else
			{
				NoEffect("This item");
				return false;
			}
		}
		public override void PublicOnIntensityChange(byte prevState, byte newState) => intensity = newState;

		public override void PublicUpdate()
		{
			if (!NetworkServer.active)
				return;
			if (Enabled)
			{
				if (!Hub.characterClassManager.IsHuman() || intensity < 1)
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
			if (newClass == RoleType.Spectator)
			{
				ServerDisable();
				return;
			}

			if (previousClass == RoleType.Spectator || newClass != RoleType.Scp0492)
				return;

			Hub.hints.Show(new TextHint("You have succumbed to your infection!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
			ServerDisable();
		}

		public void CureInfection()
		{
			Hub.hints.Show(new TextHint("Your infection has been cured!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
			ServerDisable();
		}
		public void DecreaseIntensity(int Level)
		{
			PublicOnIntensityChange((byte)intensity, (byte)Level);

			if (intensity < 1)
				CureInfection();
			else
				Hub.hints.Show(new TextHint("Your infection's intensity has decreased!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
		}
		public void NoEffect(string item)
		{
			Hub.hints.Show(new TextHint($"{item} had no effect on your infection!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
		}

		//public bool GetSpectatorText(out string s)
		public bool GetSpectatorText(out string s)
		{
			s = "Infected";
			return true;
		}
	}
}
