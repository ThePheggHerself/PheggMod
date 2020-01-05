using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API.Events
{
    public abstract class Event
    {
        public abstract void ExecuteHandler(IEventHandler handler);
    }

    public interface IEventHandler
    {
    }
}
