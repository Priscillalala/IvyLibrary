using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="SurvivorDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class SurvivorExtensions
    {
        /// <summary>
        /// Set body prefab of this survivor.
        /// </summary>
        /// <returns><paramref name="survivorDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurvivorDef SetBodyPrefab<TSurvivorDef>(this TSurvivorDef survivorDef, GameObject bodyPrefab) where TSurvivorDef : SurvivorDef
        {
            survivorDef.bodyPrefab = bodyPrefab;
            return survivorDef;
        }

        /// <summary>
        /// Set the display prefab for this survivor in the lobby.
        /// </summary>
        /// <returns><paramref name="survivorDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurvivorDef SetDisplayPrefab<TSurvivorDef>(this TSurvivorDef survivorDef, GameObject displayPrefab) where TSurvivorDef : SurvivorDef
        {
            survivorDef.displayPrefab = displayPrefab;
            return survivorDef;
        }

        /// <summary>
        /// Set the primary color of this survivor.
        /// </summary>
        /// <returns><paramref name="survivorDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurvivorDef SetPrimaryColor<TSurvivorDef>(this TSurvivorDef survivorDef, Color primaryColor) where TSurvivorDef : SurvivorDef
        {
            survivorDef.primaryColor = primaryColor;
            return survivorDef;
        }

        /// <summary>
        /// Set the desired postition of this survivor in the character select screen.
        /// </summary>
        /// <returns><paramref name="survivorDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurvivorDef SetDesiredSortPosition<TSurvivorDef>(this TSurvivorDef survivorDef, float desiredSortPosition) where TSurvivorDef : SurvivorDef
        {
            survivorDef.desiredSortPosition = desiredSortPosition;
            return survivorDef;
        }

        /// <summary>
        /// Set the boolean values of this survivor with <see cref="SurvivorFlags"/>.
        /// </summary>
        /// <returns><paramref name="survivorDef"/>, to continue a method chain.</returns>
        public static TSurvivorDef SetFlags<TSurvivorDef>(this TSurvivorDef survivorDef, SurvivorFlags flags) where TSurvivorDef : SurvivorDef
        {
            survivorDef.hidden = (flags & SurvivorFlags.Hidden) > SurvivorFlags.None;
            return survivorDef;
        }
    }
}