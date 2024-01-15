using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of an equipment.
    /// </summary>
    [Flags]
    public enum EquipmentFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        /// <summary>
        /// This equipment cannot be randomly triggered; relevant for Bottled Chaos.
        /// </summary>
        NeverRandomlyTriggered = 1 << 0,
        /// <summary>
        /// This equipment will not appear in the Artifact of Enigma equipment pool.
        /// </summary>
        EnigmaIncompatible = 1 << 1,
    }
}