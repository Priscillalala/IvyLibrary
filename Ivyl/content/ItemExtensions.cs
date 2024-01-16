using System;
using RoR2;
using UnityEngine;
using HG;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System.Collections;
using RoR2.ContentManagement;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="ItemDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class ItemExtensions
    {
        /// <summary>
        /// Set the tier of this item to a vanilla <see cref="ItemTier"/>.
        /// </summary>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemDef SetItemTier<TItemDef>(this TItemDef itemDef, ItemTier itemTier) where TItemDef : ItemDef
        {
#pragma warning disable
            itemDef.deprecatedTier = itemTier;
#pragma warning restore
            itemDef._itemTierDef = null;
            return itemDef;
        }

        /// <summary>
        /// Set the tier of this item to any <see cref="ItemTierDef"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="SetItemTier{TItemDef}(TItemDef, ItemTier)"/> is preferred if this item is part of a vanilla <see cref="ItemTier"/>.
        /// </remarks>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemDef SetItemTier<TItemDef>(this TItemDef itemDef, ItemTierDef itemTier) where TItemDef : ItemDef
        {
            itemDef._itemTierDef = itemTier;
            return itemDef;
        }

        /// <summary>
        /// Set the icon sprite of this item.
        /// </summary>
        /// <remarks>
        /// Item icons are usually 128px.
        /// </remarks>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemDef SetIconSprite<TItemDef>(this TItemDef itemDef, Sprite iconSprite) where TItemDef : ItemDef
        {
            itemDef.pickupIconSprite = iconSprite;
            return itemDef;
        }

        /// <summary>
        /// Set the physical model of this item in the world, and parameters for that model in the logbook.
        /// </summary>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemDef SetPickupModelPrefab<TItemDef>(this TItemDef itemDef, GameObject pickupModelPrefab, ModelPanelParams logbookModelParams) where TItemDef : ItemDef
        {
            itemDef.pickupModelPrefab = pickupModelPrefab;
            Ivyl.SetupModelPanelParameters(pickupModelPrefab, logbookModelParams);
            return itemDef;
        }

        /// <summary>
        /// Set the physical model of this item in the world.
        /// </summary>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemDef SetPickupModelPrefab<TItemDef>(this TItemDef itemDef, GameObject pickupModelPrefab) where TItemDef : ItemDef
        {
            itemDef.pickupModelPrefab = pickupModelPrefab;
            return itemDef;
        }

        /// <summary>
        /// Set an <see cref="ItemTag"/> list that is used to categorize this item in different situations (e.g., category chests).
        /// </summary>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItemDef SetTags<TItemDef>(this TItemDef itemDef, params ItemTag[] tags) where TItemDef : ItemDef
        {
            itemDef.tags = tags;
            return itemDef;
        }

        /// <summary>
        /// Set the boolean values of this item with <see cref="ItemFlags"/>.
        /// </summary>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        public static TItemDef SetFlags<TItemDef>(this TItemDef itemDef, ItemFlags flags) where TItemDef : ItemDef
        {
            itemDef.hidden = (flags & ItemFlags.Hidden) > ItemFlags.None;
            itemDef.canRemove = (flags & ItemFlags.CantRemove) <= ItemFlags.None;
            return itemDef;
        }

        /// <summary>
        /// Asynchronously set a list of items that this void item will corrupt.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> to be yielded in an <see cref="IContentPackProvider"/>.</returns>
        public static IEnumerator SetItemsToCorruptAsync(this ItemDef itemDef, params ItemDef[] itemsToCorrupt)
        {
            var ContagiousItemProvider = Addressables.LoadAssetAsync<ItemRelationshipProvider>("RoR2/DLC1/Common/ContagiousItemProvider.asset");
            if (!ContagiousItemProvider.IsDone)
            {
                yield return ContagiousItemProvider;
            }
            SetItemsToCorruptImpl(itemDef, itemsToCorrupt, ContagiousItemProvider.Result);
        }

        /// <summary>
        /// Immediately set a list of items that this void item will corrupt.
        /// </summary>
        /// <remarks>
        /// This method will block the main thread until completed. <see cref="SetItemsToCorruptAsync(ItemDef, ItemDef[])"/> should be used instead.
        /// </remarks>
        /// <returns><paramref name="itemDef"/>, to continue a method chain.</returns>
        [Obsolete($"{nameof(SetItemsToCorrupt)} is not asynchronous and may stall loading. {nameof(SetItemsToCorruptAsync)} is preferred.", false)]
        public static TItemDef SetItemsToCorrupt<TItemDef>(this TItemDef itemDef, params ItemDef[] itemsToCorrupt) where TItemDef : ItemDef
        {
            SetItemsToCorruptImpl(itemDef, itemsToCorrupt, Addressables.LoadAssetAsync<ItemRelationshipProvider>("RoR2/DLC1/Common/ContagiousItemProvider.asset").WaitForCompletion());
            return itemDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetItemsToCorruptImpl(ItemDef itemDef, ItemDef[] itemsToCorrupt, ItemRelationshipProvider ContagiousItemProvider)
        {
            for (int i = ContagiousItemProvider.relationships.Length - 1; i >= 0; i--)
            {
                if (ContagiousItemProvider.relationships[i].itemDef2 == itemDef)
                {
                    ArrayUtils.ArrayRemoveAtAndResize(ref ContagiousItemProvider.relationships, i);
                }
            }
            for (int i = 0; i < itemsToCorrupt.Length; i++)
            {
                ArrayUtils.ArrayAppend(ref ContagiousItemProvider.relationships, new ItemDef.Pair
                {
                    itemDef1 = itemsToCorrupt[i],
                    itemDef2 = itemDef
                });
            }
        }
    }
}