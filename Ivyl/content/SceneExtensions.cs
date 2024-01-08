using System;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System.Collections;

namespace IvyLibrary
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

        public static IEnumerator SetPreviewTextureAsync(this SceneDef sceneDef, Texture previewTexture)
        {
            var matBazaarSeerGolemplains = Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat");
            if (!matBazaarSeerGolemplains.IsDone)
            {
                yield return matBazaarSeerGolemplains;
            }
            SetPreviewTextureImpl(sceneDef, previewTexture, matBazaarSeerGolemplains.Result);
        }

        [Obsolete($"{nameof(SetPreviewTexture)} is not asynchronous and may stall loading. {nameof(SetPreviewTextureAsync)} is preferred.", false)]
        public static TSceneDef SetPreviewTexture<TSceneDef>(this TSceneDef sceneDef, Texture previewTexture) where TSceneDef : SceneDef
        {
            SetPreviewTextureImpl(sceneDef, previewTexture, Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat").WaitForCompletion());
            return sceneDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetPreviewTextureImpl(SceneDef sceneDef, Texture previewTexture, Material matBazaarSeerGolemplains)
        {
            if ((sceneDef.previewTexture = previewTexture) != null)
            {
                if (!sceneDef.portalMaterial)
                {
                    sceneDef.portalMaterial = new Material(matBazaarSeerGolemplains);
                    sceneDef.portalMaterial.name = "matBazaarSeer" + sceneDef.cachedName;
                }
                sceneDef.portalMaterial.SetTexture("_MainTex", previewTexture);
            }
            else if (sceneDef.portalMaterial)
            {
                UnityEngine.Object.Destroy(sceneDef.portalMaterial);
                sceneDef.portalMaterial = null;
            }
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