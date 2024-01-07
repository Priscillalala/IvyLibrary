using System;

namespace IvyLibrary
{
    [Flags]
    public enum SurfaceFlags
    {
        None,
        Slippery = 1 << 0,
    }
}