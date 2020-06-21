#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using MonoMod;
using UnityEngine;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::RagdollManager")]
    class PMRagdollManager : RagdollManager
    {
        public extern void orig_SpawnRagdoll(Vector3 pos, Quaternion rot, Vector3 velocity, int classId, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick, int playerId);
        public new void SpawnRagdoll(Vector3 pos, Quaternion rot, Vector3 velocity, int classId, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick, int playerId)
        {
            if (PMConfigFile.enable008 && (ragdollInfo.GetDamageType() == DamageTypes.Scp0492 || ragdollInfo.GetDamageType() == DamageTypes.Poison))
                return;
            else
                orig_SpawnRagdoll(pos, rot, velocity, classId, ragdollInfo, allowRecall, ownerID, ownerNick, playerId);
        }
    }
}
