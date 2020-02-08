#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using PheggMod.API.Commands;
using PheggMod.API.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::RemoteAdmin.CommandProcessor")]
    class PMCommandProcessor
    {
        internal static string lastCommand;

        public static extern void orig_ProcessQuery(string q, CommandSender sender);
        public static void ProcessQuery(string q, CommandSender sender)
        {
            string[] query = q.Split(new char[] { ' ' });

            if (!q.ToUpper().Contains("SILENT"))
            {
                lastCommand = q;

                PheggPlayer Sender = new PheggPlayer(PlayerManager.players.Where(p => p.GetComponent<NicknameSync>().MyNick == sender.Nickname).FirstOrDefault());

                PluginManager.TriggerEvent<IEventHandlerAdminQuery>(new AdminQueryEvent(Sender, q));

                KeyValuePair<string, ICommand> cmdPair = PluginManager.allCommands.FirstOrDefault(w => w.Key == query[0].ToUpper());

                if (!cmdPair.Equals(default(KeyValuePair<string, ICommand>)))
                {
                    GameObject admin = PlayerManager.players.Where(s => s.GetComponent<NicknameSync>().MyNick == sender.Nickname).FirstOrDefault();
                    if (admin == null)
                    {
                        Base.Error($"Admin is null. This most likely means that the sender is the server!");
                        return;
                    }
                    else
                    {
                        PluginManager.TriggerCommand(cmdPair, q, admin, sender);
                    }
                    return;
                }
            }

            orig_ProcessQuery(q, sender);
        }
    }
}
