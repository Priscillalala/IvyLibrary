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
        /// <summary>
        /// Using this skill resets any current cooldown progress to zero.
        /// </summary>
        ResetCooldownTimerOnUse = 1 << 0,
        /// <summary>
        /// This skill will not regain max stock when reassigned (e.g., when a skill override is unset).
        /// </summary>
        NoRestockOnAssign = 1 << 1,
        /// <summary>
        /// This stock of this skill is capped to max stock when the skill is assigned.
        /// </summary>
        DontAllowPastMaxStocks = 1 << 2,
        /// <summary>
        /// The cooldown of this skill is frozen until the activation state is completed.
        /// </summary>
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