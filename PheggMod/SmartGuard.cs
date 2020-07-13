using GameCore;
using Mirror;
using PheggMod.EventTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod
{
    internal static class SmartGuard
    {
        internal enum InfractionType
        {
            none = 0,
            minor = 1,
            medium = 2,
            major = 3
        }
        private enum Domain
        {
            steam = 0,
            discord,
            northwood
        }

        private static Regex _rgx = new Regex("[^a-zA-Z0-9]");
        private static Dictionary<string, string> leetrules = new Dictionary<string, string>
        {
            {"4", "A"},
            {@"/\", "A"},
            {"@", "A"},
            {"^", "A"},
            {"13", "B"},
            {"/3", "B"},
            {"|3", "B"},
            {"8", "B"},
            {"><", "X"},
            {"<", "C"},
            {"(", "C"},
            {"|)", "D"},
            {"|>", "D"},
            {"3", "E"},
            {"6", "G"},
            {"/-/", "H"},
            {"[-]", "H"},
            {"]-[", "H"},
            {"!", "I"},
            {"1", "I"},
            {"|_", "L"},
            {"_/", "J"},
            {"_|", "J"},
            {"|<", "K" },
            {"0", "O"},
            {"5", "S"},
            {"7", "T"},
            {@"\/\/", "W"},
            {@"\/", "V"},
            { "2", "Z"}
        };


        /// <summary>
        /// A very quick SmatGuard check during the preauth period. Can help pick up blacklisted UserIds or new discord accounts and filter them out before they are able to connect fully
        /// </summary>
        /// <param name="preauthItem"></param>
        /// <param name="flags"></param>
        /// <param name="infractionType"></param>
        internal static void SGPreauthCheck(PreauthItem preauthItem, CentralAuthPreauthFlags flags, out InfractionType infractionType, out string reason)
        {
            infractionType = InfractionType.none;
            reason = null;

            if (flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreBans) || flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreWhitelist) || flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreGeoblock) && PMConfigFile.skipGlobalStaff
                || PMConfigFile.whitelistedUids.Contains(preauthItem.UserId))
                return;

            Domain domain = GetDomain(preauthItem.UserId);
            if (domain == Domain.northwood && PMConfigFile.skipStudioStaff)
                return;

            //UID Check
            if (PMConfigFile.blacklistedUids.Contains(preauthItem.UserId))
            {
                infractionType = InfractionType.medium;
                reason = "UserId has been blocked on this server";
                return;
            }

            if (domain == Domain.discord)
            {
                //Generates the timespan since the discord account was created (Thanks discord and your use of Snowflake IDs <3)
                TimeSpan timeSinceCreation = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(((ulong.Parse(preauthItem.UserId.Split('@')[0]) >> 22) + 1420070400000) / 1000));
                
                if(timeSinceCreation.Minutes < PMConfigFile.accountAgeRequirement)
                {
                    infractionType = InfractionType.medium;
                    reason = "Account is too new";
                    return;
                }
            }
        }


        private static Domain GetDomain(string UserId)
        {
            string[] args = UserId.Split('@');

            switch (args[0].ToLower())
            {
                case "discord":
                    return Domain.discord;
                case "northwood":
                    return Domain.northwood;
                default:
                    return Domain.steam;
            }
        }






        /// <summary>
        /// Is run whenever a user joins the server (instead of pre-authenticates)
        /// </summary>
        /// <param name="gameObject"></param>
        //public void SmartGuardDeepCheck(GameObject go)
        //{
        //    try
        //    {
        //        if (!_enable)
        //            return;

        //        CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();
        //        ServerRoles sr = go.GetComponent<ServerRoles>();
        //        NicknameSync ns = go.GetComponent<NicknameSync>();
        //        CustomLiteNetLib4MirrorTransport cln = go.GetComponent<CustomLiteNetLib4MirrorTransport>();

        //        string domain = ccm.UserId.Split('@')[1].ToLower();

        //        //Whitelist Check
        //        if (sr.BypassStaff && _skStaffGlobal)
        //        {
        //            Base.SmartGuard("User is global staff. Skipping...");
        //            return;
        //        }
        //        else if (sr.RemoteAdmin && _skStaffServer)
        //        {
        //            Base.SmartGuard("User is server staff. Skipping...");
        //            return;
        //        }
        //        else if (_uidWhitelist.Contains(ccm.UserId))
        //        {
        //            Base.SmartGuard("User's UserId is whitelisted. Skipping...");
        //        }
        //        else if (_nameWhitelist.Contains(ns.MyNick))
        //        {
        //            Base.SmartGuard("User's name is whitelisted (not recommended). Skipping...");
        //            return;
        //        }

        //        //Blacklist Check
        //        if (_nameFilterSmart)
        //        {
        //            string antil33t = _rgx.Replace(ns.MyNick.ToLower(), string.Empty);

        //            foreach (KeyValuePair<string, string> pair in leetrules)
        //            {
        //                antil33t = antil33t.Replace(pair.Key, pair.Value);
        //            }

        //            if (_nameBlacklist.Contains(antil33t))
        //            {
        //                if (_banDurationOne == 0)
        //                {
        //                    HandlePunishments(go, "Blacklisted name");
        //                    return;
        //                }
        //                else if (_banDurationOne > 0)
        //                {
        //                    HandlePunishments(go, "Blacklisted name");
        //                    return;
        //                }
        //            }
        //        }
        //        else if (!_nameFilterSmart)
        //        {
        //            if (_nameBlacklist.Contains(ns.MyNick.ToLower()))
        //            {
        //                if (_banDurationOne == 0)
        //                {
        //                    HandlePunishments(go, "Blacklisted name");
        //                    return;
        //                }
        //                else if (_banDurationOne > 0)
        //                {
        //                    HandlePunishments(go, "Blacklisted name");
        //                    return;
        //                }
        //            }
        //        }

        //        if (_uidBlacklist.Contains(ccm.UserId))
        //        {
        //            if (_banDurationOne == 0)
        //            {
        //                HandlePunishments(go, "Blacklisted UID");
        //            }
        //            else if (_banDurationOne > 0)
        //            {
        //                HandlePunishments(go, "Blacklisted UID");
        //            }
        //        }

        //        bool doContinue = true;

        //        if (domain == "discord")
        //            doContinue = CheckDiscord(go);
        //        else if (domain == "steam")
        //            doContinue = CheckSteam(go);

        //        if (!doContinue)
        //            return;
        //    }
        //    catch (Exception e)
        //    {
        //        Base.Error(e.ToString());
        //    }
        //}
        //private bool CheckDiscord(GameObject go)
        //{
        //    string[] uid = go.GetComponent<CharacterClassManager>().UserId.Split('@');
        //    bool success = Int64.TryParse(uid[0], out long snowflake);

        //    if (!success)
        //    {
        //        Base.Error("REEEEE");
        //        return false;
        //    }

        //    DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        //    date = date.AddSeconds(((snowflake >> 22) + 1420070400000) / 1000).ToLocalTime();

        //    new TimeSpan(date.Ticks);


        //    Base.Error($"{uid[0]} - {date.ToString()} - {(DateTime.Now - date).Days}");

        //    return true;
        //}
        //private bool CheckSteam(GameObject go)
        //{
        //    return true;
        //}


        //private void HandlePunishments(GameObject go, string reason)
        //{
        //    if (_banDurationOne < 0)
        //        return;

        //    Base.Disconnect(go, $"You have been automatically disconnected from the server by SmartGuard.\nReason: {reason}");

        //    Base.SmartGuard($"{go.GetComponent<NicknameSync>().MyNick} was {(_banDurationOne > 0 ? "banned" : "kicked")}: {reason}");

        //    if (_banDurationOne > 0)
        //    {
        //        Base.Disconnect(go, $"You have been automatically disconnected from the server by SmartGuard.\nReason: {reason}\nNote: This action was performed automatically by a server modification!");

        //        BanHandler.IssueBan(new BanDetails
        //        {
        //            OriginalName = go.GetComponent<NicknameSync>().MyNick,
        //            Id = go.GetComponent<CharacterClassManager>().UserId,
        //            Expires = DateTime.UtcNow.AddMinutes(_banDurationOne).Ticks,
        //            Reason = $"SMARTGUARD - {reason}",
        //            Issuer = "SMARTGUARD",
        //            IssuanceTime = DateTime.UtcNow.Ticks
        //        }, BanHandler.BanType.UserId);

        //        BanHandler.IssueBan(new BanDetails
        //        {
        //            OriginalName = go.GetComponent<NicknameSync>().MyNick,
        //            Id = go.GetComponent<NetworkIdentity>().connectionToClient.address,
        //            Expires = DateTime.UtcNow.AddMinutes(_banDurationOne).Ticks,
        //            Reason = $"SMARTGUARD - {reason}",
        //            Issuer = "SMARTGUARD",
        //            IssuanceTime = DateTime.UtcNow.Ticks
        //        }, BanHandler.BanType.IP);

        //        Base.SmartGuard($"{go.GetComponent<NicknameSync>().MyNick} was banned for: {reason}");
        //    }
        //}
    }
}
