using System;

namespace Ivyl
{
    [Flags]
    public enum SurfaceFlags
    {
        None,
        Slippery = 1 << 0,
    }
}