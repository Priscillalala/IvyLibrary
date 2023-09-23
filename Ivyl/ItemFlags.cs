using System;

namespace Ivyl
{
    [Flags]
    public enum ItemFlags
    {
        None,
        Hidden = 1 << 0,
        CantRemove = 1 << 1,
    }
}