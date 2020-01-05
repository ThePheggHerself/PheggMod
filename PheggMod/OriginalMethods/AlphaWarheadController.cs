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

namespace PheggMod
{
    [MonoModPatch("global::AlphaWarheadController")]
    class PMAlphaWarheadController : AlphaWarheadController
    {
        public extern void orig_CancelDetonation(GameObject disabler);
        public new void CancelDetonation(GameObject disabler)
        {
            orig_CancelDetonation(disabler);

            if (this.timeToDetonation <= 10f) return;

            BotWorker.NewMessage($"**Warhead detonation canceled{(disabler != null ? $" by {disabler.GetComponent<NicknameSync>().MyNick}" : "")}!**");
        }

        public extern void orig_StartDetonation();
        public new void StartDetonation()
        {
            orig_StartDetonation();

            if(this.timeToDetonation >= ConfigFile.ServerConfig.GetInt("warhead_tminus_start_duration", 90))
                BotWorker.NewMessage($"**Alpha warhead detonation sequence engaged! The underground ection of the facility will be detonated in T-minus {this.timeToDetonation} seconds!**");
            else
                BotWorker.NewMessage($"**Alpha warhead detonation sequence resumed! {this.timeToDetonation} seconds to detonation!**");            
        }

        public extern void orig_Detonate();
        public void Detonate()
        {
            orig_Detonate();

            BotWorker.NewMessage($"**Alpha warhead detonation sequence complete!**");
        }
    }
}
