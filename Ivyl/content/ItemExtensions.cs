using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.ContentManagement;
using HG;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System.Collections;

namespace IvyLibrary
{
    public static class ItemExtensions
    {
        public static TItemDef SetItemTier<TItemDef>(this TItemDef itemDef, ItemTier itemTier) where TItemDef : ItemDef
        {
#pragma warning disable
            itemDef.deprecatedTier = itemTier;
#pragma warning restore
            itemDef._itemTierDef = null;
            return itemDef;
        }

        public static TItemDef SetItemTier<TItemDef>(this TItemDef itemDef, ItemTierDef itemTier) where TItemDef : ItemDef
        {
            itemDef._itemTierDef = itemTier;
            return itemDef;
        }

        public static TItemDef SetIconSprite<TItemDef>(this TItemDef itemDef, Sprite iconSprite) where TItemDef : ItemDef
        {
            itemDef.pickupIconSprite = iconSprite;
            return itemDef;
        }

        public static TItemDef SetPickupModelPrefab<TItemDef>(this TItemDef itemDef, GameObject pickupModelPrefab, ModelPanelParams logbookModelParams) where TItemDef : ItemDef
        {
            itemDef.pickupModelPrefab = pickupModelPrefab;
            Ivyl.SetupModelPanelParameters(pickupModelPrefab, logbookModelParams);
            return itemDef;
        }

        public static TItemDef SetPickupModelPrefab<TItemDef>(this TItemDef itemDef, GameObject pickupModelPrefab) where TItemDef : ItemDef
        {
            itemDef.pickupModelPrefab = pickupModelPrefab;
            return itemDef;
        }

        public static TItemDef SetTags<TItemDef>(this TItemDef itemDef, params ItemTag[] tags) where TItemDef : ItemDef
        {
            itemDef.tags = tags;
            return itemDef;
        }

        public static TItemDef SetFlags<TItemDef>(this TItemDef itemDef, ItemFlags flags) where TItemDef : ItemDef
        {
            itemDef.hidden = (flags & ItemFlags.Hidden) > ItemFlags.None;
            itemDef.canRemove = (flags & ItemFlags.CantRemove) <= ItemFlags.None;
            return itemDef;
        }

        public static IEnumerator SetItemsToCorruptAsync(this ItemDef itemDef, params ItemDef[] itemsToCorrupt)
        {
            var ContagiousItemProvider = Addressables.LoadAssetAsync<ItemRelationshipProvider>("RoR2/DLC1/Common/ContagiousItemProvider.asset");
            if (!ContagiousItemProvider.IsDone)
            {
                yield return ContagiousItemProvider;
            }
            SetItemsToCorruptImpl(itemDef, itemsToCorrupt, ContagiousItemProvider.Result);
        }

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