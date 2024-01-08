using RoR2;
using UnityEngine;

namespace IvyLibrary
{
    public static class ItemTierExtensions
    {
        public static TItemTierDef SetBackgroundTexture<TItemTierDef>(this TItemTierDef itemTierDef, Texture backgroundTexture) where TItemTierDef : ItemTierDef
        {
            itemTierDef.bgIconTexture = backgroundTexture;
            return itemTierDef;
        }

        public static TItemTierDef SetColors<TItemTierDef>(this TItemTierDef itemTierDef, ColorCatalog.ColorIndex baseColor, ColorCatalog.ColorIndex darkColor) where TItemTierDef : ItemTierDef
        {
            itemTierDef.colorIndex = baseColor;
            itemTierDef.darkColorIndex = darkColor;
            return itemTierDef;
        }

        public static TItemTierDef SetFlags<TItemTierDef>(this TItemTierDef itemTierDef, ItemTierFlags flags) where TItemTierDef : ItemTierDef
        {
            itemTierDef.isDroppable = (flags & ItemTierFlags.NoDrop) <= ItemTierFlags.None;
            itemTierDef.canScrap = (flags & ItemTierFlags.NoScrap) <= ItemTierFlags.None;
            itemTierDef.canRestack = (flags & ItemTierFlags.NoRestack) <= ItemTierFlags.None;
            return itemTierDef;
        }

        public static TItemTierDef SetPickupRules<TItemTierDef>(this TItemTierDef itemTierDef, ItemTierDef.PickupRules pickupRules) where TItemTierDef : ItemTierDef
        {
            itemTierDef.pickupRules = pickupRules;
            return itemTierDef;
        }

        public static TItemTierDef SetHighlightPrefab<TItemTierDef>(this TItemTierDef itemTierDef, GameObject highlightPrefab) where TItemTierDef : ItemTierDef
        {
            itemTierDef.highlightPrefab = highlightPrefab;
            return itemTierDef;
        }

        public static TItemTierDef SetDropletDisplayPrefab<TItemTierDef>(this TItemTierDef itemTierDef, GameObject dropletDisplayPrefab) where TItemTierDef : ItemTierDef
        {
            itemTierDef.dropletDisplayPrefab = dropletDisplayPrefab;
            return itemTierDef;
        }
    }
}