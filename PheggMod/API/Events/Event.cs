using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API.Events
{
	public enum EventStage
	{
		PreEvent = 0,
		PostEvent = 1,
		InitEvent = 2
	}

	public abstract class Event
    {
		public EventStage Stage { get; internal set; }
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
