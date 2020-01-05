using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API.Plugin
{
    public abstract class Plugin
    {
        public abstract void initializePlugin();

        public class PluginDetails : Attribute
        {
            public string name;
            public string author;
            public string description;
            public string version;
        }

        public PluginDetails Details
        {
            get;
            internal set;
        }

        public void AddEventHandlers(IEventHandler handler)
        {
            PluginManager.AddEventHandlers(this, handler);
        }

        public void Error(string m) => Base.Error(string.Format("{0} | {1}", Assembly.GetCallingAssembly().GetName().Name, m));
        public void Warn(string m) => Base.Warn(string.Format("{0} | {1}", Assembly.GetCallingAssembly().GetName().Name, m));
        public void Info(string m) => Base.Info(string.Format("{0} | {1}", Assembly.GetCallingAssembly().GetName().Name, m));
    }
}
