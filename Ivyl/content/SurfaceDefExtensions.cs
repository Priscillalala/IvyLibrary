using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="SurfaceDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class SurfaceDefExtensions
    {
        /// <summary>
        /// Set the approximate color of this surface.
        /// </summary>
        /// <returns><paramref name="surfaceDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurfaceDef SetApproximateColor<TSurfaceDef>(this TSurfaceDef surfaceDef, Color approximateColor) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.approximateColor = approximateColor;
            return surfaceDef;
        }

        /// <summary>
        /// Set an effect prefab to be spawned on impact with this surface.
        /// </summary>
        /// <returns><paramref name="surfaceDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurfaceDef SetImpactEffectPrefab<TSurfaceDef>(this TSurfaceDef surfaceDef, GameObject impactEffectPrefab) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.impactEffectPrefab = impactEffectPrefab;
            return surfaceDef;
        }

        /// <summary>
        /// Set an effect prefab to be spawned from footsteps on this surface.
        /// </summary>
        /// <returns><paramref name="surfaceDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurfaceDef SetFootstepEffectPrefab<TSurfaceDef>(this TSurfaceDef surfaceDef, GameObject footstepEffectPrefab) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.footstepEffectPrefab = footstepEffectPrefab;
            return surfaceDef;
        }

        /// <summary>
        /// Set an sound event to be played on impact with this surface.
        /// </summary>
        /// <returns><paramref name="surfaceDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurfaceDef SetImpactSoundString<TSurfaceDef>(this TSurfaceDef surfaceDef, string impactSoundString) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.impactSoundString = impactSoundString;
            return surfaceDef;
        }

        /// <summary>
        /// Set a switch state string to control the sound of this surface (e.g., "dirt", "stone", "snow", "metal").
        /// </summary>
        /// <returns><paramref name="surfaceDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSurfaceDef SetMaterialSwitchString<TSurfaceDef>(this TSurfaceDef surfaceDef, string materialSwitchString) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.materialSwitchString = materialSwitchString;
            return surfaceDef;
        }

        /// <summary>
        /// Set the boolean values of this surface with <see cref="SurfaceFlags"/>.
        /// </summary>
        /// <returns><paramref name="surfaceDef"/>, to continue a method chain.</returns>
        public static TSurfaceDef SetFlags<TSurfaceDef>(this TSurfaceDef surfaceDef, SurfaceFlags flags) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.isSlippery = (flags & SurfaceFlags.Slippery) > SurfaceFlags.None;
            return surfaceDef;
        }
    }
}