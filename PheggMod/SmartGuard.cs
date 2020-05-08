using GameCore;
using Mirror;
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
        private bool _skipServerStaff;
        private bool _skipGlobalStaff;
        private bool _respectDNT;
        private bool _smartNameFilter;

        private string _apiKey;

        private List<string> _nameWhitelist;
        private List<string> _nameBlacklist;

        private List<string> _uidWhitelist;
        private List<string> _uidBlacklist;

        private int _banDuration;

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
            _enable = ConfigFile.ServerConfig.GetBool("smart_guard", true);
            _skipServerStaff = ConfigFile.ServerConfig.GetBool("whitelist_server_staff", true);
            _skipGlobalStaff = ConfigFile.ServerConfig.GetBool("whitelist_global_staff", true);
            _respectDNT = ConfigFile.ServerConfig.GetBool("respect_dnt_flag", false);
            _smartNameFilter = ConfigFile.ServerConfig.GetBool("smart_name_filter", true);

            _apiKey = ConfigFile.ServerConfig.GetString("steam_api_key", null);

            _nameWhitelist = ConfigFile.ServerConfig.GetStringList("smart_guard_whitelist_names");
            _nameBlacklist = ConfigFile.ServerConfig.GetStringList("smart_guard_blacklist_names");
            _uidWhitelist = ConfigFile.ServerConfig.GetStringList("smart_guard_whitelist_uids");
            _uidBlacklist = ConfigFile.ServerConfig.GetStringList("smart_guard_whitelist_uids");

            _banDuration = ConfigFile.ServerConfig.GetInt("smart_guard_ban_duration", 0);

            _nameBlacklist.AddRange(new List<string> { "kite1101", "beefteef420", "saltcollector", "phoenix" });

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
                if (!_enable /*|| string.IsNullOrEmpty(ConfigFile.ServerConfig.GetString("steam_api_key", null))*/)
                    return;

                CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();
                ServerRoles sr = go.GetComponent<ServerRoles>();
                NicknameSync ns = go.GetComponent<NicknameSync>();

                CustomLiteNetLib4MirrorTransport cln = go.GetComponent<CustomLiteNetLib4MirrorTransport>();

                //Whitelist Check
                if (sr.BypassStaff && _skipGlobalStaff)
                {
                    Base.SmartGuard("User is global staff. Skipping...");
                    return;
                }
                else if (sr.RemoteAdmin && _skipServerStaff)
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
                if (_smartNameFilter)
                {
                    string antil33t = _rgx.Replace(ns.MyNick.ToLower(), string.Empty);

                    foreach (KeyValuePair<string, string> pair in leetrules)
                    {
                        antil33t = antil33t.Replace(pair.Key, pair.Value);
                    }

                    if (_nameBlacklist.Contains(antil33t))
                    {
                        if (_banDuration == 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                        else if (_banDuration > 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                    }
                }
                else if (!_smartNameFilter)
                {
                    if (_nameBlacklist.Contains(ns.MyNick.ToLower()))
                    {
                        if (_banDuration == 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                        else if (_banDuration > 0)
                        {
                            HandlePunishments(go, "Blacklisted name");
                            return;
                        }
                    }
                }

                if (_uidBlacklist.Contains(ccm.UserId))
                {
                    if (_banDuration == 0)
                    {
                        HandlePunishments(go, "Blacklisted UID");
                    }
                    else if (_banDuration > 0)
                    {
                        HandlePunishments(go, "Blacklisted UID");
                    }
                }

            }
            catch (Exception e)
            {
                Base.Error(e.ToString());
            }
        }
        private void HandlePunishments(GameObject go, string reason)
        {
            if (_banDuration < 0)
                return;

            Base.Disconnect(go, $"You have been automatically disconnected from the server by SmartGuard.\nReason: {reason}");

            Base.SmartGuard($"{go.GetComponent<NicknameSync>().MyNick} was {(_banDuration > 0 ? "banned" : "kicked" )}: {reason}");

            if (_banDuration > 0)
            {
                Base.Disconnect(go, $"You have been automatically disconnected from the server by SmartGuard.\nReason: {reason}");

                BanHandler.IssueBan(new BanDetails
                {
                    OriginalName = go.GetComponent<NicknameSync>().MyNick,
                    Id = go.GetComponent<CharacterClassManager>().UserId,
                    Expires = DateTime.UtcNow.AddMinutes(_banDuration).Ticks,
                    Reason = $"SMARTGUARD - {reason}",
                    Issuer = "SMARTGUARD",
                    IssuanceTime = DateTime.UtcNow.Ticks
                }, BanHandler.BanType.UserId);

                BanHandler.IssueBan(new BanDetails
                {
                    OriginalName = go.GetComponent<NicknameSync>().MyNick,
                    Id = go.GetComponent<NetworkIdentity>().connectionToClient.address,
                    Expires = DateTime.UtcNow.AddMinutes(_banDuration).Ticks,
                    Reason = $"SMARTGUARD - {reason}",
                    Issuer = "SMARTGUARD",
                    IssuanceTime = DateTime.UtcNow.Ticks
                }, BanHandler.BanType.IP);

                Base.SmartGuard($"{go.GetComponent<NicknameSync>().MyNick} was banned for: {reason}");
            }
        }
    }
}
