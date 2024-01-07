using System;

namespace IvyLibrary
{
    [Flags]
    public enum GameEndingFlags
    {
        None,
        Victory = 1 << 0,
        ShowCredits = 1 << 1,
    }
}