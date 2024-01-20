using System;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System.Collections;
using RoR2.ContentManagement;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="SceneDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class SceneExtensions
    {
        /// <summary>
        /// Override the name of this scene; used to associate scene variants with their base scene.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetBaseSceneIdentifierOverride<TSceneDef>(this TSceneDef sceneDef, string baseSceneIdentifierOverride) where TSceneDef : SceneDef
        {
            sceneDef.baseSceneNameOverride = baseSceneIdentifierOverride;
            return sceneDef;
        }

        /// <summary>
        /// Set the <see cref="SceneType"/> of this scene.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetSceneType<TSceneDef>(this TSceneDef sceneDef, SceneType sceneType) where TSceneDef : SceneDef
        {
            sceneDef.sceneType = sceneType;
            return sceneDef;
        }

        /// <summary>
        /// Set a stage order to determine the place of this scene in a loop (e.g., logbook ordering, Lunar Seer destinations in the Bazaar Between Time). 
        /// </summary>
        /// <remarks>
        /// Hidden realms have large stage order values.
        /// </remarks>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetStageOrder<TSceneDef>(this TSceneDef sceneDef, int stageOrder) where TSceneDef : SceneDef
        {
            sceneDef.stageOrder = stageOrder;
            return sceneDef;
        }

        /// <summary>
        /// Asynchronously set the preview texture for this scene (e.g., in the logbook and Lunar Seers).
        /// </summary>
        /// <remarks>
        /// Scene previews are usually 480x288px.
        /// </remarks>
        /// <returns>An <see cref="IEnumerator"/> to be yielded in an <see cref="IContentPackProvider"/>.</returns>
        public static IEnumerator SetPreviewTextureAsync(this SceneDef sceneDef, Texture previewTexture)
        {
            var matBazaarSeerGolemplains = Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat");
            if (!matBazaarSeerGolemplains.IsDone)
            {
                yield return matBazaarSeerGolemplains;
            }
            SetPreviewTextureImpl(sceneDef, previewTexture, matBazaarSeerGolemplains.Result);
        }

        /// <summary>
        /// Immediately set the preview texture for this scene (e.g., in the logbook and Lunar Seers).
        /// </summary>
        /// <remarks>
        /// <para>This method will block the main thread until completed. <see cref="SetPreviewTextureAsync(SceneDef, Texture)"/> should be used instead.</para>
        /// <para>Scene previews are usually 480x288px.</para>
        /// </remarks>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
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

        /// <summary>
        /// Set the diorama prefab for this scene in the logbook, with display parameters.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetLogbookDioramaPrefab<TSceneDef>(this TSceneDef sceneDef, GameObject logbookDioramaPrefab, ModelPanelParams modelParams) where TSceneDef : SceneDef
        {
            sceneDef.dioramaPrefab = logbookDioramaPrefab;
            Ivyl.SetupModelPanelParameters(logbookDioramaPrefab, modelParams);
            return sceneDef;
        }

        /// <summary>
        /// Set the diorama prefab for this scene in the logbook.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetLogbookDioramaPrefab<TSceneDef>(this TSceneDef sceneDef, GameObject logbookDioramaPrefab) where TSceneDef : SceneDef
        {
            sceneDef.dioramaPrefab = logbookDioramaPrefab;
            return sceneDef;
        }

        /// <summary>
        /// Set the music to play in this scene.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetMusic<TSceneDef>(this TSceneDef sceneDef, MusicTrackDef mainMusic, MusicTrackDef bossMusic) where TSceneDef : SceneDef
        {
            sceneDef.mainTrack = mainMusic;
            sceneDef.bossTrack = bossMusic;
            return sceneDef;
        }

        /// <summary>
        /// Set the boolean values of this scene with <see cref="ItemTierFlags"/>.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
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

        /// <summary>
        /// Set the destinations of this scene.
        /// </summary>
        /// <returns><paramref name="sceneDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSceneDef SetDestinationsGroup<TSceneDef>(this TSceneDef sceneDef, SceneCollection destinationsGroup) where TSceneDef : SceneDef
        {
            sceneDef.destinationsGroup = destinationsGroup;
            return sceneDef;
        }
    }
}