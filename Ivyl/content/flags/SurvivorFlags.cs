using System;

namespace IvyLibrary
{
    [Flags]
    public enum SurvivorFlags
    {
        None,
        Hidden = 1 << 0,
    }
}