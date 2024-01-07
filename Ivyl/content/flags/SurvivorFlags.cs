using System;

namespace Ivyl
{
    [Flags]
    public enum SurvivorFlags
    {
        None,
        Hidden = 1 << 0,
    }
}