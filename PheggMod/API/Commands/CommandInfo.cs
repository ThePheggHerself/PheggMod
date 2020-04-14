using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.API.Commands
{
    public class CommandInfo
    {
        public CommandSender commandSender { get; }
        public GameObject gameObject { get; }
        public string commandName { get; }
        public string[] commandArgs { get; }

        public CommandInfo(CommandSender commandSender, GameObject gameObject, string commandName, string[] commandArgs)
        {
            this.commandSender = commandSender;
            this.gameObject = gameObject;
            this.commandName = commandName;
            this.commandArgs = commandArgs;
        }
    }
}
