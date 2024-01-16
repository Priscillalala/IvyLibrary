using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using R2API;
using RoR2;
using System.Runtime.CompilerServices;
using RoR2.ContentManagement;

namespace IvyLibrary
{
    /// <summary>
    /// Associates an <see cref="EliteDef"/> with an elite <see cref="EquipmentDef"/> and elite <see cref="BuffDef"/>, and provides methods for manipulating them.
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public record class EliteWrapper(EliteDef EliteDef, List<EliteDef> SubEliteDefs, EquipmentDef EliteEquipmentDef, BuffDef EliteBuffDef)
        : EliteWrapper<EliteWrapper, EliteDef, EquipmentDef, BuffDef>(EliteDef, SubEliteDefs, EliteEquipmentDef, EliteBuffDef)
    { }

    /// <inheritdoc cref="EliteWrapper"/>
    public record class EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef>(TEliteDef EliteDef, List<TEliteDef> SubEliteDefs, TEquipmentDef EliteEquipmentDef, TBuffDef EliteBuffDef) 
        : EliteWrapper<EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef>, TEliteDef, TEquipmentDef, TBuffDef>(EliteDef, SubEliteDefs, EliteEquipmentDef, EliteBuffDef)
        where TEliteDef : EliteDef
        where TEquipmentDef : EquipmentDef
        where TBuffDef : BuffDef
    { }

    /// <inheritdoc cref="EliteWrapper"/>
    public abstract record class EliteWrapper<TEliteWrapper, TEliteDef, TEquipmentDef, TBuffDef>(TEliteDef EliteDef, List<TEliteDef> SubEliteDefs, TEquipmentDef EliteEquipmentDef, TBuffDef EliteBuffDef)
        where TEliteWrapper : EliteWrapper<TEliteWrapper, TEliteDef, TEquipmentDef, TBuffDef>
        where TEliteDef : EliteDef
        where TEquipmentDef : EquipmentDef
        where TBuffDef : BuffDef
    {
        /// <summary>
        /// A callback usually reserved for adding sub-elites to the relevant <see cref="ContentPack"/>.
        /// </summary>
        public Action<EliteDef> registerSubEliteCallback;
        /// <summary>
        /// A prefix for sub-elite names, usually derived from the identifier of the relevant <see cref="ContentPack"/>.
        /// </summary>
        public string subElitePrefix;
        private Texture2D _eliteRampTexture;

        /// <summary>
        /// Set the stat boost coefficients of this elite.
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEliteWrapper SetStats(EliteStats stats)
        {
            EliteDef.healthBoostCoefficient = stats.healthBoostCoefficient;
            EliteDef.damageBoostCoefficient = stats.damageBoostCoefficient;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Set the ramp texture of this elite.
        /// </summary>
        /// <remarks>
        /// Elite ramp textures are usually 256x8px.
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        public TEliteWrapper SetEliteRampTexture(Texture2D eliteRampTexture)
        {
            EliteRamp.AddRampToMultipleElites(SubEliteDefs.Prepend(EliteDef), eliteRampTexture);
            _eliteRampTexture = eliteRampTexture;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Define a sub-elite that is identical to this elite, except in that it has different <see cref="EliteStats"/> and can be placed in different elite tiers.
        /// </summary>
        /// <remarks>
        /// <para>Useful for creating Honor variants of Tier 1 elites.</para>
        /// <para>Future changes to this elite will retroactively affect all sub-elites as well.</para>
        /// </remarks>
        /// <param name="identifier"></param>
        /// <param name="stats"></param>
        /// <param name="subEliteTiers"></param>
        /// <returns></returns>
        public TEliteWrapper DefineSubElite(string identifier, EliteStats stats, params CombatDirector.EliteTierDef[] subEliteTiers)
        {
            TEliteDef subElite = ScriptableObject.CreateInstance<TEliteDef>();
            subElite.name = string.IsNullOrEmpty(subElitePrefix) ? identifier : ContentPackExtensions.FormatAssetIdentifier(identifier, subElitePrefix);
            subElite.modifierToken = EliteDef.modifierToken;
            subElite.eliteEquipmentDef = EliteDef.eliteEquipmentDef;
            subElite.shaderEliteRampIndex = EliteDef.shaderEliteRampIndex;
            subElite.color = EliteDef.color;
            if (_eliteRampTexture)
            {
                EliteRamp.AddRamp(subElite, _eliteRampTexture);
            }
            subElite.healthBoostCoefficient = stats.healthBoostCoefficient;
            subElite.damageBoostCoefficient = stats.damageBoostCoefficient;
            for (int i = 0; i < subEliteTiers.Length; i++)
            {
                subEliteTiers[i].AddElite(subElite);
            }
            registerSubEliteCallback?.Invoke(subElite);
            SubEliteDefs.Add(subElite);
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Set icon sprite of this elite equipment.
        /// </summary>
        /// <remarks>
        /// Elite equipment icons are usually 128px.
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEliteWrapper SetEliteEquipmentIconSprite(Sprite iconSprite)
        {
            EliteEquipmentDef.pickupIconSprite = iconSprite;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Set the physical model of this elite equipment in the world.
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEliteWrapper SetEliteEquipmentPickupModelPrefab(GameObject pickupModelPrefab)
        {
            EliteEquipmentDef.pickupModelPrefab = pickupModelPrefab;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Set the icon sprite of this elite buff and a tint color for that sprite.
        /// </summary>
        /// <remarks>
        /// <para>Elite buff icons are usually 128px.</para>
        /// <para>Use <see cref="SetEliteBuffIconSprite(Sprite)"/> to set an icon with no color tint.</para>
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEliteWrapper SetEliteBuffIconSprite(Sprite iconSprite, Color spriteColor)
        {
            EliteBuffDef.iconSprite = iconSprite;
            EliteBuffDef.buffColor = spriteColor;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Set the icon sprite of this elite buff.
        /// </summary>
        /// <remarks>
        /// <para>Elite buff icons are usually 128px.</para>
        /// <para>This overload is used for icon sprites that are already colored (e.g., icons requiring multiple colors). Use <see cref="SetEliteBuffIconSprite(Sprite, Color)"/> to set an icon with a color tint.</para>
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEliteWrapper SetEliteBuffIconSprite(Sprite iconSprite)
        {
            EliteBuffDef.iconSprite = iconSprite;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Set a <see cref="NetworkSoundEventDef"/> to be played when this elite buff is added to a character.
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TEliteWrapper SetEliteBuffStartSfx(NetworkSoundEventDef startSfx)
        {
            EliteBuffDef.startSfx = startSfx;
            return this as TEliteWrapper;
        }

        /// <summary>
        /// Access the <see cref="EliteDef.modifierToken"/> of <see cref="EliteDef"/>.
        /// </summary>
        public string ModifierToken => EliteDef.modifierToken;
    }
}