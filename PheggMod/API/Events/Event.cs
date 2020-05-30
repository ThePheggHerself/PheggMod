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
        public PreauthEvent(IPEndPoint endpoint, string userId)
        {
            Endpoint = endpoint;
            UserID = userId;
        }

        public EndPoint Endpoint { get; private set; }
        public string UserID { get; private set; }

        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerPreauth)handler).OnPreauth(this);
        }
    }
}
