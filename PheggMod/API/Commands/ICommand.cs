using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.API.Commands
{
    public abstract class Command
    {
        public abstract void HandleCommand(string command, GameObject admin, CommandSender sender);
    }
    public interface ICommand
    {
        void HandleCommand(string command, GameObject admin, CommandSender sender);
    }
}
