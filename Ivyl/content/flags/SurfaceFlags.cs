using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a surface.
    /// </summary>
    [Flags]
    public enum SurfaceFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This surface will cause characters to slide around with reduced friction (e.g., the ice on Siphoned Forest).
        /// </summary>
        Slippery = 1 << 0,
    }
}