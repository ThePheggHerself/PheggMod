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
using MEC;

namespace PheggMod
{
    [MonoModPatch("global::ServerRoles")]
    class PMServerRoles : ServerRoles
    {
        public extern void orig_RefreshPermissions(bool disp = false);
        public new void RefreshPermissions(bool disp = false)
        {
            orig_RefreshPermissions(disp);

            CharacterClassManager player = GetComponent<CharacterClassManager>();

            Timing.RunCoroutine(CustomTags.CustomTag(player));
        }
    }
}
