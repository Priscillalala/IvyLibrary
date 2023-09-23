﻿using System;

namespace Ivyl
{
    [Flags]
    public enum ItemTierFlags
    {
        None,
        NoDrop = 1 << 0,
        NoScrap = 1 << 1,
        NoRestack = 1 << 2,
    }
}