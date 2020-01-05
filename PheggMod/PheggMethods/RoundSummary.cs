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

namespace PheggMod
{
    [MonoModPatch("global::RoundSummary")]
    class PMRoundSummary : RoundSummary
    {
        public extern void orig_RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd);
        public void RpcShowRoundSummary(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
        {
            RoundEnd.OnRoundEnd(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd);

            orig_RpcShowRoundSummary(list_start, list_finish, leadingTeam, e_ds, e_sc, scp_kills, round_cd);
        }
    }

    class RoundEnd
    {
        internal static void OnRoundEnd(RoundSummary.SumInfo_ClassList list_start, RoundSummary.SumInfo_ClassList list_finish, RoundSummary.LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
        {
            TimeSpan tspan = TimeSpan.FromSeconds(list_finish.time - list_start.time);

            Base.AddLog(tspan.TotalSeconds.ToString());

            BotWorker.NewMessage($"**Round Ended**\n```Round Time: {string.Format("{0} minutes and {1} seconds", (int)tspan.TotalMinutes, tspan.Seconds)}"
                + $"\nEscaped Class-D: {e_ds}/{list_start.class_ds}"
                + $"\nRescued Scientists: {e_sc}/{list_start.scientists}"
                + $"\nTerminated SCPs: {list_start.scps_except_zombies - list_finish.scps_except_zombies}/{list_start.scps_except_zombies}"
                + $"\nWarhead Status: {(AlphaWarheadController.Host.detonated == false ? "Not Detonated" : $"Detonated ({list_finish.warhead_kills} killed)")}```");
        }
    }
}
