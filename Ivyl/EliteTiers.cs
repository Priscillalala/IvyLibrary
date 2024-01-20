using R2API;
using RoR2;

namespace IvyLibrary
{
    /// <summary>
    /// Exposes elite tiers through <see cref="EliteAPI"/>.
    /// </summary>
    public static class EliteTiers
    {
        /// <summary>
        /// Standard elites (e.g., Blazing, Overloading).
        /// </summary>
        public static CombatDirector.EliteTierDef TierOne => EliteAPI.VanillaEliteTiers[1];
        /// <summary>
        /// Weaker versions of Tier 1 elites that spawn when the Artifact of Honor is enabled.
        /// </summary>
        public static CombatDirector.EliteTierDef Honor => EliteAPI.VanillaEliteTiers[2];
        /// <summary>
        /// Powerful elites that only appear past the first loop (e.g., Malachite, Celestine).
        /// </summary>
        public static CombatDirector.EliteTierDef TierTwo => EliteAPI.VanillaEliteTiers[3];
        /// <summary>
        /// Elites that spawn on the moon (e.g., Perfected elites).
        /// </summary>
        public static CombatDirector.EliteTierDef Lunar => EliteAPI.VanillaEliteTiers[4];
    }
}