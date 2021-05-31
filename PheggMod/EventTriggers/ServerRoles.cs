#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using Mirror;
using System.Net;
using GameCore;

using PheggMod.API.Events;
namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::ServerRoles")]
    class PMServerRoles : ServerRoles
    {
		internal UserGroup Group;

		public UserGroup UserGroup
		{
			get => Group;
		}

        public extern void orig_RefreshPermissions(bool disp = false);
        public new void RefreshPermissions(bool disp = false)
        {
            orig_RefreshPermissions(disp);

            try
            {
                Base.Debug("Triggering RefreshAdminPermsEvent");
                PluginManager.TriggerEvent<IEventHandlerRefreshAdminPerms>(new RefreshAdminPermsEvent(new PheggPlayer(gameObject)));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering RefreshAdminPermsEvent: {e.InnerException}");
            }
        }

		public extern void orig_TargetOpenRemoteAdmin(NetworkConnection connection, bool password);
		[MonoModPublic]
		public void TargetOpenRemoteAdmin(NetworkConnection connection, bool password) => orig_TargetOpenRemoteAdmin(connection, password);
	}
}
