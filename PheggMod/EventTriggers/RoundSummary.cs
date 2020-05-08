#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
using System.Collections.Generic;
using GameCore;
using MEC;
using MonoMod;

using PheggMod.API.Events;

namespace PheggMod.EventTriggers
{
    [MonoModPatch("global::RoundSummary")]
    class PMRoundSummary : RoundSummary
    {
        public extern void orig_RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd);
        public void RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
        {
            TimeSpan tspan = TimeSpan.FromSeconds(list_finish.time - list_start.time);

            try
            {
                PluginManager.TriggerEvent<IEventHandlerRoundEnd>(new RoundEndEvent(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd, string.Format("{0} minutes and {1} seconds", (int)tspan.TotalMinutes, tspan.Seconds)));
            }
            catch (Exception e)
            {
                Base.Error($"Error triggering RoundEndEvent: {e.ToString()}");
            }

            orig_RpcShowRoundSummary(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd);
        }

        //public extern void orig_CallRpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd);
        //public new void CallRpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
        //{
        //    RpcShowRoundSummary(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd);
        //}

        internal static void RoundFix()
        {
            if (!ConfigFile.ServerConfig.GetBool("fix_sticky_round", true)) return;

            Base.Info("The server's player count has reached 0, so the round will be ended");
            PlayerManager.localPlayer.GetComponent<PlayerStats>().Roundrestart();
        }
    }
}
