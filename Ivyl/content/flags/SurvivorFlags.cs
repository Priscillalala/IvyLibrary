using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a survivor.
    /// </summary>
    [Flags]
    public enum SurvivorFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This survivor will not appear in the character select screen (e.g., Heretic).
        /// </summary>
        Hidden = 1 << 0,
    }
}