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

using PheggMod.API.Events;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::AlphaWarheadController")]
    internal class PMAlphaWarheadController : AlphaWarheadController
    {
        public static bool nukeLock = false;

        public extern void orig_CancelDetonation(GameObject disabler);
        public new void CancelDetonation(GameObject disabler)
        {
            if (nukeLock && disabler != null) return;

            orig_CancelDetonation(disabler);

            if (!(this.timeToDetonation <= 10f))
            {
                try { 
                    PluginManager.TriggerEvent<IEventHandlerWarheadCancel>(new WarheadCancelEvent(new PheggPlayer(disabler)));
                }
                catch (Exception e)
                {
                    Base.Error($"Error triggering WarheadCancelEvent: {e.ToString()}");
                }
            }
        }

        public extern void orig_StartDetonation();
        public new void StartDetonation()
        {
            if (nukeLock && !PMCommandProcessor.lastCommand.ToUpper().Contains("DETONATION_START")) return;

            orig_StartDetonation();

            bool InitialStart;
            if (this.timeToDetonation >= ConfigFile.ServerConfig.GetInt("warhead_tminus_start_duration", 90))
                InitialStart = true;
            else
                InitialStart = false;
            try
            {
                PluginManager.TriggerEvent<IEventHandlerWarheadStart>(new WarheadStartEvent(InitialStart, this.timeToDetonation));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering WarheadStartEvent: {e.ToString()}");
            }
        }

        public extern void orig_Detonate();
        public void Detonate()
        {
            orig_Detonate();

            try
            {
                PluginManager.TriggerEvent<IEventHandlerWarheadDetonate>(new WarheadDetonateEvent());
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering WarheadDetonateEvent: {e.ToString()}");
            }
        }
    }
}
