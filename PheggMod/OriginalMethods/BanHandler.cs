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
using System.Text.RegularExpressions;

namespace PheggMod
{
    [MonoModPatch("global::BanPlayer")]
    class PMBanPlayerr : BanPlayer
    {
        public extern bool orig_BanUser(GameObject user, int duration, string reason, string issuer, bool isGlobalBan);

        public new bool BanUser(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
        {
            bool result = orig_BanUser(user, duration, reason, issuer, isGlobalBan);

            if (result)
            {
                string adminUserID = PlayerManager.players.Where(player => player.GetComponent<NicknameSync>().MyNick == issuer).FirstOrDefault().GetComponent<CharacterClassManager>().UserId;

                if (isGlobalBan)
                    BotWorker.NewMessage($"**__{user.GetComponent<NicknameSync>().MyNick} ({user.GetComponent<CharacterClassManager>().UserId}) was globally banned for cheating!__**");
                else
                    BotWorker.NewMessage((duration > 0 ? "**Player Banned!**" : "**Player Kicked!**")
                        + $"```autohotkey"
                        + $"\nUser: {user.GetComponent<NicknameSync>().MyNick} ({user.GetComponent<CharacterClassManager>().UserId})"
                        + $"\nAdmin: {issuer} ({adminUserID})"
                        + (duration > 0 ? $"\nDuration: {duration} minutes" : "") + "```");
            }

            return result;
        }
    }
}
