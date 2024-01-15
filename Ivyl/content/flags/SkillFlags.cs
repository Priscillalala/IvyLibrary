using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a skill.
    /// </summary>
    [Flags]
    public enum SkillFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        ResetCooldownTimerOnUse = 1 << 0,
        NoRestockOnAssign = 1 << 1,
        DontAllowPastMaxStocks = 1 << 2,
        BeginSkillCooldownOnSkillEnd = 1 << 3,
        /// <summary>
        /// This skill does not cancel sprinting on activation.
        /// </summary>
        Agile = 1 << 4,
        /// <summary>
        /// This skill forces sprinting throughout; commonly used for mobility skills.
        /// </summary>
        ForceSprint = 1 << 5,
        /// <summary>
        /// This skill is immediately canceled if the player starts sprinting.
        /// </summary>
        CanceledBySprinting = 1 << 6,
        /// <summary>
        /// Using this skill does not count as entering combat (e.g., will not cancel the Red Whip bonus).
        /// </summary>
        NonCombat = 1 << 7,
        /// <summary>
        /// Holding down an input will only activate this skill once.
        /// </summary>
        MustKeyPress = 1 << 8,
    }
}