using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a game ending.
    /// </summary>
    [Flags]
    public enum GameEndingFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This game ending is considered a successful ending.
        /// </summary>
        Victory = 1 << 0,
        /// <summary>
        /// The main credits will be shown during this game ending.
        /// </summary>
        ShowCredits = 1 << 1,
    }
}