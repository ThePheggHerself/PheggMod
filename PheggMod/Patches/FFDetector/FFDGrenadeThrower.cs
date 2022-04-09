using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PheggMod.Patches
{
	public class FFDGrenadeThrower
	{
		public string UserId;
		public DateTime ThrownAt;
		public Vector3 DetonatePosition;
		public RoleType Role;
		public Team Team;
	}
}
