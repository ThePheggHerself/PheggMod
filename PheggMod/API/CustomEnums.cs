using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.API
{
    public enum TagColour
    {
        DEFAULT = 0,
        PINK,
        RED,
        BROWN,
        SILVER,
        LIGHT_GREEN,
        CRIMSON,
        CYAN,
        AQUA,
        DEEP_PINK,
        TOMATO,
        YELLOW,
        MAGENTA,
        BLUE_GREEN,
        ORANGE,
        LIME,
        GREEN,
        EMERALD,
        CARMINE,
        NICKEL,
        MINT,
        ARMY_GREEN,
        PUMPKIN
    }

    public enum CleanTeam
    {
        ChaosInsurgency = 0,
        NineTailedFox,
        SCP,
        Spectator,
        Tutorial,
    }

    public enum KillType
    {
        Normal = 0,
        TeamKill,
        DisarmedKill,
        WorldKill,
        AntiCheat
    }
}
