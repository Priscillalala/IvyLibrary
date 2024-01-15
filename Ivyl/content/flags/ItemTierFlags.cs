using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of an item tier.
    /// </summary>
    [Flags]
    public enum ItemTierFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This item tier is not dropped in a standard way and associated items will be hidden from the logbook.
        /// </summary>
        NoDrop = 1 << 0,
        /// <summary>
        /// Items in this item tier cannot be scrapped.
        /// </summary>
        NoScrap = 1 << 1,
        /// <summary>
        /// This item tier is ignored by the Shrine of Order.
        /// </summary>
        NoRestack = 1 << 2,
    }
}