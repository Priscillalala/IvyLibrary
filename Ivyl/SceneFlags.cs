using System;

namespace Ivyl
{
    [Flags]
    public enum SceneFlags
    {
        None,
        Offline = 1 << 0,
        ExcludeFromLogbook = 1 << 1,
        SuppressPlayerEntry = 1 << 2,
        SuppressNPCEntry = 1 << 3,
        BlockOrbitalSkills = 1 << 4,
        NeverRandomlySelected = 1 << 5,
    }
}