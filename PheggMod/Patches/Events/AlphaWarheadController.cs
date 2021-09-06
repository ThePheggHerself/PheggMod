﻿#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
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

namespace PheggMod.Patches
{
    [MonoModPatch("global::AlphaWarheadController")]
    internal class PMAlphaWarheadController : AlphaWarheadController
    {
        public static bool nukeLock = false;

        public bool initialStart
        {
            get { return timeToDetonation >= PMConfigFile.detonationTimer; }
        }

        public extern void orig_CancelDetonation(GameObject disabler);
        public new void CancelDetonation(GameObject disabler)
        {
            if (nukeLock && disabler != null) return;

            orig_CancelDetonation(disabler);

            if (!(this.timeToDetonation <= 10f))
            {
                try
                {
                    Base.Debug("Triggering WarheadCancelEvent");
                    PluginManager.TriggerEvent<IEventHandlerWarheadCancel>(new WarheadCancelEvent(new PheggPlayer(disabler)));
                }
                catch (Exception e)
                {
                    Base.Error($"Error triggering WarheadCancelEvent: {e.InnerException}");
                }
            }
        }

        public extern void orig_StartDetonation();
        public new void StartDetonation()
        {
            if (nukeLock && !PMCommandProcessor.lastCommand.ToUpper().Contains("DETONATION_START"))
                return;

            orig_StartDetonation();
            if (!inProgress)
                return;

            try
            {
                Base.Debug("Triggering WarheadStartEvent");
                PluginManager.TriggerEvent<IEventHandlerWarheadStart>(new WarheadStartEvent(initialStart, this.timeToDetonation));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering WarheadStartEvent: {e}");
            }
        }

        public extern void orig_Detonate();
        public void Detonate()
        {
            orig_Detonate();

            try
            {
                Base.Debug("Triggering WarheadDetonateEvent");
                PluginManager.TriggerEvent<IEventHandlerWarheadDetonate>(new WarheadDetonateEvent());
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering WarheadDetonateEvent: {e}");
            }
        }
    }
}
