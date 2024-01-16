using UnityEngine;
using RoR2.ExpansionManagement;
using RoR2;
using RoR2.EntitlementManagement;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="ExpansionDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class ExpansionExtensions
    {
        /// <summary>
        /// Set an <see cref="EntitlementDef"/> that is required to enable this expansion (e.g., owning the first DLC to enable Survivors of the Void).
        /// </summary>
        /// <returns><paramref name="expansionDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TExpansionDef SetRequiredEntitlement<TExpansionDef>(this TExpansionDef expansionDef, EntitlementDef requiredEntitlement) where TExpansionDef : ExpansionDef
        {
            expansionDef.requiredEntitlement = requiredEntitlement;
            return expansionDef;
        }

        /// <summary>
        /// Set the icon sprite of this expansion in the lobby.
        /// </summary>
        /// <remarks>
        /// The Survivors of the Void icon sprite is 512px.
        /// </remarks>
        /// <returns><paramref name="expansionDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TExpansionDef SetIconSprite<TExpansionDef>(this TExpansionDef expansionDef, Sprite iconSprite) where TExpansionDef : ExpansionDef
        {
            expansionDef.iconSprite = iconSprite;
            return expansionDef;
        }

        /// <summary>
        /// Set a networked object to be attached to the <see cref="Run"/> when this expansion is enabled.
        /// </summary>
        /// <returns><paramref name="expansionDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TExpansionDef SetRunBehaviorPrefab<TExpansionDef>(this TExpansionDef expansionDef, GameObject runBehaviorPrefab) where TExpansionDef : ExpansionDef
        {
            expansionDef.runBehaviorPrefab = runBehaviorPrefab;
            return expansionDef;
        }
    }
}