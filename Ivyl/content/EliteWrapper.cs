using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using R2API;
using RoR2;

namespace IvyLibrary
{
    public record class EliteWrapper(EliteDef EliteDef, List<EliteDef> SubEliteDefs, EquipmentDef EliteEquipmentDef, BuffDef EliteBuffDef)
        : EliteWrapper<EliteWrapper, EliteDef, EquipmentDef, BuffDef>(EliteDef, SubEliteDefs, EliteEquipmentDef, EliteBuffDef)
    { }

    public record class EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef>(TEliteDef EliteDef, List<TEliteDef> SubEliteDefs, TEquipmentDef EliteEquipmentDef, TBuffDef EliteBuffDef) 
        : EliteWrapper<EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef>, TEliteDef, TEquipmentDef, TBuffDef>(EliteDef, SubEliteDefs, EliteEquipmentDef, EliteBuffDef)
        where TEliteDef : EliteDef
        where TEquipmentDef : EquipmentDef
        where TBuffDef : BuffDef
    { }

    public abstract record class EliteWrapper<TEliteWrapper, TEliteDef, TEquipmentDef, TBuffDef>(TEliteDef EliteDef, List<TEliteDef> SubEliteDefs, TEquipmentDef EliteEquipmentDef, TBuffDef EliteBuffDef)
        where TEliteWrapper : EliteWrapper<TEliteWrapper, TEliteDef, TEquipmentDef, TBuffDef>
        where TEliteDef : EliteDef
        where TEquipmentDef : EquipmentDef
        where TBuffDef : BuffDef
    {
        public Action<EliteDef> registerSubEliteCallback;
        public string subEliteTokenPrefix;
        private Texture2D _eliteRampTexture;

        public TEliteWrapper SetStats(EliteStats stats)
        {
            EliteDef.healthBoostCoefficient = stats.healthBoostCoefficient;
            EliteDef.damageBoostCoefficient = stats.damageBoostCoefficient;
            return this as TEliteWrapper;
        }

        public TEliteWrapper AddToEliteTier(CombatDirector.EliteTierDef eliteTier)
        {
            eliteTier.AddElite(EliteDef);
            return this as TEliteWrapper;
        }

        public TEliteWrapper SetEliteRampTexture(Texture2D eliteRampTexture)
        {
            EliteRamp.AddRampToMultipleElites(SubEliteDefs.Prepend(EliteDef), eliteRampTexture);
            _eliteRampTexture = eliteRampTexture;
            return this as TEliteWrapper;
        }

        public TEliteWrapper DefineSubElite(string identifier, EliteStats stats, params CombatDirector.EliteTierDef[] subEliteTiers)
        {
            TEliteDef subElite = ScriptableObject.CreateInstance<TEliteDef>();
            subElite.name = string.IsNullOrEmpty(subEliteTokenPrefix) ? identifier : (subEliteTokenPrefix + '_' + identifier);
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
            registerSubEliteCallback(subElite);
            SubEliteDefs.Add(subElite);
            return this as TEliteWrapper;
        }

        public TEliteWrapper SetEliteEquipmentIconSprite(Sprite iconSprite)
        {
            EliteEquipmentDef.pickupIconSprite = iconSprite;
            return this as TEliteWrapper;
        }

        public TEliteWrapper SetEliteEquipmentPickupModelPrefab(GameObject pickupModelPrefab)
        {
            EliteEquipmentDef.pickupModelPrefab = pickupModelPrefab;
            return this as TEliteWrapper;
        }

        public TEliteWrapper SetEliteBuffIconSprite(Sprite iconSprite, Color spriteColor)
        {
            EliteBuffDef.iconSprite = iconSprite;
            EliteBuffDef.buffColor = spriteColor;
            return this as TEliteWrapper;
        }

        public TEliteWrapper SetEliteBuffIconSprite(Sprite iconSprite)
        {
            EliteBuffDef.iconSprite = iconSprite;
            return this as TEliteWrapper;
        }

        public TEliteWrapper SetEliteBuffStartSfx(NetworkSoundEventDef startSfx)
        {
            EliteBuffDef.startSfx = startSfx;
            return this as TEliteWrapper;
        }

        public string ModifierToken => EliteDef.modifierToken;
    }
}