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

using PheggMod.API;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::Generator079")]
    class PMGenerator079 : Generator079
    {
        //BAD CODE
        //None of this works thanks to the dumbass code. Keeping it here for now as maybe eventually i can get it working (probably not)

        //public extern void orig_EjectTablet();
        //public new void EjectTablet()
        //{
        //    orig_EjectTablet();

        //    try
        //    {
        //        GameObject roomGO = GameObject.Find("HeavyRooms" + "/" + this.curRoom);

        //        if (roomGO == null)
        //        {
        //            Base.AddLog("IsNULL");
        //            return;
        //        }

        //        if (this.GetComponent<Scp079PlayerScript>() == null)
        //        {
        //            Base.AddLog("PlayerScriptNull");
        //            return;
        //        }

        //        PheggRoom proom = new PheggRoom(roomGO.transform);

        //        proom.Flicker();

        //        //foreach (FlickerableLight flickerableLight in FindObjectsOfType<FlickerableLight>())
        //        //{
        //        //    if(flickerableLight == null)
        //        //    {
        //        //        Base.AddLog("flickerableLight IsNULL");
        //        //        return;
        //        //    }

        //        //    //if (component.currentZonesAndRooms[0].currentRoom == this.curRoom)
        //        //        flickerableLight.EnableFlickering(15f);


        //        //    //if (component == null || component.currentZonesAndRooms[0].currentZone == "HeavyRooms")
        //        //    //{
        //        //    //    flickerableLight.EnableFlickering(10f);
        //        //    //}
        //        //}

        //        //roomGO.GetComponent<Scp079Interactable>().currentZonesAndRooms.Where(t=> t.currentRoom == this.curRoom).FirstOrDefault() == null

        //        //if (GetComponent<Scp079Interactable>() == null)
        //        //{
        //        //    Base.AddLog("roomGO Interactable IsNULL");
        //        //    return;
        //        //}

        //        //Interface079.lply.CallRpcFlickerLights(this.curRoom, "HeavyRooms", 15);

        //        //gameObject2.GetComponent<Scp079PlayerScript>().CallRpcFlickerLights(this.curRoom, "HeavyRooms", 15);

        //        //Scp079PlayerScript scp079plrs = new Scp079PlayerScript();

        //        //scp079plrs.CallRpcFlickerLights(this.curRoom, "HeavyRooms", 15);
        //    }
        ////    catch (Exception e) { Base.Error(e.Message); }
        ////}
    }
}
