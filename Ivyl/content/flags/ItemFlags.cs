using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of an item.
    /// </summary>
    [Flags]
    public enum ItemFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This item will not be displayed in the HUD.
        /// </summary>
        Hidden = 1 << 0,
        /// <summary>
        /// This item cannot be stolen, scrapped, or removed in other ways.
        /// </summary>
        CantRemove = 1 << 1,
    }
}