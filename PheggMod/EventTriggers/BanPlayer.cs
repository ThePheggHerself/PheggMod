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
    [MonoModPatch("global::BanPlayer")]
    class PMBanPlayerr : BanPlayer
    {
        public extern bool orig_BanUser(GameObject user, int duration, string reason, string issuer, bool isGlobalBan);

        public new bool BanUser(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
        {

            bool result = orig_BanUser(user, duration, reason, issuer, isGlobalBan);

            if (result)
            {
                GameObject Admin = PlayerManager.players.Where(player => player.GetComponent<NicknameSync>().MyNick == issuer).FirstOrDefault();

                if (isGlobalBan)
                    PluginManager.TriggerEvent<IEventHandlerGlobalBan>(new GlobalBanEvent(new PheggPlayer(user)));
                else if (duration < 1)
                    PluginManager.TriggerEvent<IEventHandlerPlayerKick>(new PlayerKickEvent(new PheggPlayer(user), new PheggPlayer(Admin), reason));
                else
                    PluginManager.TriggerEvent<IEventHandlerPlayerBan>(new PlayerBanEvent(new PheggPlayer(user), duration, new PheggPlayer(Admin), reason));
            }

            return result;
        }
    }
}
