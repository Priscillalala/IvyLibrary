using System;

namespace Ivyl
{
    [Flags]
    public enum GameEndingFlags
    {
        None,
        Victory = 1 << 0,
        ShowCredits = 1 << 1,
    }
}