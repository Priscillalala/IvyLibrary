using System;

namespace Ivyl
{
    [Flags]
    public enum SkillFlags
    {
        None,
        ResetCooldownTimerOnUse = 1 << 0,
        NoRestockOnAssign = 1 << 1,
        DontAllowPastMaxStocks = 1 << 2,
        BeginSkillCooldownOnSkillEnd = 1 << 3,
        Agile = 1 << 4,
        ForceSprint = 1 << 5,
        CanceledBySprinting = 1 << 6,
        NonCombat = 1 << 7,
        MustKeyPress = 1 << 8,
    }
}