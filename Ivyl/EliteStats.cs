using RoR2;

namespace IvyLibrary
{
    /// <summary>
    /// Stat bonus values for an <see cref="EliteDef"/>.
    /// </summary>
    public struct EliteStats
    {
        /// <summary>
        /// Standard stat boosts for Tier 1 elites like Blazing and Overloading.
        /// </summary>
        public static readonly EliteStats TierOne = new EliteStats(4f, 2f);
        /// <summary>
        /// Standard, weaker stat boosts for Tier 1 elites when the Honor artifact is enabled.
        /// </summary>
        public static readonly EliteStats Honor = new EliteStats(2.5f, 1.5f);
        /// <summary>
        /// Standard stat boosts for Tier 2 elites like Malachite and Celestine.
        /// </summary>
        public static readonly EliteStats TierTwo = new EliteStats(18f, 6f);
        /// <summary>
        /// Standard stat boosts for Lunar elites like Perfected.
        /// </summary>
        public static readonly EliteStats Lunar = new EliteStats(2f, 2f);

        public float healthBoostCoefficient;
        public float damageBoostCoefficient;

        public EliteStats(float healthBoostCoefficient, float damageBoostCoefficient)
        {
            this.healthBoostCoefficient = healthBoostCoefficient;
            this.damageBoostCoefficient = damageBoostCoefficient;
        }
    }
}