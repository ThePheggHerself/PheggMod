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

namespace PheggMod
{
    [MonoModPatch("global::RemoteAdmin.QueryProcessor")]
    class PMQueryProcessor : RemoteAdmin.QueryProcessor
    {
        public extern void orig_OnDestroy();
        public void OnDestroy()
        {
            orig_OnDestroy();

            CharacterClassManager player = this.GetComponent<CharacterClassManager>();
            NicknameSync name = this.GetComponent<NicknameSync>();

            if (string.IsNullOrEmpty(player.UserId) || !player.UserId.Contains('@')) return;
            else BotWorker.NewMessage($"{name.MyNick} ({player.UserId}) disconnected from the server");
        }
    }
}
