using System;

namespace IvyLibrary
{
    [Flags]
    public enum BuffFlags
    {
        None,
        Stackable = 1 << 0,
        Debuff = 1 << 1,
        Cooldown = 1 << 2,
        Hidden = 1 << 3,
    }
}