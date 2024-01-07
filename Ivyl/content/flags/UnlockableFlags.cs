using System;

namespace IvyLibrary
{
    [Flags]
    public enum UnlockableFlags
    {
        None,
        Hidden = 1 << 0,
    }
}