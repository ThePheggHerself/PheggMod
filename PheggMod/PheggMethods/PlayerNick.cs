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

    [MonoModPatch("global::NicknameSync")]
    class PMNicknameSync : NicknameSync
    {
        public extern void orig_SetNick(string nick);
        public void SetNick(string nick)
        {
            orig_SetNick(nick);

            PlayerNick.pheggNickSync(new PheggPlayer(this.gameObject));
        }
    }

    class PlayerNick
    {
        public delegate void NickSyncEvent();
        public event NickSyncEvent OnNickSync;

        internal static void pheggNickSync(PheggPlayer player)
        {
            if (string.IsNullOrEmpty(player.userId)) return;
            else BotWorker.NewMessage($"**{player.name} ({player.userId} from ||~~{player.ipAddress}~~||) has joined the server**");
        }
    }
}
