using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of an unlockable.
    /// </summary>
    [Flags]
    public enum UnlockableFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This unlockable will not appear in the game ending report.
        /// </summary>
        Hidden = 1 << 0,
    }
}