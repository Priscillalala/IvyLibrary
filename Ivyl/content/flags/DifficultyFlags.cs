using System;

namespace IvyLibrary
{
    [Flags]
    public enum DifficultyFlags
    {
        None,
        HardMode = 1 << 0,
        Hidden = 1 << 1,
    }
}