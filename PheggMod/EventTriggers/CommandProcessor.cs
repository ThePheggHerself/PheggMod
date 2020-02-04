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
    [MonoModPatch("global::RemoteAdmin.CommandProcessor")]
    class PMCommandProcessor
    {
        public static extern void orig_ProcessQuery(string q, CommandSender sender);
        public static void ProcessQuery(string q, CommandSender sender)
        {
            string[] query = q.Split(new char[] { ' ' });

            if (!q.ToUpper().Contains("SILENT"))
            {
                PheggPlayer Sender = new PheggPlayer(PlayerManager.players.Where(p => p.GetComponent<NicknameSync>().MyNick == sender.Nickname).FirstOrDefault());

                PluginManager.TriggerEvent<IEventHandlerAdminQuery>(new AdminQueryEvent(Sender, q));
            }

            orig_ProcessQuery(q, sender);
        }
    }
}
