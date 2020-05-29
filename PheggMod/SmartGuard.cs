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
    class SmartGuard
    {
        public static SmartGuard instance;

        private bool _enable;
        private bool _skStaffServer;
        private bool _skStaffGlobal;
        //private bool _respectDnt;
        private bool _nameFilterSmart;


        private int _ageRequirement;

        private List<string> _nameWhitelist;
        private List<string> _nameBlacklist;

        private List<string> _uidWhitelist;
        private List<string> _uidBlacklist;

        private int _banDurationOne;
        private int _banDurationTwo;
        private int _banDurationThree;

        private string _apiKey;
        private bool _defaultProfile;
        private bool _profNotSet;

        private Regex _rgx = new Regex("[^a-zA-Z0-9]");

        Dictionary<string, string> leetrules = new Dictionary<string, string>
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

        public SmartGuard()
        {
            ///All punishments will be rated on a scale of 1-4
            ///These different levels will use seperate punishment configs
            ///Level 1 violations include not set up steam profiles, and accounts under the age limit
            ///Level 2 violations include blacklisted names and blacklisted userids
            ///Level 3 violations include blacklisted ASNs


            _enable = ConfigFile.ServerConfig.GetBool("sg_enabled", true);
            _skStaffServer = ConfigFile.ServerConfig.GetBool("sg_skip_staff_server", true);
            _skStaffGlobal = ConfigFile.ServerConfig.GetBool("sg_skip_staff_global", true);
            _nameFilterSmart = ConfigFile.ServerConfig.GetBool("sg_smart_filter", true);

            _nameWhitelist = ConfigFile.ServerConfig.GetStringList("sg_whitelist_names"); //
            _nameBlacklist = ConfigFile.ServerConfig.GetStringList("sg_blacklist_names");
            _nameBlacklist.AddRange(new List<string> { "kite1101", "beefteef420", "saltcollector" });

            _uidWhitelist = ConfigFile.ServerConfig.GetStringList("sg_whitelist_uids");
            _uidBlacklist = ConfigFile.ServerConfig.GetStringList("sg_whitelist_uids");

            _banDurationOne = ConfigFile.ServerConfig.GetInt("sg_lvl1_duration", 15); //15 minutes
            _banDurationTwo = ConfigFile.ServerConfig.GetInt("sg_lvl2_duration", 4320); //3 days
            _banDurationThree = ConfigFile.ServerConfig.GetInt("sg_lvl3_duration", 20160); //2 weeks

            _ageRequirement = ConfigFile.ServerConfig.GetInt("account_age_requirement", 10080);

            _apiKey = ConfigFile.ServerConfig.GetString("steam_api_key", null);
            _defaultProfile = ConfigFile.ServerConfig.GetBool("sg_steam_pdefaultprofile", false);
            _profNotSet = ConfigFile.ServerConfig.GetBool("sg_steam_pprofilenotset", true);

            instance = this;
        }

        /// <summary>
        /// Is run whenever a user joins the server (instead of pre-authenticates)
        /// </summary>
        /// <param name="gameObject"></param>
        public void SmartGuardDeepCheck(GameObject go)
        {
            try
            {
                if (!_enable)
                    return;

                CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();
                ServerRoles sr = go.GetComponent<ServerRoles>();
                NicknameSync ns = go.GetComponent<NicknameSync>();
                CustomLiteNetLib4MirrorTransport cln = go.GetComponent<CustomLiteNetLib4MirrorTransport>();

                string domain = ccm.UserId.Split('@')[1].ToLower();

                //Whitelist Check
                if (sr.BypassStaff && _skStaffGlobal)
                {
                    Base.SmartGuard("User is global staff. Skipping...");
                    return;
                }
                else if (sr.RemoteAdmin && _skStaffServer)
                {
                    Base.SmartGuard("User is server staff. Skipping...");
                    return;
                }
                else if (_uidWhitelist.Contains(ccm.UserId))
                {
                    Base.SmartGuard("User's UserId is whitelisted. Skipping...");
                }
                else if (_nameWhitelist.Contains(ns.MyNick))
                {
                    Base.SmartGuard("User's name is whitelisted (not recommended). Skipping...");
                    return;
                }

                //Blacklist Check
                if (_nameFilterSmart)
                {
                    string antil33t = _rgx.Replace(ns.MyNick.ToLower(), string.Empty);

                    foreach (KeyValuePair<string, string> pair in leetrules)
                    {
                        antil33t = antil33t.Replace(pair.Key, pair.Value);
                    }

                    if (_nameBlacklist.Contains(antil33t))
                    {
                        if (_banDurationOne == 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                        else if (_banDurationOne > 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                    }
                }
                else if (!_nameFilterSmart)
                {
                    if (_nameBlacklist.Contains(ns.MyNick.ToLower()))
                    {
                        if (_banDurationOne == 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                        else if (_banDurationOne > 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                    }
                }

                if (_uidBlacklist.Contains(ccm.UserId))
                {
                    if (_banDurationOne == 0)
                    {
                        HandlePunishments(go, "Blacklisted UID");
                    }
                    else if (_banDurationOne > 0)
                    {
                        HandlePunishments(go, "Blacklisted UID");
                    }
                }

                bool doContinue = true;

                if (domain == "discord")
                    doContinue = CheckDiscord(go);
                else if (domain == "steam")
                    doContinue = CheckSteam(go);

                if (!doContinue)
                    return;
            }
            catch (Exception e)
            {
                Base.Error(e.ToString());
            }
        }
        private bool CheckDiscord(GameObject go)
        {
            string[] uid = go.GetComponent<CharacterClassManager>().UserId.Split('@');
            bool success = Int64.TryParse(uid[0], out long snowflake);

            if (!success)
            {
                Base.Error("REEEEE");
                return false;
            }

            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            date = date.AddSeconds(((snowflake >> 22) + 1420070400000) / 1000).ToLocalTime();

            new TimeSpan(date.Ticks);


            Base.Error($"{uid[0]} - {date.ToString()} - {(DateTime.Now - date).Days}");

            return true;
        }
        private bool CheckSteam(GameObject go)
        {
            return true;
        }


        private void HandlePunishments(GameObject go, string reason)
        {
            if (_banDurationOne < 0)
                return;

            Base.Disconnect(go, $"You have been automatically disconnected from the server by SmartGuard.\nReason: {reason}");

            Base.SmartGuard($"{go.GetComponent<NicknameSync>().MyNick} was {(_banDurationOne > 0 ? "banned" : "kicked")}: {reason}");

            if (_banDurationOne > 0)
            {
                Base.Disconnect(go, $"You have been automatically disconnected from the server by SmartGuard.\nReason: {reason}\nNote: This action was performed automatically by a server modification!");

                BanHandler.IssueBan(new BanDetails
                {
                    OriginalName = go.GetComponent<NicknameSync>().MyNick,
                    Id = go.GetComponent<CharacterClassManager>().UserId,
                    Expires = DateTime.UtcNow.AddMinutes(_banDurationOne).Ticks,
                    Reason = $"SMARTGUARD - {reason}",
                    Issuer = "SMARTGUARD",
                    IssuanceTime = DateTime.UtcNow.Ticks
                }, BanHandler.BanType.UserId);

                BanHandler.IssueBan(new BanDetails
                {
                    OriginalName = go.GetComponent<NicknameSync>().MyNick,
                    Id = go.GetComponent<NetworkIdentity>().connectionToClient.address,
                    Expires = DateTime.UtcNow.AddMinutes(_banDurationOne).Ticks,
                    Reason = $"SMARTGUARD - {reason}",
                    Issuer = "SMARTGUARD",
                    IssuanceTime = DateTime.UtcNow.Ticks
                }, BanHandler.BanType.IP);

                Base.SmartGuard($"{go.GetComponent<NicknameSync>().MyNick} was banned for: {reason}");
            }
        }
    }
}
