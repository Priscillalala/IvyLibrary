using System;
using UnityEngine;
using RoR2;
using RoR2.Achievements;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Associates an <see cref="AchievementDef"/> with an <see cref="UnlockableDef"/> and provides methods for manipulating them.
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public record class AchievementWrapper(AchievementDef AchievementDef, UnlockableDef UnlockableDef) 
        : AchievementWrapper<AchievementWrapper, AchievementDef, UnlockableDef>(AchievementDef, UnlockableDef) 
    { }

    /// <inheritdoc cref="AchievementWrapper"/>
    public record class AchievementWrapper<TAchievementDef, TUnlockableDef>(TAchievementDef AchievementDef, TUnlockableDef UnlockableDef) 
        : AchievementWrapper<AchievementWrapper<TAchievementDef, TUnlockableDef>, TAchievementDef, TUnlockableDef>(AchievementDef, UnlockableDef)
        where TAchievementDef : AchievementDef
        where TUnlockableDef : UnlockableDef
    { }

    /// <inheritdoc cref="AchievementWrapper"/>
    public abstract record class AchievementWrapper<TAchievementWrapper, TAchievementDef, TUnlockableDef>(TAchievementDef AchievementDef, TUnlockableDef UnlockableDef)
        where TAchievementWrapper : AchievementWrapper<TAchievementWrapper, TAchievementDef, TUnlockableDef>
        where TAchievementDef : AchievementDef
        where TUnlockableDef : UnlockableDef
    {
        /// <summary>
        /// Set the icon sprite of this achievement.
        /// </summary>
        /// <remarks>
        /// Achievement icons are usually 128px.
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAchievementWrapper SetIconSprite(Sprite iconSprite)
        {
            AchievementDef.SetAchievedIcon(iconSprite);
            UnlockableDef.achievementIcon = iconSprite;
            return this as TAchievementWrapper;
        }

        /// <summary>
        /// Set an achievement that must be completed before this achievement is available (e.g., hiding skill achievements while a survivor is locked).
        /// </summary>
        /// <returns>this, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAchievementWrapper SetPrerequisiteAchievement(AchievementDef prerequisiteAchievement)
        {
            AchievementDef.prerequisiteAchievementIdentifier = prerequisiteAchievement?.identifier;
            return this as TAchievementWrapper;
        }

        /// <inheritdoc cref="SetPrerequisiteAchievement(RoR2.AchievementDef)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TAchievementWrapper SetPrerequisiteAchievement(string prerequisiteAchievementIdentifier)
        {
            AchievementDef.prerequisiteAchievementIdentifier = prerequisiteAchievementIdentifier;
            return this as TAchievementWrapper;
        }

        /// <summary>
        /// Set achievement tracker types to be installed with this achievement.
        /// </summary>
        /// <remarks>
        /// <paramref name="localTrackerType"/> should inherit from <see cref="BaseAchievement"/> and <paramref name="serverTrackerType"/> should inherit from <see cref="BaseServerAchievement"/>.
        /// <paramref name="serverTrackerType"/> is optional.
        /// </remarks>
        /// <returns>this, to continue a method chain.</returns>
        public TAchievementWrapper SetTrackerTypes(Type localTrackerType, Type serverTrackerType = null)
        {
            if (localTrackerType == null)
            {
                throw new ArgumentNullException(nameof(localTrackerType));
            }
            if (!localTrackerType.IsSubclassOf(typeof(BaseAchievement)))
            {
                throw new ArgumentException(nameof(localTrackerType));
            }
            if (serverTrackerType != null && !serverTrackerType.IsSubclassOf(typeof(BaseServerAchievement)))
            {
                throw new ArgumentException(nameof(serverTrackerType));
            }
            AchievementDef.type = localTrackerType;
            AchievementDef.serverTrackerType = serverTrackerType;
            return this as TAchievementWrapper;
        }

        /// <summary>
        /// Access the <see cref="AchievementDef.nameToken"/> of <see cref="AchievementDef"/>.
        /// </summary>
        public string NameToken => AchievementDef.nameToken;

        /// <summary>
        /// Access the <see cref="AchievementDef.descriptionToken"/> of <see cref="AchievementDef"/>.
        /// </summary>
        public string DescriptionToken => AchievementDef.descriptionToken;
    }
}