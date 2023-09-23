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
using System.Collections.Generic;

namespace Ivyl
{
    public static class EquipmentExtensions
    {
        public static TEquipmentDef SetIconSprite<TEquipmentDef>(this TEquipmentDef equipmentDef, Sprite iconSprite) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.pickupIconSprite = iconSprite;
            return equipmentDef;
        }

        public static TEquipmentDef SetCooldown<TEquipmentDef>(this TEquipmentDef equipmentDef, float cooldown) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.cooldown = cooldown;
            return equipmentDef;
        }

        public static TEquipmentDef SetPickupModelPrefab<TEquipmentDef>(this TEquipmentDef equipmentDef, GameObject pickupModelPrefab, ModelPanelParams logbookModelParams) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.pickupModelPrefab = pickupModelPrefab;
            IvyLibrary.SetupModelPanelParameters(pickupModelPrefab, logbookModelParams);
            return equipmentDef;
        }

        public static TEquipmentDef SetPickupModelPrefab<TEquipmentDef>(this TEquipmentDef equipmentDef, GameObject pickupModelPrefab) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.pickupModelPrefab = pickupModelPrefab;
            return equipmentDef;
        }

        public static TEquipmentDef SetFlags<TEquipmentDef>(this TEquipmentDef equipmentDef, EquipmentFlags flags) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.canBeRandomlyTriggered = (flags & EquipmentFlags.NeverRandomlyTriggered) <= EquipmentFlags.None;
            equipmentDef.enigmaCompatible = (flags & EquipmentFlags.EnigmaIncompatible) <= EquipmentFlags.None;
            return equipmentDef;
        }

        /*public static TEquipmentDef SetRequiredUnlockable<TEquipmentDef>(this TEquipmentDef equipmentDef, UnlockableDef requiredUnlockable) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.unlockableDef = requiredUnlockable;
            return equipmentDef;
        }*/

        public static TEquipmentDef SetPassiveBuff<TEquipmentDef>(this TEquipmentDef equipmentDef, BuffDef passiveBuff) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.passiveBuffDef = passiveBuff;
            return equipmentDef;
        }

        public static TEquipmentDef SetAvailability<TEquipmentDef>(this TEquipmentDef equipmentDef, EquipmentAvailability availability) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.canDrop = availability != EquipmentAvailability.Never;
            equipmentDef.appearsInSinglePlayer = availability == EquipmentAvailability.Always || availability == EquipmentAvailability.SingleplayerExclusive;
            equipmentDef.appearsInMultiPlayer = availability == EquipmentAvailability.Always || availability == EquipmentAvailability.MultiplayerExclusive;
            return equipmentDef;
        }

        public static TEquipmentDef SetEquipmentType<TEquipmentDef>(this TEquipmentDef equipmentDef, EquipmentType equipmentType) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.isLunar = equipmentType == EquipmentType.Lunar;
            equipmentDef.isBoss = equipmentType == EquipmentType.Boss;
            equipmentDef.colorIndex = equipmentType switch
            {
                EquipmentType.Lunar => ColorCatalog.ColorIndex.LunarItem,
                EquipmentType.Boss => ColorCatalog.ColorIndex.BossItem,
                _ => ColorCatalog.ColorIndex.Equipment
            };
            return equipmentDef;
        }

        private static Dictionary<EquipmentDef, Func<EquipmentSlot, bool>> equipmentActivationFunctions;

        public static TEquipmentDef SetActivationFunction<TEquipmentDef>(this TEquipmentDef equipmentDef, Func<EquipmentSlot, bool> activationFunction) where TEquipmentDef : EquipmentDef
        {
            if (equipmentActivationFunctions == null)
            {
                equipmentActivationFunctions = new Dictionary<EquipmentDef, Func<EquipmentSlot, bool>>();
                On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            }
            equipmentActivationFunctions[equipmentDef] = activationFunction;
            return equipmentDef;
        }

        private static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentActivationFunctions.TryGetValue(equipmentDef, out Func<EquipmentSlot, bool> activationFunction))
            {
                return activationFunction != null && activationFunction(self);
            }
            return orig(self, equipmentDef);
        }
    }
}