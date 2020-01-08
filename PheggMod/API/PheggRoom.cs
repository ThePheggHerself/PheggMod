using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.API
{
    class PheggRoom
    {
        public string zone { get; internal set; }
        public string room { get; internal set; }
        public Vector3 position { get; internal set; }

        public PheggRoom(Transform Object)
        {
            //string name2 = Object.parent.name;

            zone = Object.parent.name;
            room = Object.name;
            position = Object.position;
        }

        public void Flicker()
        {
            if (zone == "EZ" ||zone == null || room == null) return;

            Interface079.lply.CallRpcFlickerLights(this.room, this.zone, 15f);
        }
    }
}
