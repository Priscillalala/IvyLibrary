using RoR2;

namespace IvyLibrary
{
    /// <summary>
    /// Stat bonus values for an <see cref="EliteDef"/>.
    /// </summary>
    public struct EliteStats
    {
        /// <summary>
        /// Standard stat boosts for Tier 1 elites (e.g., Blazing, Overloading).
        /// </summary>
        /// <remarks>
        /// Tier 1 elites have 4.0x health (3.0x for Mending elites) and 2.0x damage.
        /// </remarks>
        public static readonly EliteStats TierOne = new EliteStats(4f, 2f);
        /// <summary>
        /// Standard, weaker stat boosts for Tier 1 elites when the Honor artifact is enabled.
        /// </summary>
        /// <remarks>
        /// Honor elites have 2.5x health and 1.5x damage.
        /// </remarks>
        public static readonly EliteStats Honor = new EliteStats(2.5f, 1.5f);
        /// <summary>
        /// Standard stat boosts for Tier 2 elites (e.g., Malachite, Celestine).
        /// </summary>
        /// <remarks>
        /// Tier 2 elites have 18.0x health and 6.0x damage.
        /// </remarks>
        public static readonly EliteStats TierTwo = new EliteStats(18f, 6f);
        /// <summary>
        /// Standard stat boosts for Lunar elites (e.g., Perfected).
        /// </summary>
        /// <remarks>
        /// Perfected elites have 2.0x health and 2.0x damage.
        /// </remarks>
        public static readonly EliteStats Lunar = new EliteStats(2f, 2f);

        /// <summary>
        /// A health multiplier for elites, applied in increments of 0.1;
        /// </summary>
        /// <remarks>
        /// 1.0 would be base health.
        /// </remarks>
        public float healthBoostCoefficient;
        /// <summary>
        /// A damage multiplier for elites, applied in increments of 0.1;
        /// </summary>
        /// <remarks>
        /// 1.0 would be base damage.
        /// </remarks>
        public float damageBoostCoefficient;

        public EliteStats(float healthBoostCoefficient, float damageBoostCoefficient)
        {
            this.healthBoostCoefficient = healthBoostCoefficient;
            this.damageBoostCoefficient = damageBoostCoefficient;
        }
    }
}