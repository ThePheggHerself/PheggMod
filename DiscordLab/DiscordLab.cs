using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Harmony;

using PheggMod.API.Plugin;
using PheggMod.API.Events;
using UnityEngine;
using System;
using PheggMod.API.Commands;
using Mirror;
using Cryptography;
using RemoteAdmin;
using System.Text.RegularExpressions;
using GameCore;
using System.Threading;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace DiscordLab
{
    [Plugin.PluginDetails(
        author = "ThePheggHimself",
        name = "DiscordLab",
        description = "Basic logging bot for SCP: Secret Laboratory",
        version = "1.0"
    )]

    public class DiscordLab : Plugin
    {
        public static Bot bot;
        public override void initializePlugin()
        {
            bot = new Bot();
        }
    }

    public class Bot
    {
        private static Regex _rgx = new Regex("(.gg/)|(<@)|(http)|(www)");

        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private DateTime _lastSentMessage;
        internal static char[] validUnits = { 'm', 'h', 'd', 'w', 'M', 'y' };

        public enum messageType
        {
            MSG = 0,
            CMD = 1,
            PLIST = 2,
            SUPDATE = 3,
            KEEPALIVE = 4
        }

        private class msgMessage
        {
            public string Type = "msg";
            public string Message;
        }
        private class cmdMessage
        {
            public string Type = "cmdmsg";
            public string CommandMessage;
            public string ChannelID;
            public string StaffID;
        }
        private class plistMessage
        {
            public string Type = "plist";
            public string PlayerNames;
            public string PlayerCount = PlayerManager.players.Count() + "/" + ConfigFile.ServerConfig.GetInt("max_players", 20);
            public string ChannelID;
            public string ServerIP;
        }
        private class supdateMessage
        {
            public string Type = "supdate";
            public string CurrentPlayers = PlayerManager.players.Count() + "/" + ConfigFile.ServerConfig.GetInt("max_players", 20);
        }
        private class keepaliveMessage
        {
            public string Type = "keepalive";
        }

        private supdateMessage _lastSupdateMessage;

        public Bot()
        {
            Timer timer = new Timer((t) => { StatusUpdate(); }, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        }

        private void StatusUpdate()
        {
            supdateMessage message = new supdateMessage();
            if (_lastSupdateMessage.CurrentPlayers == message.CurrentPlayers) return;
        }
        private void KeepAlive()
        {
            if ((DateTime.Now - _lastSentMessage).TotalMinutes < 30) return;

            NewMessage("RandomStringToKeepAlive", messageType.KEEPALIVE);
        }

        public void NewMessage(string message, messageType type = messageType.MSG)
        {
            if (string.IsNullOrEmpty(message)) return;

            string json;

            if (type == messageType.MSG)
            {
                msgMessage msg = new msgMessage()
                {
                    Message = _rgx.Replace(message, string.Empty)
                };

                json = JsonConvert.SerializeObject(msg);
            }
            else if (type == messageType.KEEPALIVE)
            {
                keepaliveMessage msg = new keepaliveMessage();

                json = JsonConvert.SerializeObject(msg);
            }
            else
            {
                throw new Exception("Invalid messageType given!");
            }

            SendMessage(json);
        }

        private void SendMessage(string json)
        {
            if (!_socket.Connected) return;

            try
            {
                _socket.Send(Encoding.UTF8.GetBytes(json));
                _lastSentMessage = DateTime.Now;
            }
            catch (Exception e) {
                Plugin.Error(e.InnerException.Message + "\n" + e.InnerException.StackTrace);
            }
        }
    }
}
