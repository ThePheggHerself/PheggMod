using PheggMod.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLab.DiscordBot
{
    public class DiscordBotHandler
    {
        public DiscordBotHandler()
        {
            StartAsync();
        }

        public async Task StartAsync()
        {
            Plugin.Warn("Hello, World!");

            await Task.Delay(-1);
        }
    }
}
