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
                try
                {
                    int index = PlayerManager.players.FindIndex(player => player.GetComponent<NicknameSync>().MyNick == issuer);

                    if (index > -1)
                    {
                        if (isGlobalBan)
                            try
                            {
                                Base.Debug("Triggering GlobalBanEvent");
                                PluginManager.TriggerEvent<IEventHandlerGlobalBan>(new GlobalBanEvent(new PheggPlayer(user)));
                            }
                            catch (Exception e)
                            {
                                Base.Error($"Error triggering GlobalBanEvent: {e.ToString()}");
                            }
                        else if (duration < 1)
                            try
                            {
                                Base.Debug("Triggering PlayerKickEvent");
                                PluginManager.TriggerEvent<IEventHandlerPlayerKick>(new PlayerKickEvent(new PheggPlayer(user), new PheggPlayer(PlayerManager.players[index]), reason));
                            }
                            catch (Exception e)
                            {
                                Base.Error($"Error triggering PlayerKickEvent: {e.ToString()}");
                            }
                        else
                            try
                            {
                                Base.Debug("Triggering PlayerBanEvent");
                                PluginManager.TriggerEvent<IEventHandlerPlayerBan>(new PlayerBanEvent(new PheggPlayer(user), duration, new PheggPlayer(PlayerManager.players[index]), reason));
                            }
                            catch (Exception e)
                            {
                                Base.Error($"Error triggering PlayerBanEvent: {e.ToString()}");
                            }
                    }


                }
                catch (Exception) { }
            }

            return result;
        }
    }
}
