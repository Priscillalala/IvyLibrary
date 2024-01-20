using RoR2;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="ItemRelationshipProvider"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class ItemRelationshipProviderExtensions
    {
        /// <summary>
        /// Set the <see cref="ItemRelationshipType"/> of this item relationship provider.
        /// </summary>
        /// <returns><paramref name="itemRelationshipProvider"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemRelationshipProvider SetRelationshipType<TItemRelationshipProvider>(this TItemRelationshipProvider itemRelationshipProvider, ItemRelationshipType relationshipType) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            itemRelationshipProvider.relationshipType = relationshipType;
            return itemRelationshipProvider;
        }

        /// <summary>
        /// Set the relationship pairs of this item relationship provider..
        /// </summary>
        /// <returns><paramref name="itemRelationshipProvider"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemRelationshipProvider SetRelationships<TItemRelationshipProvider>(this TItemRelationshipProvider itemRelationshipProvider, params ItemDef.Pair[] relationships) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            itemRelationshipProvider.relationships = relationships;
            return itemRelationshipProvider;
        }
    }
}