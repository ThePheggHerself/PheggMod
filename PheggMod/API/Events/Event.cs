using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

    //Preauth event
    public interface IEventHandlerPreauth : IEventHandler
    {
        void OnPreauth(PreauthEvent ev);
    }
    public class PreauthEvent : Event
    {
        public PreauthEvent(IPEndPoint endpoint, string userId, CentralAuthPreauthFlags flags)
        {
            Endpoint = endpoint;
            UserID = userId;
            Flags = flags;
        }

        public EndPoint Endpoint { get; private set; }
        public string UserID { get; private set; }
        public CentralAuthPreauthFlags Flags { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPreauth)handler).OnPreauth(this);
        }
    }
}
