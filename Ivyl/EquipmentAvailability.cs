namespace IvyLibrary
{
    /// <summary>
    /// The availablity of an equipment in the drop pool.
    /// </summary>
    public enum EquipmentAvailability
    {
        /// <summary>
        /// This equipment is always available in the standard drop pool.
        /// </summary>
        Always,
        /// <summary>
        /// This equipment is only available during singleplayer games.
        /// </summary>
        SingleplayerExclusive,
        /// <summary>
        /// This equipment is only available during multiplayer games.
        /// </summary>
        MultiplayerExclusive,
        /// <summary>
        /// This equipment is never available through the standard drop pool.
        /// </summary>
        Never
    }
}