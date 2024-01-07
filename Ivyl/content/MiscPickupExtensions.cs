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
    public static class MiscPickupExtensions
    {
        public static TMiscPickupDef SetCoinValue<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, uint coinValue) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.coinValue = coinValue;
            return miscPickupDef;
        }

        public static TMiscPickupDef SetDisplayPrefab<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, GameObject displayPrefab) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.displayPrefab = displayPrefab;
            return miscPickupDef;
        }

        public static TMiscPickupDef SetDropletDisplayPrefab<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, GameObject dropletDisplayPrefab) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.dropletDisplayPrefab = dropletDisplayPrefab;
            return miscPickupDef;
        }

        public static TMiscPickupDef SetColors<TMiscPickupDef>(this TMiscPickupDef miscPickupDef, ColorCatalog.ColorIndex baseColor, ColorCatalog.ColorIndex darkColor) where TMiscPickupDef : MiscPickupDef
        {
            miscPickupDef.baseColor = baseColor;
            miscPickupDef.darkColor = darkColor;
            return miscPickupDef;
        }
    }
}