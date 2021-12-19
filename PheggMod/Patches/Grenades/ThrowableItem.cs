#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Events;
using Respawning;
using System;
using System.Diagnostics;
using UnityEngine;
using InventorySystem;
using System.Linq;

namespace PheggMod.Patches
{
	[MonoModPatch("global::InventorySystem.Items.ThrowableProjectiles.ThrowableItem")]
	public class ThrowableItem : InventorySystem.Items.ThrowableProjectiles.ThrowableItem
	{
		public extern void orig_ServerThrow(float forceAmount, float upwardFactor, Vector3 torque, Vector3 startVel);
		public void ServerThrow(float forceAmount, float upwardFactor, Vector3 torque, Vector3 startVel)
		{
			orig_ServerThrow(forceAmount, upwardFactor, torque, startVel);

			try
			{
				Base.Debug("Triggering PlayerThrowGrenadeEvent");
				PluginManager.TriggerEvent<IEventHandlerPlayerThrowGrenade>(new PlayerThrowGrenadeEvent(new PheggPlayer(Owner.gameObject), this.Projectile));
			}
			catch (Exception e)
			{
				Base.Error($"Error triggering PlayerThrowGrenadeEvent: {e.ToString()}");
			}
		}
	}
}
