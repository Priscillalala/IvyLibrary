using System;
using UnityEngine;
using RoR2;

namespace IvyLibrary
{
    public record class AchievementWrapper(AchievementDef AchievementDef, UnlockableDef UnlockableDef) 
        : AchievementWrapper<AchievementWrapper, AchievementDef, UnlockableDef>(AchievementDef, UnlockableDef) 
    { }

    public record class AchievementWrapper<TAchievementDef, TUnlockableDef>(TAchievementDef AchievementDef, TUnlockableDef UnlockableDef) 
        : AchievementWrapper<AchievementWrapper<TAchievementDef, TUnlockableDef>, TAchievementDef, TUnlockableDef>(AchievementDef, UnlockableDef)
        where TAchievementDef : AchievementDef
        where TUnlockableDef : UnlockableDef
    { }

    public abstract record class AchievementWrapper<TAchievementWrapper, TAchievementDef, TUnlockableDef>(TAchievementDef AchievementDef, TUnlockableDef UnlockableDef)
        where TAchievementWrapper : AchievementWrapper<TAchievementWrapper, TAchievementDef, TUnlockableDef>
        where TAchievementDef : AchievementDef
        where TUnlockableDef : UnlockableDef
    {
        public TAchievementWrapper SetIconSprite(Sprite iconSprite)
        {
            AchievementDef.SetAchievedIcon(iconSprite);
            UnlockableDef.achievementIcon = iconSprite;
            return this as TAchievementWrapper;
        }

        public TAchievementWrapper SetPrerequisiteAchievement(AchievementDef prerequisiteAchievement)
        {
            AchievementDef.prerequisiteAchievementIdentifier = prerequisiteAchievement.identifier;
            return this as TAchievementWrapper;
        }

        public TAchievementWrapper SetPrerequisiteAchievement(string prerequisiteAchievementIdentifier)
        {
            AchievementDef.prerequisiteAchievementIdentifier = prerequisiteAchievementIdentifier;
            return this as TAchievementWrapper;
        }

        public TAchievementWrapper SetTrackerTypes(Type localTrackerType, Type serverTrackerType = null)
        {
            AchievementDef.type = localTrackerType;
            AchievementDef.serverTrackerType = serverTrackerType;
            return this as TAchievementWrapper;
        }

        public string NameToken => AchievementDef.nameToken;

        public string DescriptionToken => AchievementDef.descriptionToken;
    }
}