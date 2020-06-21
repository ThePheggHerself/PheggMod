using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API
{
    public class PlayerHurtCache
    {
        public RoleType PlayerOriginalRole { get; internal set; }
        public RoleType? AttackerOriginalRole { get; internal set; }
        public PMDamageType PMDamageType { get; internal set; }
        public PlayerStats.HitInfo HitInfo { get; internal set; }
    }
}
