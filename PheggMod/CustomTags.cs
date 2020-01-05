#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using MEC;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace PheggMod
{
    class CustomTags
    {
        public static List<TagData> tagDataList = new List<TagData>();

        [Serializable]
        public class TagData
        {
            public string steamID;
            public string discordID;
            public string prefix;
            public string suffix;
            public string colour;
        }


        [Serializable]
        public class TDList
        {
            public List<TagData> tdlist;
        }

        public static IEnumerator<float> HandleTagUpdate()
        {
            DownloadHandlerBuffer download = new DownloadHandlerBuffer();

            UnityWebRequest www = new UnityWebRequest($"{Base.APILocation}GetTags.php");
            www.downloadHandler = download;

            yield return Timing.WaitUntilDone(www.SendWebRequest());

            if (www.isHttpError || www.isNetworkError)
            {
                Base.AddLog(www.error.ToString());
                yield return 0f;
            }

            TDList tdlist = JsonUtility.FromJson<TDList>(www.downloadHandler.text);

            tagDataList = tdlist.tdlist;

            yield return 0f;
        }

        public static IEnumerator<float> CustomTag(CharacterClassManager player)
        {
            Base.AddLog("APPLES");

            string[] uid = player.UserId.Split('@');

            int index = tagDataList.FindIndex(p => uid[1].ToUpper() == "DISCORD" ? p.discordID == uid[0] : p.steamID == uid[0]);

            Base.AddLog(index.ToString());

            if (index < 0) yield return 0f;

            ServerRoles sroles = player.GetComponent<ServerRoles>();

            if (sroles.Permissions > 0)
            {
                ulong perms = sroles.Permissions;

                sroles.SetText(tagDataList[index].prefix + " - " + tagDataList[index].suffix);
                sroles.SetColor(tagDataList[index].colour);
                sroles.Permissions = perms;
            }

            yield return 0f;
        }
    }
}
