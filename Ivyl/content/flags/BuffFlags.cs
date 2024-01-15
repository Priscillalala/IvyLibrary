using System;
using RoR2;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a buff.
    /// </summary>
    [Flags]
    public enum BuffFlags
    {
        /// <summary>
        /// Default state.
        /// </summary>
        None,
        /// <summary>
        /// This buff can be stacked multiple times and will display those stacks visually.
        /// </summary>
        Stackable = 1 << 0,
        /// <summary>
        /// This buff is considered a negative effect (e.g., contributes to Death Mark, cleansed by Blast Shower).
        /// </summary>
        Debuff = 1 << 1,
        /// <summary>
        /// This buff is considered a cooldown for some other bonus (e.g., cleansed by Blast Shower).
        /// </summary>
        Cooldown = 1 << 2,
        /// <summary>
        /// This buff will not display with an icon.
        /// </summary>
        Hidden = 1 << 3,
    }
}