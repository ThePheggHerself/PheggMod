﻿#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using MonoMod;

using PheggMod.API.Events;

namespace PheggMod.Patches
{
    [MonoModPatch("global::RoundSummary")]
    public class PMRoundSummary : RoundSummary
    {
        public extern void orig_RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd);
        public void RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
        {
            RespawnManagerCrap.blockRespawns = false;
            TimeSpan tspan = TimeSpan.FromSeconds(list_finish.time - list_start.time);
            Base.roundStartTime = null;

            try
            {
                Base.Debug("Triggering RoundEndEvent");
                PluginManager.TriggerEvent<IEventHandlerRoundEnd>(new RoundEndEvent(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd, string.Format("{0} minutes and {1} seconds", (int)tspan.TotalMinutes, tspan.Seconds)));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering RoundEndEvent: {e.InnerException}");
            }

			//if (FFDetector.FFDetector.DetectorEnabled)
			//{
			//	FFDetector.FFDetector.FFPlayers.Clear();
			//	FFDetector.FFDetector.FFDGrenadeThrowers.Clear();
			//	FFDetector.FFDetector.DoCheck = false;
			//}

				orig_RpcShowRoundSummary(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd);
        }

        internal static void RoundFix()
        {
            if (!PMConfigFile.stickyRound) return;

            Base.Info("The server's player count has reached 0, so the round will be ended");
			RoundRestarting.RoundRestart.InitiateRoundRestart();
        }

        [MonoModEnumReplace]
        public enum LeadingTeam
        {
            FacilityForces,
            ChaosInsurgency,
            Anomalies,
            Draw
        }
    }
}
