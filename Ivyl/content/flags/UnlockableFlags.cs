using System;

namespace Ivyl
{
    [Flags]
    public enum UnlockableFlags
    {
        None,
        Hidden = 1 << 0,
    }
}