using BepInEx;
using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using MonoMod.Cil;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using UnityEngine;
using System.Collections.Generic;
using R2API.ScriptableObjects;
using UnityEngine.AddressableAssets;
using System.Linq;
using R2API;
using System.Runtime.CompilerServices;
using System.Collections;
using RoR2.ContentManagement;
using UnityEngine.Events;
using BepInEx.Configuration;
using RoR2;
using ThreeEyedGames;

namespace Ivyl
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

        public string GetNameToken() => AchievementDef.nameToken;

        public string GetDescriptionToken() => AchievementDef.descriptionToken;
    }
}