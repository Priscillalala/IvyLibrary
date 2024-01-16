using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="MiscPickupDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class MiscPickupExtensions
    {
        /// <summary>
        /// Set a coin value to be granted by this pickup implementation (e.g., <see cref="LunarCoinDef"/>, <see cref="VoidCoinDef"/>).
        /// </summary>
        /// <returns><paramref name="miscPickupDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMiscPickupDef SetCoinValue<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, uint coinValue) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.coinValue = coinValue;
            return miscPickupDef;
        }

        /// <summary>
        /// Set the physical model of this pickup in the world.
        /// </summary>
        /// <returns><paramref name="miscPickupDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMiscPickupDef SetDisplayPrefab<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, GameObject displayPrefab) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.displayPrefab = displayPrefab;
            return miscPickupDef;
        }

        /// <summary>
        /// Set the droplet prefab of this pickup.
        /// </summary>
        /// <returns><paramref name="miscPickupDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TMiscPickupDef SetDropletDisplayPrefab<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, GameObject dropletDisplayPrefab) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.dropletDisplayPrefab = dropletDisplayPrefab;
            return miscPickupDef;
        }


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Set the colors of this pickup.
        /// </summary>
        /// <remarks>
        /// Custom colors can be registered with <see cref="R2API.ColorsAPI"/>.
        /// </remarks>
        /// <returns><paramref name="miscPickupDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
        public static TMiscPickupDef SetColors<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, ColorCatalog.ColorIndex baseColor, ColorCatalog.ColorIndex darkColor) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.baseColor = baseColor;
            miscPickupDef.darkColor = darkColor;
            return miscPickupDef;
        }
    }
}