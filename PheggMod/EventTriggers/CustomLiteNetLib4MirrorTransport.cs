#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using LiteNetLib;
using LiteNetLib.Utils;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::CustomLiteNetLib4MirrorTransport")]
    public class PMCustomLiteNetLib4MirrorTransport : CustomLiteNetLib4MirrorTransport
    {
        protected extern void orig_ProcessConnectionRequest(ConnectionRequest request);

        protected override void ProcessConnectionRequest(ConnectionRequest request)
        {
            orig_ProcessConnectionRequest(request);

            //try
            //{
            //    Base.Debug("Triggering PreauthEvent");
            //    PluginManager.TriggerEvent<IEventHandlerPreauth>(new PreauthEvent(request.RemoteEndPoint, UserIds[request.RemoteEndPoint].UserId));
            //}
            //catch (Exception e)
            //{
            //    Base.Error($"Error triggering PreauthEvent: {e.InnerException.ToString()}");
            //}

            //NetDataWriter a = new NetDataWriter();
            //a.Reset();
            //a.Put(10);
            //a.Put("Custom Smelling badly");
            //a.Put("Testing");
            //request.Reject(a);

            //orig_ProcessConnectionRequest(request);

            //KeyValuePair<BanDetails, BanDetails> pair = BanHandler.QueryBan(null, request.RemoteEndPoint.Address.ToString());

            //if (pair.Value == null)
            //    Base.Debug("Endpoint is NOT banned");

            //else
            //{
            //    Base.Debug("Endpoint IS banned");

            //    NetDataWriter writer = new NetDataWriter();

            //    writer.Reset();
            //    writer.Put(6);
            //    writer.Put(pair.Value.Expires);

            //    request.RejectForce(writer);
            //}


            //Code used for testing integration of SmartGuard into the pre-authentication system (allowing for SmartGuard to trigger faster). Ignore it for now
            #region Pre-Auth testing

            //Ignore this code. It's simply here to gether dust until i either figure out reject type 10 (Kicked by a server modification),
            //or one of the game's developers comes along and tells me how it is used (It always returns (No Reason) currently)

            //NetDataWriter a = new NetDataWriter();
            //a.Reset();
            //a.Put(10);
            //a.Put("Custom Smelling badly");
            //a.Put("Testing");
            //request.RejectForce(a);


            ///// 1 = full server
            ///// 2 = invalid digital signature
            ///// 3 = different game version
            ///// 4 = unspecified error
            ///// 5 = requires auth
            ///// 6 = banned
            ///// 7 = not whitelisted
            ///// 8 = global ban
            ///// 9 = blocked country
            ///// 10 = Kicked by server modification
            ///// 11 = expired token
            ///// 12 = Rate limit

            //request.RejectForce(a);

            #endregion


        }
    }
}
