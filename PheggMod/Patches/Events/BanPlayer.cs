#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Linq;
using MonoMod;
using UnityEngine;
using PheggMod.API.Events;

namespace PheggMod.Patches
{
    [MonoModPatch("global::BanPlayer")]
    class PMBanPlayerr : BanPlayer
    {
        public extern bool orig_BanUser(GameObject user, long duration, string reason, string issuer, bool isGlobalBan);
        public new bool BanUser(GameObject user, long duration, string reason, string issuer, bool isGlobalBan)
        {
			bool result = orig_BanUser(user, duration, reason, issuer, isGlobalBan);

            if (result)
            {
				try
                {
					var hubs = ReferenceHub.GetAllHubs().ToList();
                    int index = hubs.FindIndex(plr => issuer == $"{plr.Value.nicknameSync.MyNick} ({plr.Value.characterClassManager.UserId})");

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
                                Base.Error($"Error triggering GlobalBanEvent: {e.InnerException}");
                            }
                        else if (duration < 1)
                            try
                            {
								Base.Debug("Triggering PlayerKickEvent");
                                PluginManager.TriggerEvent<IEventHandlerPlayerKick>(new PlayerKickEvent(new PheggPlayer(user), new PheggPlayer(hubs[index].Key), reason));
                            }
                            catch (Exception e)
                            {
                                Base.Error($"Error triggering PlayerKickEvent: {e.InnerException}");
                            }
                        else
                            try
                            {
								Base.Debug("Triggering PlayerBanEvent");
                                PluginManager.TriggerEvent<IEventHandlerPlayerBan>(new PlayerBanEvent(new PheggPlayer(user), duration, new PheggPlayer(hubs[index].Key), reason));
                            }
                            catch (Exception e)
                            {
                                Base.Error($"Error triggering PlayerBanEvent: {e.InnerException}");
                            }
                    }


                }
                catch (Exception e) { Base.Error(e.ToString()); }
            }

            return result;
        }
    }
}
