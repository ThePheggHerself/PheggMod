using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Patches
{
	public class FFInfo
	{
		public PlayerStatsSystem.AttackerDamageHandler AttackerDamageHandler;
		public ReferenceHub Target;
		public ReferenceHub Attacker;
		public Team AttackerTeam;
		public List<ReferenceHub> Hostiles = new List<ReferenceHub>();
		public List<ReferenceHub> Friendlies = new List<ReferenceHub>();
		public List<ReferenceHub> NearbyPlayers = new List<ReferenceHub>();
		public FFDGrenadeThrower FFDGrenadeThrower;
		public FFPlayer FFPlayer;
		public DateTime LastLegitDamage;
	}
}
