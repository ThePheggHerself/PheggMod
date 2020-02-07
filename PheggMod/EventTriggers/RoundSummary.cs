#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
using System;
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

            PluginManager.TriggerEvent<IEventHandlerRoundEnd>(new RoundEndEvent(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd, string.Format("{0} minutes and {1} seconds", (int)tspan.TotalMinutes, tspan.Seconds)));

            orig_RpcShowRoundSummary(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd);
        }
    }
}
