#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using MEC;
using System.Threading;

namespace PheggMod
{
    public class BotWorker
    {
        [System.Serializable]
        public class MessageObject
        {
            //Message (Type: msg)
            public string Type;
            public string Message;

            ////AdminCommand (Type: acmd)
            //public string CommandMessage { get; set; }

            //Status Update (Type: supdate)
            public int CurrentPlayers;
            public int MaxPlayers = ((CustomNetworkManager)CustomNetworkManager.singleton).ReservedMaxPlayers;

            //Player List (Type: plist)
            public string PlayerNames;
            public string ChannelID;
            public string ServerIP;
        }

        internal static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        internal static DateTime lastMessage;

        public static void NewMessage(string message, string type = "msg")
        {
            if (string.IsNullOrEmpty(message)) return;
            else
            {
                MessageObject newmessage = new MessageObject();
                newmessage.Type = type;
                newmessage.Message = $"[{DateTime.Now.ToString("HH:mm:ss")}] {message}";

                string msg = JsonUtility.ToJson(newmessage);

                if (socket.Connected)
                {
                    SendMessage(msg);
                }
                else
                {
                    OpenConnection();
                    SendMessage(msg);
                    return;
                }
            }
        }

        private static void SendMessage(string message)
        {
            if (socket.Connected)
            {
                try
                {
                    socket.Send(Encoding.UTF8.GetBytes(message));
                    lastMessage = DateTime.Now;
                }
                catch (Exception e) { Base.Error(e.Message); }
            }
            else return;
        }

        public static void OpenConnection()
        {
            int port = Base.Port + 1000;

            if (port == 1000)
            {
                return;
            }
            else
            {
                try
                {
                    socket.Connect(IPAddress.Parse("127.0.0.1"), port);
                    Base.Info("Connection Successful!");
                }
                catch (Exception)
                {
                    try
                    {
                        socket.Disconnect(true);

                        socket.Connect(IPAddress.Parse("127.0.0.1"), port);
                        Base.Info("Connection Successful!");
                    }
                    catch (Exception e)
                    {
                        Base.Error(e.Message);
                    }
                }

            }
        }

        internal static IEnumerator<float> UpdatePlayerCount()
        {
            yield return Timing.WaitForSeconds(6f);

            if (socket.Connected)
            {
                MessageObject newmessage = new MessageObject();
                newmessage.Type = "supdate";
                newmessage.CurrentPlayers = PlayerManager.players.Count;

                string msg = JsonUtility.ToJson(newmessage);

                SendMessage(msg);
            }

            Timing.RunCoroutine(UpdatePlayerCount());
        }
        public class botmessage
        {
            //Types: plist, command

            public string Type;
            public string channel;
            public string Message = null;
        }

        public static void BotListener()
        {
            while (2 > 1)
            {
                if (socket.Connected)
                {
                    byte[] data = new byte[4096];
                    int dataLength = socket.Receive(data);

                    string incomingData = Encoding.UTF8.GetString(data, 0, dataLength);

                    List<string> messages = new List<string>(incomingData.Split('\n'));

                    Base.AddLog(messages.Count.ToString());

                    while (messages.Count > 0)
                    {
                        Base.AddLog(messages[0].ToString());

                        if (!string.IsNullOrEmpty(messages[0]))
                        {
                            botmessage botmsg = new botmessage();

                            JsonUtility.FromJsonOverwrite(messages[0], botmsg);

                            if (botmsg.Type == "plist")
                            {
                                List<string> plist = new List<string>();

                                foreach (GameObject go in PlayerManager.players)
                                {
                                    plist.Add(go.GetComponent<NicknameSync>().MyNick);
                                }

                                MessageObject newmessage = new MessageObject();
                                newmessage.Type = "plist";
                                newmessage.PlayerNames = String.Join("\n", plist);
                                newmessage.CurrentPlayers = PlayerManager.players.Count;
                                newmessage.ChannelID = botmsg.channel;
                                newmessage.ServerIP = Base.Ip + ":" + Base.Port;
                                string msg = JsonUtility.ToJson(newmessage);

                                SendMessage(msg);
                            }
                        }

                        messages.RemoveAt(0);
                    }
                }
                else
                {
                    OpenConnection();
                }

                Thread.Sleep(1000);
            }
        }

        internal static IEnumerator<float> KeepConnectionAlive()
        {
            if((DateTime.Now - lastMessage).TotalMinutes > 45)
            {
                NewMessage("RandomPacketToKeepAlive!", "alive");
            }

            yield return Timing.WaitForSeconds(1800);

            KeepConnectionAlive();
        }
    }
}
