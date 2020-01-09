using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using PheggMod.API.Plugin;
using PheggMod.API.Events;

namespace ExamplePlugin
{
    [Plugin.PluginDetails(
    author = "ThePheggHimself",
    name = "TestPlugin",
    description = "Test plugin for Pheggmod",
    version = "1.0_BETA"
    )]

    public class ExamplePlugin : Plugin
    {
        public override void initializePlugin()
        {
            Info("PLUGIN LOADED!!!!");

            this.AddEventHandlers(new Playerhurt(this));
        }
    }
}
