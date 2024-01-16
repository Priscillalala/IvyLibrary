using System;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="EquipmentDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class EquipmentExtensions
    {
        /// <summary>
        /// Set the icon sprite of this equipment.
        /// </summary>
        /// <remarks>
        /// Equipment icons are usually 128px.
        /// </remarks>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEquipmentDef SetIconSprite<TEquipmentDef>(this TEquipmentDef equipmentDef, Sprite iconSprite) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.pickupIconSprite = iconSprite;
            return equipmentDef;
        }

        /// <summary>
        /// Set the cooldon of this equipment, in seconds.
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEquipmentDef SetCooldown<TEquipmentDef>(this TEquipmentDef equipmentDef, float cooldown) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.cooldown = cooldown;
            return equipmentDef;
        }

        /// <summary>
        /// Set the physical model of this equipment in the world, and parameters for that model in the logbook.
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEquipmentDef SetPickupModelPrefab<TEquipmentDef>(this TEquipmentDef equipmentDef, GameObject pickupModelPrefab, ModelPanelParams logbookModelParams) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.pickupModelPrefab = pickupModelPrefab;
            Ivyl.SetupModelPanelParameters(pickupModelPrefab, logbookModelParams);
            return equipmentDef;
        }

        /// <summary>
        /// Set the physical model of this equipment in the world.
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEquipmentDef SetPickupModelPrefab<TEquipmentDef>(this TEquipmentDef equipmentDef, GameObject pickupModelPrefab) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.pickupModelPrefab = pickupModelPrefab;
            return equipmentDef;
        }

        /// <summary>
        /// Set the boolean values of this equipment with <see cref="EquipmentFlags"/>.
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        public static TEquipmentDef SetFlags<TEquipmentDef>(this TEquipmentDef equipmentDef, EquipmentFlags flags) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.canBeRandomlyTriggered = (flags & EquipmentFlags.NeverRandomlyTriggered) <= EquipmentFlags.None;
            equipmentDef.enigmaCompatible = (flags & EquipmentFlags.EnigmaIncompatible) <= EquipmentFlags.None;
            return equipmentDef;
        }

        /// <summary>
        /// Set a buff to be passively granted while this equipment is held (e.g., elite aspect buffs).
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEquipmentDef SetPassiveBuff<TEquipmentDef>(this TEquipmentDef equipmentDef, BuffDef passiveBuff) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.passiveBuffDef = passiveBuff;
            return equipmentDef;
        }

        /// <summary>
        /// Set the drop table availablity of this equipment.
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        public static TEquipmentDef SetAvailability<TEquipmentDef>(this TEquipmentDef equipmentDef, EquipmentAvailability availability) where TEquipmentDef : EquipmentDef
        {
            equipmentDef.canDrop = availability != EquipmentAvailability.Never;
            equipmentDef.appearsInSinglePlayer = availability != EquipmentAvailability.MultiplayerExclusive;
            equipmentDef.appearsInMultiPlayer = availability != EquipmentAvailability.SingleplayerExclusive;
            return equipmentDef;
        }

        /// <summary>
        /// Set the <see cref="EquipmentType"/> of this equipment.
        /// </summary>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
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

        /// <summary>
        /// Set a callback to be invoked when this equipment is activated.
        /// </summary>
        /// <remarks>
        /// <paramref name="activationFunction"/> should return true if the equipment could be successfully activated, otherwise, false.
        /// </remarks>
        /// <returns><paramref name="equipmentDef"/>, to continue a method chain.</returns>
        public static TEquipmentDef SetActivationFunction<TEquipmentDef>(this TEquipmentDef equipmentDef, Func<EquipmentSlot, bool> activationFunction) where TEquipmentDef : EquipmentDef
        {
            if (equipmentActivationFunctions == null)
            {
                equipmentActivationFunctions = new Dictionary<EquipmentDef, Func<EquipmentSlot, bool>>();
                On.RoR2.EquipmentSlot.PerformEquipmentAction += PerformEquipmentAction;
            }
            equipmentActivationFunctions[equipmentDef] = activationFunction;
            return equipmentDef;

            static bool PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
            {
                if (equipmentActivationFunctions.TryGetValue(equipmentDef, out Func<EquipmentSlot, bool> activationFunction))
                {
                    return activationFunction != null && activationFunction(self);
                }
                return orig(self, equipmentDef);
            }
        }
    }
}