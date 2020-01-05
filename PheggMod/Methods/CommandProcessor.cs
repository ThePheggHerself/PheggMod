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
using RemoteAdmin;

namespace PheggMod
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

                BotWorker.NewMessage("```yaml"
                    + $"\nAdmin: {sender.Nickname}"
                    + $"\nExecuted: {q.ToUpper()}```");

                if (query[0].ToUpper().Contains("CASSIE"))
                {
                    if (!CheckPermissions(sender, query[0], PlayerPermissions.Noclip)) return;
                }
                else if(query[0].ToUpper() == "ENDROUND")
                {

                }
            }

            orig_ProcessQuery(q, sender);

        }

        private void RejectQuery(string error, CommandSender sender)
        {
            switch (error)
            {
                case "invalidperms":
                    sender.RaReply("SYSTEM#Insufficient permissions!", false, true, "");
                    break;
                default:
                    sender.RaReply("SYSTEM#Command Failed", false, true, "");
                    break;
            }

        }

        private static bool CheckPermissions(CommandSender sender, string queryZero, PlayerPermissions perm, string replyScreen = "", bool reply = true)
        {
            if (ServerStatic.IsDedicated && sender.FullPermissions)
            {
                return true;
            }
            if (PermissionsHandler.IsPermitted(sender.Permissions, perm))
            {
                return true;
            }
            if (reply)
            {
                sender.RaReply(queryZero + "#You don't have permissions to execute this command.\nMissing permission: " + perm, false, true, replyScreen);
            }
            return false;
        }
    }
}
