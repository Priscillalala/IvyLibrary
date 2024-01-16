using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="ItemTierDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class ItemTierExtensions
    {
        /// <summary>
        /// Set the background texture of this item tier in the logbook.
        /// </summary>
        /// <remarks>
        /// Item tier backgrounds are usually 512px.
        /// </remarks>
        /// <returns><paramref name="itemTierDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemTierDef SetBackgroundTexture<TItemTierDef>(this TItemTierDef itemTierDef, Texture backgroundTexture) where TItemTierDef : ItemTierDef
        {
            itemTierDef.bgIconTexture = backgroundTexture;
            return itemTierDef;
        }


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Set the colors of this item tier.
        /// </summary>
        /// <remarks>
        /// Custom colors can be registered with <see cref="R2API.ColorsAPI"/>.
        /// </remarks>
        /// <returns><paramref name="itemTierDef"/>, to continue a method chain.</returns>
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemTierDef SetColors<TItemTierDef>(this TItemTierDef itemTierDef, ColorCatalog.ColorIndex baseColor, ColorCatalog.ColorIndex darkColor) where TItemTierDef : ItemTierDef
        {
            itemTierDef.colorIndex = baseColor;
            itemTierDef.darkColorIndex = darkColor;
            return itemTierDef;
        }

        /// <summary>
        /// Set the boolean values of this item tier with <see cref="ItemTierFlags"/>.
        /// </summary>
        /// <returns><paramref name="itemTierDef"/>, to continue a method chain.</returns>
        public static TItemTierDef SetFlags<TItemTierDef>(this TItemTierDef itemTierDef, ItemTierFlags flags) where TItemTierDef : ItemTierDef
        {
            itemTierDef.isDroppable = (flags & ItemTierFlags.NoDrop) <= ItemTierFlags.None;
            itemTierDef.canScrap = (flags & ItemTierFlags.NoScrap) <= ItemTierFlags.None;
            itemTierDef.canRestack = (flags & ItemTierFlags.NoRestack) <= ItemTierFlags.None;
            return itemTierDef;
        }

        /// <summary>
        /// Set the pickup rules for items in this item tier.
        /// </summary>
        /// <returns><paramref name="itemTierDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemTierDef SetPickupRules<TItemTierDef>(this TItemTierDef itemTierDef, ItemTierDef.PickupRules pickupRules) where TItemTierDef : ItemTierDef
        {
            itemTierDef.pickupRules = pickupRules;
            return itemTierDef;
        }

        /// <summary>
        /// Set the highlight prefab for pickups in this item tier.
        /// </summary>
        /// <returns><paramref name="itemTierDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemTierDef SetHighlightPrefab<TItemTierDef>(this TItemTierDef itemTierDef, GameObject highlightPrefab) where TItemTierDef : ItemTierDef
        {
            itemTierDef.highlightPrefab = highlightPrefab;
            return itemTierDef;
        }

        /// <summary>
        /// Set the droplet prefab for pickups in this item tier.
        /// </summary>
        /// <returns><paramref name="itemTierDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemTierDef SetDropletDisplayPrefab<TItemTierDef>(this TItemTierDef itemTierDef, GameObject dropletDisplayPrefab) where TItemTierDef : ItemTierDef
        {
            itemTierDef.dropletDisplayPrefab = dropletDisplayPrefab;
            return itemTierDef;
        }
    }
}