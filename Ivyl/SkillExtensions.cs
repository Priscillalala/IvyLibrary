using BepInEx;
using System;
using RoR2;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.ContentManagement;
using HG;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using EntityStates;

namespace Ivyl
{
    public static class SkillExtensions
    {
        public static TSkillDef SetIconSprite<TSkillDef>(this TSkillDef skillDef, Sprite iconSprite) where TSkillDef : SkillDef
        {
            skillDef.icon = iconSprite;
            return skillDef;
        }

        public static TSkillDef SetKeywordTokens<TSkillDef>(this TSkillDef skillDef, params string[] keywordTokens) where TSkillDef : SkillDef
        {
            skillDef.keywordTokens = keywordTokens;
            return skillDef;
        }

        public static TSkillDef SetActivationState<TSkillDef>(this TSkillDef skillDef, SerializableEntityStateType activationState, string activationStateMachineName) where TSkillDef : SkillDef
        {
            skillDef.activationState = activationState;
            skillDef.activationStateMachineName = activationStateMachineName;
            return skillDef;
        }

        public static TSkillDef SetActivationState<TSkillDef>(this TSkillDef skillDef, Type activationStateType, string activationStateMachineName) where TSkillDef : SkillDef
        {
            if (activationStateType == null)
            {
                throw new ArgumentNullException(nameof(activationStateType));
            }
            if (!activationStateType.IsSubclassOf(typeof(EntityState)))
            {
                throw new ArgumentException(nameof(activationStateType));
            }
            skillDef.activationState = new SerializableEntityStateType(activationStateType);
            skillDef.activationStateMachineName = activationStateMachineName;
            return skillDef;
        }

        public static TSkillDef SetInterruptPriority<TSkillDef>(this TSkillDef skillDef, InterruptPriority interruptPriority) where TSkillDef : SkillDef
        {
            skillDef.interruptPriority = interruptPriority;
            return skillDef;
        }

        public static TSkillDef SetCooldown<TSkillDef>(this TSkillDef skillDef, float cooldown) where TSkillDef : SkillDef
        {
            skillDef.baseRechargeInterval = cooldown;
            return skillDef;
        }

        public static TSkillDef SetMaxStock<TSkillDef>(this TSkillDef skillDef, int maxStock) where TSkillDef : SkillDef
        {
            skillDef.baseMaxStock = maxStock;
            return skillDef;
        }

        public static TSkillDef SetRechargeStock<TSkillDef>(this TSkillDef skillDef, int rechargeStock) where TSkillDef : SkillDef
        {
            skillDef.rechargeStock = rechargeStock;
            return skillDef;
        }

        public static TSkillDef SetRequiredStock<TSkillDef>(this TSkillDef skillDef, int requiredStock) where TSkillDef : SkillDef
        {
            skillDef.requiredStock = requiredStock;
            return skillDef;
        }

        public static TSkillDef SetStockToConsume<TSkillDef>(this TSkillDef skillDef, int stockToConsume) where TSkillDef : SkillDef
        {
            skillDef.stockToConsume = stockToConsume;
            return skillDef;
        }

        public static TSkillDef SetFlags<TSkillDef>(this TSkillDef skillDef, SkillFlags flags) where TSkillDef : SkillDef
        {
            skillDef.resetCooldownTimerOnUse = (flags & SkillFlags.ResetCooldownTimerOnUse) > SkillFlags.None;
            skillDef.fullRestockOnAssign = (flags & SkillFlags.NoRestockOnAssign) <= SkillFlags.None;
            skillDef.dontAllowPastMaxStocks = (flags & SkillFlags.DontAllowPastMaxStocks) > SkillFlags.None;
            skillDef.beginSkillCooldownOnSkillEnd = (flags & SkillFlags.BeginSkillCooldownOnSkillEnd) > SkillFlags.None;
            skillDef.cancelSprintingOnActivation = (flags & SkillFlags.Agile) <= SkillFlags.None;
            skillDef.forceSprintDuringState = (flags & SkillFlags.ForceSprint) > SkillFlags.None;
            skillDef.canceledFromSprinting = (flags & SkillFlags.CanceledBySprinting) > SkillFlags.None;
            skillDef.isCombatSkill = (flags & SkillFlags.NonCombat) <= SkillFlags.None;
            skillDef.mustKeyPress = (flags & SkillFlags.MustKeyPress) > SkillFlags.None;
            return skillDef;
        }

        public static TSkillDef SetAdditionalFields<TSkillDef>(this TSkillDef skillDef, Action<TSkillDef> _) where TSkillDef : SkillDef
        {
            _(skillDef);
            return skillDef;
        }

        /*public static TSkillDef AddToSkillFamily<TSkillDef>(this TSkillDef skillDef, SkillFamily skillFamily, UnlockableDef requiredUnlockable = null) where TSkillDef : SkillDef
        {
            skillFamily.AddSkill(skillDef, requiredUnlockable);
            return skillDef;
        }*/
    }
}