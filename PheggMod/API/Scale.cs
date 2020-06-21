using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.API
{
    public class Scale
    {
        public class scaleObject
        {
            public float lastMultiplier;
            public int respawnCount;
        }

        public static Dictionary<string, scaleObject> lastScales = new Dictionary<string, scaleObject>();
        static BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static;
        static MethodInfo sendSpawnMessage = null;

        public static void SetSize(float scaleMultiplier, GameObject gameObject)
        {
            try
            {
                NetworkIdentity nId = gameObject.GetComponent<NetworkIdentity>();

                gameObject.transform.localScale = new Vector3(1 * scaleMultiplier, 1 * scaleMultiplier, 1 * scaleMultiplier);

                ObjectDestroyMessage dMsg = new ObjectDestroyMessage();
                dMsg.netId = nId.netId;

                for (var q = 0; q < PlayerManager.players.Count; q++)
                {
                    NetworkConnection conn = PlayerManager.players[q].GetComponent<NetworkIdentity>().connectionToClient;

                    if (PlayerManager.players[q] != gameObject)
                        conn.Send(dMsg, 0);

                    if (sendSpawnMessage == null)
                        sendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", flags);

                    sendSpawnMessage.Invoke(null, new object[] { nId, conn });
                }
            }
            catch (Exception e) { Base.Error(e.ToString()); }
        }
    }
}
