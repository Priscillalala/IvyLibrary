namespace IvyLibrary
{
    public struct EliteStats
    {
        public static readonly EliteStats TierOne = new EliteStats(4f, 2f);
        public static readonly EliteStats Honor = new EliteStats(2.5f, 1.5f);
        public static readonly EliteStats TierTwo = new EliteStats(18f, 6f);
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