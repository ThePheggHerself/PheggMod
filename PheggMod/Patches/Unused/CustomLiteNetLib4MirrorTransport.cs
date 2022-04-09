#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using LiteNetLib;
using LiteNetLib.Utils;
using MonoMod;
using PheggMod.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Patches
{
    [MonoModPatch("global::CustomLiteNetLib4MirrorTransport")]
    public class PMCustomLiteNetLib4MirrorTransport : CustomLiteNetLib4MirrorTransport
    {
        protected extern void orig_ProcessConnectionRequest(ConnectionRequest request);

        protected override void ProcessConnectionRequest(ConnectionRequest request)
        {
            IPEndPoint ipep = request.RemoteEndPoint;		
            if (ipep.Port < 1024)
                return;

			//Base.Info($"Port for incoming connection from {ipep.Address} is {ipep.Port}");

			orig_ProcessConnectionRequest(request);

            if (UserIds.ContainsKey(ipep))
            {
                try
                {
                    Base.Debug("Triggering PreauthEvent");
                    PluginManager.TriggerEvent<IEventHandlerPreauth>(new PreauthEvent(ipep, UserIds[ipep].UserId));
                }
                catch (Exception e)
                {
                    Base.Error($"Error triggering PreauthEvent: {e.InnerException}");
                }
            }

			#region a
			//if (UserIds.ContainsKey(request.RemoteEndPoint))
			//{
			//    if (PMConfigFile.enableSmartGuard)
			//    {
			//        NetDataReader nDR = request.Data;

			//        if (!nDR.TryGetByte(out var a) || !nDR.TryGetByte(out var b) || !nDR.TryGetByte(out var c) || !nDR.TryGetInt(out var d) ||
			//            !nDR.TryGetBytesWithLength(out var e) || !nDR.TryGetString(out var f) || !nDR.TryGetULong(out var g) || !nDR.TryGetByte(out byte flags))
			//            return;

			//        SmartGuard.SGPreauthCheck(UserIds[request.RemoteEndPoint], (CentralAuthPreauthFlags)flags, out SmartGuard.InfractionType infractionType, out string Reason);

			//        //if(infractionType != SmartGuard.InfractionType.none)
			//        //{
			//        NetDataWriter RequestWriter = new NetDataWriter();
			//        RequestWriter.Reset();
			//        RequestWriter.Put((byte)RejectionReason.Custom);
			//        RequestWriter.Put("TESTING123");
			//        request.RejectForce(RequestWriter);
			//        return;
			//        //}
			//    }
			//}

			//try
			//{
			//    Base.Debug("Triggering PreauthEvent");
			//    PluginManager.TriggerEvent<IEventHandlerPreauth>(new PreauthEvent(request.RemoteEndPoint, UserIds[request.RemoteEndPoint].UserId));
			//}
			//catch (Exception e)
			//{
			//    Base.Error($"Error triggering PreauthEvent: {e.InnerException.ToString()}");
			//}

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
			#endregion
		}

		private static extern bool orig_CheckIpRateLimit(ConnectionRequest request);
        private static bool CheckIpRateLimit(ConnectionRequest request) => orig_CheckIpRateLimit(request);
    }
}
