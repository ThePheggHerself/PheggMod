#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using System.IO;
using System.Reflection;
using UnityEngine;
using Mirror;
using System.Net;
using GameCore;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::NineTailedFoxAnnouncer")]
    class PMNineTailedFoxAnnouncer : NineTailedFoxAnnouncer
    {
        //public extern void orif_ServerOnlyAddGlitchyPhrase(string tts, float glitchChance, float jamChance);

        public new void ServerOnlyAddGlitchyPhrase(string tts, float glitchChance, float jamChance)
        {
            string[] array = tts.Split(new char[]
            {
            ' '
            });
            this.newWords.Clear();
            this.newWords.EnsureCapacity(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                this.newWords.Add(array[i]);
                if (i < array.Length - 1)
                {
                    if (ConfigFile.ServerConfig.GetBool("cassie_glitch", false) || (ConfigFile.ServerConfig.GetBool("cassie_glitch_post_detonation", false) && AlphaWarheadController.Host.detonated))
                    {

                        if (UnityEngine.Random.value < glitchChance * 2)
                        {
                            this.newWords.Add(".G" + UnityEngine.Random.Range(1, 7));
                        }
                        if (UnityEngine.Random.value < jamChance * 2)
                        {
                            this.newWords.Add(string.Concat(new object[]
                            {
                        "JAM_",
                        UnityEngine.Random.Range(0, 70).ToString("000"),
                        "_",
                        UnityEngine.Random.Range(2, 6)
                            }));
                        }
                    }
                }
            }
            tts = "";
            foreach (string str in this.newWords)
            {
                tts = tts + str + " ";
            }
            PlayerManager.localPlayer.GetComponent<MTFRespawn>().RpcPlayCustomAnnouncement(tts, false, true);

        }

        private List<string> newWords = new List<string>();
    }
}
