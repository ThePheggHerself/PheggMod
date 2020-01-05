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
using MEC;

namespace PheggMod
{
    [MonoModPatch("global::CharacterClassManager")]
    class PMCharacterClassManager : CharacterClassManager
    {
        internal ServerRoles SrvRoles { get; private set; }

        public extern System.Collections.IEnumerator orig_Init();
        public System.Collections.IEnumerator Init()
        {
            base.StartCoroutine(orig_Init());

            if (isLocalPlayer && NetworkServer.active && ServerStatic.IsDedicated)
            {
                BotWorker.NewMessage($"**Waiting for players...**");

                Timing.RunCoroutine(CustomTags.HandleTagUpdate());
            }

            yield return 1f;
        }

        public extern static bool orig_ForceRoundStart();

        public new static bool ForceRoundStart()
        {
            orig_ForceRoundStart();

            BotWorker.NewMessage($"**A new round has begun!**");
            return true;
        }

        public extern void orig_ApplyProperties(bool lite = false, bool escape = false);
        public new void ApplyProperties(bool lite = false, bool escape = false)
        {
            orig_ApplyProperties(lite, escape);

            if (isLocalPlayer || (int)this.CurClass == 2) return;

            NicknameSync nick = this.GetComponent<NicknameSync>();

            if (!escape) BotWorker.NewMessage($"{nick.MyNick} spawned as {this.Classes.SafeGet(this.CurClass).fullName}");
            else BotWorker.NewMessage($"{nick.MyNick} escaped and became {this.Classes.SafeGet(this.CurClass).fullName}");
        }

        public extern void orig_CallCmdRequestShowTag(bool global);
        public new void CallCmdRequestShowTag(bool global)
        {
            orig_CallCmdRequestShowTag(global);

            if (!global) return;

            //Timing.RunCoroutine(CustomTags.CustomTag(this));
        }
    }
}
