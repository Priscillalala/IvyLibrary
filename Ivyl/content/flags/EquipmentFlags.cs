using System;

namespace IvyLibrary
{
    [Flags]
    public enum EquipmentFlags
    {
        None,
        NeverRandomlyTriggered = 1 << 0,
        EnigmaIncompatible = 1 << 1,
    }
}