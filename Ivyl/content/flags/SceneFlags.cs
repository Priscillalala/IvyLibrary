using System;

namespace IvyLibrary
{
    /// <summary>
    /// Flags to represent the boolean values of a scene.
    /// </summary>
    [Flags]
    public enum SceneFlags
    {
        /// <inheritdoc cref="BuffFlags.None"/>
        None,
        Offline = 1 << 0,
        /// <summary>
        /// This scene will not appear in the logbook.
        /// </summary>
        ExcludeFromLogbook = 1 << 1,
        /// <summary>
        /// Prevent players from spawning in this scene; useful for cutscenes and similar.
        /// </summary>
        SuppressPlayerEntry = 1 << 2,
        /// <summary>
        /// Prevent persistent non-player characters such as drones from spawning in this scene.
        /// </summary>
        SuppressNPCEntry = 1 << 3,
        /// <summary>
        /// Prevent Captain from using orbital skills in this scene.
        /// </summary>
        BlockOrbitalSkills = 1 << 4,
        /// <summary>
        /// This scene will not appear when random stage order is enabled (e.g., in Prismatic Trials).
        /// </summary>
        NeverRandomlySelected = 1 << 5,
    }
}