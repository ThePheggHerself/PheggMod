using CustomPlayerEffects;
using Hints;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace PheggMod.CustomEffects
{
	public class SCP008 : PlayerEffect, IDisplayablePlayerEffect, IHealablePlayerEffect
	{
		public bool hasNotified = false;
		public bool HasUsedPainkiller = false;

		protected override void Enabled()
		{
			hasNotified = false;
			HasUsedPainkiller = false;

			TimeBetweenTicks = 10;
			TimeLeft = TimeBetweenTicks;
			IsEnabled = true;
		}

		protected override void Disabled()
		{
			hasNotified = false;
			HasUsedPainkiller = false;
			IsEnabled = false;
			Intensity = 0;
		}

		protected override void OnUpdate()
		{
			if (!NetworkServer.active)
				return;

			if (!IsEnabled || !Hub.characterClassManager.IsHuman() || Intensity < 0)
				this.Disabled();

			if (!hasNotified)
			{
				Hub.hints.Show(new TextHint("You have been infected!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
				hasNotified = true;
			}

			TimeLeft -= Time.deltaTime;
			if (TimeLeft > 0)
				return;

			TimeLeft += TimeBetweenTicks;

			Hub.playerStats.DealDamage(new UniversalDamageHandler(2f * Intensity, DeathTranslations.Poisoned));
							
		}

		public bool IsHealable(ItemType it)
		{
			if (!NetworkServer.active || !IsEnabled)
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
				DecreaseIntensity(Intensity - 1);

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
					DecreaseIntensity(Mathf.Clamp(Intensity - 1, 1, 20));
					return false;
				}
			}
			else
			{
				NoEffect("This item");
				return false;
			}
		}
		public override void OnClassChanged(RoleType previousClass, RoleType newClass)
		{
			if (newClass == RoleType.Spectator)
			{
				Hub.characterClassManager.NetworkCurClass = RoleType.Scp0492;

				Intensity = 0;
				return;
			}

			if (previousClass == RoleType.Spectator || newClass != RoleType.Scp0492)
				return;

			Hub.hints.Show(new TextHint("You have succumbed to your infection!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
			Intensity = 0;
		}

		public void CureInfection()
		{
			Hub.hints.Show(new TextHint("Your infection has been cured!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
			Intensity = 0;
			Disabled();
		}
		public void DecreaseIntensity(int Level)
		{
			Intensity = (byte)Level;

			if (Intensity < 1)
				CureInfection();
			else
				Hub.hints.Show(new TextHint("Your infection's intensity has decreased!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
		}
		public void NoEffect(string item)
		{
			Hub.hints.Show(new TextHint($"{item} had no effect on your infection!", new HintParameter[] { new StringHintParameter("") }, HintEffectPresets.FadeInAndOut(1, 2.5f, 1.5f), 5));
		}
		public bool GetSpectatorText(out string s)
		{
			s = "Infected";
			return true;
		}

		protected override void IntensityChanged(byte prevState, byte newState)
		{
			Intensity = newState;
		}
	}
}
