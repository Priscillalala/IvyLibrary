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

        /*public static TItemDef SetRequiredUnlockable<TItemDef>(this TItemDef itemDef, UnlockableDef requiredUnlockable) where TItemDef : ItemDef
        {
            itemDef.unlockableDef = requiredUnlockable;
            return itemDef;
        }*/

        public static TItemDef SetItemsToCorrupt<TItemDef>(this TItemDef itemDef, params ItemDef[] itemsToCorrupt) where TItemDef : ItemDef
        {
            ItemRelationshipProvider contagiousItemProvider = Addressables.LoadAssetAsync<ItemRelationshipProvider>("RoR2/DLC1/Common/ContagiousItemProvider.asset").WaitForCompletion();
            for (int i = contagiousItemProvider.relationships.Length - 1; i >= 0; i--)
            {
                if (contagiousItemProvider.relationships[i].itemDef2 == itemDef)
                {
                    ArrayUtils.ArrayRemoveAtAndResize(ref contagiousItemProvider.relationships, i);
                }
            }
            for (int i = 0; i < itemsToCorrupt.Length; i++)
            {
                ArrayUtils.ArrayAppend(ref contagiousItemProvider.relationships, new ItemDef.Pair
                {
                    itemDef1 = itemsToCorrupt[i],
                    itemDef2 = itemDef
                });
            }
            return itemDef;
        }
    }
}