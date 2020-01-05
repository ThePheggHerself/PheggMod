using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API
{
    public abstract class Event
    {
        public abstract void ExecuteHandler(IEventHandler handler);
    }

    public interface IEventHandler
    {
    }

    class EventManager
    {
        public static List<IEventHandler> events = new List<IEventHandler>();


        public static void HandleEvent<t1, t2>(t1 ev) where t1 : Event where t2 : IEventHandler
        {
            foreach (t2 Event in events)
            {
                ev.ExecuteHandler(Event);
            }
        }

        public void AddEventHandlers(IEventHandler handler)
        {
            events.Add(handler);
        }
    }
}
