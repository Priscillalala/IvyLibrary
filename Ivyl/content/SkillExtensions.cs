using System;
using UnityEngine;
using RoR2.Skills;
using EntityStates;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="SkillDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class SkillExtensions
    {
        /// <summary>
        /// Set the icon sprite for this skill.
        /// </summary>
        /// <remarks>
        /// Skill icons are usually 128px.
        /// </remarks>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetIconSprite<TSkillDef>(this TSkillDef skillDef, Sprite iconSprite) where TSkillDef : SkillDef
        {
            skillDef.icon = iconSprite;
            return skillDef;
        }

        /// <summary>
        /// Set the keywords for this skill to be displayed in the loadout selection screen.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetKeywordTokens<TSkillDef>(this TSkillDef skillDef, params string[] keywordTokens) where TSkillDef : SkillDef
        {
            skillDef.keywordTokens = keywordTokens;
            return skillDef;
        }

        /// <summary>
        /// Set the activation entity state of this skill, and the name of the entity state machine that will handle this state (e.g., "Weapon", "Body").
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetActivationState<TSkillDef>(this TSkillDef skillDef, SerializableEntityStateType activationState, string activationStateMachineName) where TSkillDef : SkillDef
        {
            skillDef.activationState = activationState;
            skillDef.activationStateMachineName = activationStateMachineName;
            return skillDef;
        }

        /// <inheritdoc cref="SetActivationState{TSkillDef}(TSkillDef, SerializableEntityStateType, string)"/>
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

        /// <summary>
        /// Set the <see cref="InterruptPriority"/> of this skill.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetInterruptPriority<TSkillDef>(this TSkillDef skillDef, InterruptPriority interruptPriority) where TSkillDef : SkillDef
        {
            skillDef.interruptPriority = interruptPriority;
            return skillDef;
        }

        /// <summary>
        /// Set the cooldown of this skill, in seconds.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetCooldown<TSkillDef>(this TSkillDef skillDef, float cooldown) where TSkillDef : SkillDef
        {
            skillDef.baseRechargeInterval = cooldown;
            return skillDef;
        }

        /// <summary>
        /// Set the base maximum stock of this skill.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetMaxStock<TSkillDef>(this TSkillDef skillDef, int maxStock) where TSkillDef : SkillDef
        {
            skillDef.baseMaxStock = maxStock;
            return skillDef;
        }

        /// <summary>
        /// Set the amount of stock to recharge per cooldown cycle of this skill.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetRechargeStock<TSkillDef>(this TSkillDef skillDef, int rechargeStock) where TSkillDef : SkillDef
        {
            skillDef.rechargeStock = rechargeStock;
            return skillDef;
        }

        /// <summary>
        /// Set the amount of stock required to activate this skill.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetRequiredStock<TSkillDef>(this TSkillDef skillDef, int requiredStock) where TSkillDef : SkillDef
        {
            skillDef.requiredStock = requiredStock;
            return skillDef;
        }

        /// <summary>
        /// Set the amount of stock to consume when activating this skill.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkillDef SetStockToConsume<TSkillDef>(this TSkillDef skillDef, int stockToConsume) where TSkillDef : SkillDef
        {
            skillDef.stockToConsume = stockToConsume;
            return skillDef;
        }

        /// <summary>
        /// Set the boolean values of this skill with <see cref="SkillFlags"/>.
        /// </summary>
        /// <returns><paramref name="skillDef"/>, to continue a method chain.</returns>
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
    }
}