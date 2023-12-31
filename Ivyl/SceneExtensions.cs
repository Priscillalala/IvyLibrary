﻿using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.ContentManagement;
using HG;
using UnityEngine.AddressableAssets;

namespace Ivyl
{
    public static class SceneExtensions
    {
        public static TSceneDef SetBaseSceneIdentifierOverride<TSceneDef>(this TSceneDef sceneDef, string baseSceneIdentifierOverride) where TSceneDef : SceneDef
        {
            sceneDef.baseSceneNameOverride = baseSceneIdentifierOverride;
            return sceneDef;
        }

        public static TSceneDef SetSceneType<TSceneDef>(this TSceneDef sceneDef, SceneType sceneType) where TSceneDef : SceneDef
        {
            sceneDef.sceneType = sceneType;
            return sceneDef;
        }

        public static TSceneDef SetStageOrder<TSceneDef>(this TSceneDef sceneDef, int stageOrder) where TSceneDef : SceneDef
        {
            sceneDef.stageOrder = stageOrder;
            return sceneDef;
        }

        public static TSceneDef SetPreviewTexture<TSceneDef>(this TSceneDef sceneDef, Texture previewTexture) where TSceneDef : SceneDef
        {
            if (sceneDef.previewTexture = previewTexture)
            {
                if (!sceneDef.portalMaterial)
                {
                    sceneDef.portalMaterial = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat").WaitForCompletion());
                    sceneDef.portalMaterial.name = "matBazaarSeer" + sceneDef.cachedName;
                }
                sceneDef.portalMaterial.SetTexture("_MainTex", previewTexture);
            }
            else if (sceneDef.portalMaterial)
            {
                UnityEngine.Object.Destroy(sceneDef.portalMaterial);
                sceneDef.portalMaterial = null;
            }
            return sceneDef;
        }

        public static TSceneDef SetLogbookDioramaPrefab<TSceneDef>(this TSceneDef sceneDef, GameObject logbookDioramaPrefab) where TSceneDef : SceneDef
        {
            sceneDef.dioramaPrefab = logbookDioramaPrefab;
            return sceneDef;
        }

        public static TSceneDef SetMusic<TSceneDef>(this TSceneDef sceneDef, MusicTrackDef mainMusic, MusicTrackDef bossMusic) where TSceneDef : SceneDef
        {
            sceneDef.mainTrack = mainMusic;
            sceneDef.bossTrack = bossMusic;
            return sceneDef;
        }

        public static TSceneDef SetFlags<TSceneDef>(this TSceneDef sceneDef, SceneFlags flags) where TSceneDef : SceneDef
        {
            sceneDef.isOfflineScene = (flags & SceneFlags.Offline) > SceneFlags.None;
            sceneDef.shouldIncludeInLogbook = (flags & SceneFlags.ExcludeFromLogbook) <= SceneFlags.None;
            sceneDef.suppressPlayerEntry = (flags & SceneFlags.SuppressPlayerEntry) > SceneFlags.None;
            sceneDef.suppressNpcEntry = (flags & SceneFlags.SuppressNPCEntry) > SceneFlags.None;
            sceneDef.blockOrbitalSkills = (flags & SceneFlags.BlockOrbitalSkills) > SceneFlags.None;
            sceneDef.validForRandomSelection = (flags & SceneFlags.NeverRandomlySelected) <= SceneFlags.None;
            return sceneDef;
        }

        public static TSceneDef SetDestinationsGroup<TSceneDef>(this TSceneDef sceneDef, SceneCollection destinationsGroup) where TSceneDef : SceneDef
        {
            sceneDef.destinationsGroup = destinationsGroup;
            return sceneDef;
        }
    }
}