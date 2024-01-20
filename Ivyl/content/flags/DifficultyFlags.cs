using System;
using RoR2;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a difficulty.
    /// </summary>
    [Flags]
    public enum DifficultyFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This difficulty will apply Monsoon stat afflictions to the player and count as a hard difficulty for certain achievements (e.g., mastery achievements).
        /// </summary>
        HardMode = 1 << 0,
        /// <summary>
        /// This difficulty will not appear in the lobby.
        /// </summary>
        Hidden = 1 << 1,
    }
}