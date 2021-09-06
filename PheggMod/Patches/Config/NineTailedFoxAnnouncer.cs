using MonoMod;
using Respawning;
using System.Collections.Generic;

namespace PheggMod.Patches
{
    [MonoModPatch("global::NineTailedFoxAnnouncer")]
    class PMNineTailedFoxAnnouncer : NineTailedFoxAnnouncer
    {
        [MonoModReplace]
        public new void ServerOnlyAddGlitchyPhrase(string tts, float glitchChance, float jamChance)
        {
            if (PMConfigFile.cassieGlitch || (PMConfigFile.cassieGlitchDetonation && AlphaWarheadController.Host.detonated))
            {
                string[] array = tts.Split(' ');
                List<string> newWords = new List<string>();
                newWords.EnsureCapacity(array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    newWords.Add(array[i]);
                    if (i < array.Length - 1)
                    {
                        if (PMConfigFile.cassieGlitch || PMConfigFile.cassieGlitchDetonation && AlphaWarheadController.Host.detonated)
                        {

                            if (UnityEngine.Random.value < glitchChance * 2)
                            {
                                newWords.Add(".G" + UnityEngine.Random.Range(1, 7));
                            }
                            if (UnityEngine.Random.value < jamChance * 2)
                            {
                               newWords.Add(string.Concat(new object[]
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
                foreach (string str in newWords)
                {
                    tts = tts + str + " ";
                }
            }
            RespawnEffectsController.PlayCassieAnnouncement(tts, false, true);
        }
    }
}
