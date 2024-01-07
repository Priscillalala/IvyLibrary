using R2API;
using RoR2;

namespace IvyLibrary
{
    public static class EliteTiers
    {
        public static CombatDirector.EliteTierDef TierOne => EliteAPI.VanillaEliteTiers[1];
        public static CombatDirector.EliteTierDef Honor => EliteAPI.VanillaEliteTiers[2];
        public static CombatDirector.EliteTierDef TierTwo => EliteAPI.VanillaEliteTiers[3];
        public static CombatDirector.EliteTierDef Lunar => EliteAPI.VanillaEliteTiers[4];
    }
}