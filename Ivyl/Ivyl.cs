using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine;
using RoR2.ContentManagement;
using HG;
using UnityEngine.AddressableAssets;
using R2API;
using System.Linq;
using RoR2.Skills;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using HG.GeneralSerializer;
using UnityEngine.Rendering;
using UnityEngine.Networking;
using BepInEx.Configuration;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Runtime.CompilerServices;
using ThreeEyedGames;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace IvyLibrary
{
    /// <summary>
    /// A collection of useful static and extension methods provided by IVYL.
    /// </summary>
    public static class Ivyl
    {
        private static Transform _prefabParent;

        private static void InitPrefabParent()
        {
            if (_prefabParent)
            {
                return;
            }
            _prefabParent = new GameObject("IVYLPrefabs").transform;
            _prefabParent.gameObject.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(_prefabParent.gameObject);
            On.RoR2.Util.IsPrefab += (orig, gameObject) => orig(gameObject) || gameObject.transform.parent == _prefabParent;
        }

        /// <summary>
        /// Create an empty prefab.
        /// </summary>
        /// <param name="name">The name of this prefab.</param>
        /// <returns>The newly-created prefab.</returns>
        public static GameObject CreatePrefab(string name)
        {
            InitPrefabParent();
            GameObject prefab = new GameObject(name);
            prefab.transform.SetParent(_prefabParent);
            return prefab;
        }

        /// <summary>
        /// Clone an existing prefab.
        /// </summary>
        /// <remarks>
        /// If the new prefab has a <see cref="NetworkIdentity"/>, the asset id will be reset to avoid asset collisions.
        /// A new asset id can be generated manually or through <see cref="ContentPackExtensions.PopulateNetworkedObjectAssetIds(ContentPack)"/>.
        /// </remarks>
        /// <param name="original">The <see cref="GameObject"/> to clone.</param>
        /// <param name="name">The name of the new prefab.</param>
        /// <returns>The newly-created prefab.</returns>
        public static GameObject ClonePrefab(GameObject original, string name)
        {
            InitPrefabParent();
            GameObject prefab = UnityEngine.Object.Instantiate(original, _prefabParent);
            prefab.name = name;
            if (prefab.TryGetComponent(out NetworkIdentity networkIdentity))
            {
                networkIdentity.m_AssetId.Reset();
            }
            return prefab;
        }

        /// <summary>
        /// Adds a <see cref="ModelPanelParameters"/> component to <paramref name="model"/> based on the input parameters.
        /// </summary>
        /// <remarks>
        /// Looks for a child named "FocusPoint" or "Focus Point" for the <see cref="ModelPanelParameters.focusPointTransform"/>; otherwise, creates one.
        /// Looks for a child named "CameraPosition" or "Camera Position" for the <see cref="ModelPanelParameters.cameraPositionTransform"/>; otherwise, creates one.
        /// </remarks>
        /// <returns>The newly-created <see cref="ModelPanelParameters"/>.</returns>
        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, Vector3 modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
        {
            return SetupModelPanelParameters(model, Quaternion.Euler(modelRotation), minDistance, maxDistance, focusPoint, cameraPosition);
        }

        /// <inheritdoc cref="SetupModelPanelParameters(GameObject, Vector3, float, float, Transform, Transform)"/>        
        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, ModelPanelParams modelPanelParams)
        {
            return SetupModelPanelParameters(model, modelPanelParams.modelRotation, modelPanelParams.minDistance, modelPanelParams.maxDistance, modelPanelParams.focusPoint, modelPanelParams.cameraPosition);
        }

        /// <inheritdoc cref="SetupModelPanelParameters(GameObject, Vector3, float, float, Transform, Transform)"/>        
        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, Quaternion modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
        {
            ModelPanelParameters parameters = model.AddComponent<ModelPanelParameters>();
            parameters.modelRotation = modelRotation;
            parameters.minDistance = minDistance;
            parameters.maxDistance = maxDistance;
            parameters.focusPointTransform = focusPoint ?? model.transform.Find("FocusPoint") ?? model.transform.Find("Focus Point");
            if (!parameters.focusPointTransform)
            {
                Transform newFocusPoint = new GameObject("FocusPoint").transform;
                newFocusPoint.SetParent(model.transform);
                parameters.focusPointTransform = newFocusPoint;
            }
            parameters.cameraPositionTransform = cameraPosition ?? model.transform.Find("CameraPosition") ?? model.transform.Find("Camera Position");
            if (!parameters.cameraPositionTransform)
            {
                Transform newCameraPosition = new GameObject("CameraPosition").transform;
                newCameraPosition.SetParent(model.transform);
                newCameraPosition.localPosition = model.transform.forward;
                parameters.cameraPositionTransform = newCameraPosition;
            }
            return parameters;
        }

        /// <summary>
        /// Adds an <see cref="ItemDisplay"/> component to <paramref name="displayModelPrefab"/> and populates the <see cref="ItemDisplay.rendererInfos"/>.
        /// </summary>
        /// <returns>The newly-created <see cref="ItemDisplay"/>.</returns>
        public static ItemDisplay SetupItemDisplay(GameObject displayModelPrefab)
        {
            ItemDisplay itemDisplay = displayModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = displayModelPrefab.GetComponentsInChildren<Renderer>()
                .Where(x => x is MeshRenderer or SkinnedMeshRenderer)
                .Select(x => new CharacterModel.RendererInfo
                {
                    defaultMaterial = x.sharedMaterial,
                    renderer = x,
                    defaultShadowCastingMode = ShadowCastingMode.On,
                    ignoreOverlays = false
                })
                .ToArray();
            return itemDisplay;
        }

        /// <summary>
        /// Calculate the value of an item stack where the first stack bonus differs from subsequent stacks.
        /// </summary>
        /// <param name="baseValue">The value of the first item stack.</param>
        /// <param name="stackValue">The value of subsequent item stacks.</param>
        /// <param name="stack">The current item stack.</param>
        /// <returns>Current value of <paramref name="stack"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float StackScaling(float baseValue, float stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0f;
        }

        /// <inheritdoc cref="StackScaling(float, float, int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StackScaling(int baseValue, int stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0;
        }

        /// <summary>
        /// Asynchronously load an addressable asset with inlined syntax.
        /// </summary>
        /// <typeparam name="TObject">Asset type.</typeparam>
        /// <param name="key">A key for this asset; usually an address string.</param>
        /// <param name="handle">The load handle, to access the loaded asset.</param>
        /// <returns><paramref name="handle"/> or null as an <see cref="IEnumerator"/>, to be yielded.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerator LoadAddressableAssetAsync<TObject>(object key, out AsyncOperationHandle<TObject> handle)
        {
            return (handle = Addressables.LoadAssetAsync<TObject>(key)).IsDone ? null : handle;
        }

        /// <summary>
        /// Asynchronously load multiple addressable assets and construct an asset dictionary.
        /// </summary>
        /// <typeparam name="TObject">Asset type.</typeparam>
        /// <param name="keys">Keys or labels for the desired assets.</param>
        /// <param name="handle">The load handle, to access the loaded assets.</param>
        /// <param name="mergeMode">Determines how <paramref name="keys"/> are evaluated.</param>
        /// <returns><paramref name="handle"/> or null as an <see cref="IEnumerator"/>, to be yielded.</returns>
        public static IEnumerator LoadAddressableAssetsAsync<TObject>(IEnumerable keys, out AsyncOperationHandle<IDictionary<string, TObject>> handle, Addressables.MergeMode mergeMode = Addressables.MergeMode.Union)
        {
            var loadLocations = Addressables.LoadResourceLocationsAsync(keys, mergeMode, typeof(TObject));
            var loadAssets = Addressables.ResourceManager.CreateChainOperation<IList<TObject>>(loadLocations, x =>
            {
                return Addressables.LoadAssetsAsync<TObject>(loadLocations.Result, null, false);
            });
            handle = Addressables.ResourceManager.CreateChainOperation<IDictionary<string, TObject>>(loadAssets, x =>
            {
                Dictionary<string, TObject> dict = new Dictionary<string, TObject>();
                for (int i = 0; i < loadAssets.Result.Count; i++)
                {
                    string key = loadLocations.Result[i].PrimaryKey;
                    dict[key] = dict[System.IO.Path.GetFileNameWithoutExtension(key)] = loadAssets.Result[i];
                }
                return Addressables.ResourceManager.CreateCompletedOperation<IDictionary<string, TObject>>(dict, null);
            });
            return handle.IsDone ? null : handle;
        }

        /// <summary>
        /// Load an asset bundle relative to this plugin.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="path"/> includes the file name.</para>
        /// <para><see cref="LoadAssetBundleAsync(BaseUnityPlugin, string)"/> is lightly preferred due to not blocking the main thread.</para>
        /// </remarks>
        /// <param name="plugin"></param>
        /// <param name="path">The relative path from this plugin to the asset bundle file, or an absolute path to the asset bundle file</param>
        /// <returns>The loaded <see cref="AssetBundle"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssetBundle LoadAssetBundle(this BaseUnityPlugin plugin, string path)
        {
            return AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(plugin.Info.Location), path));
        }

        /// <summary>
        /// Asynchronously load an asset bundle relative to this plugin.
        /// </summary>
        /// <remarks>
        /// <paramref name="path"/> includes the file name.
        /// </remarks>
        /// <param name="plugin"></param>
        /// <param name="path">The relative path from this plugin to the asset bundle file, or an absolute path to the asset bundle file.</param>
        /// <returns>An <see cref="AssetBundleCreateRequest"/> to track load progress.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssetBundleCreateRequest LoadAssetBundleAsync(this BaseUnityPlugin plugin, string path)
        {
            return AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(plugin.Info.Location), path));
        }

        /// <summary>
        /// Convert this <see cref="AssetBundleRequest"/> to an <see cref="AssetBundleRequest{T}"/>.
        /// </summary>
        public static AssetBundleRequest<T> Convert<T>(this AssetBundleRequest request) where T : UnityEngine.Object
        {
            return new AssetBundleRequest<T>(request);
        }

        /// <summary>
        /// Asynchronously load a bundled asset with inlined syntax.
        /// </summary>
        /// <typeparam name="T">Asset type.</typeparam>
        /// <param name="assetBundle"></param>
        /// <param name="name">The name of the requested asset.</param>
        /// <param name="request">The load request, to access the loaded asset.</param>
        /// <returns><paramref name="request"/> as an <see cref="IEnumerator"/>, to be yielded.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerator LoadAssetAsync<T>(this AssetBundle assetBundle, string name, out AssetBundleRequest<T> request) where T : UnityEngine.Object
        {
            return request = assetBundle.LoadAssetAsync<T>(name).Convert<T>();
        }

        /// <summary>
        /// Create an additional configuration file for this plugin.
        /// </summary>
        /// <remarks>
        /// <paramref name="path"/> includes the file name.
        /// Enforces the .cfg file extension.
        /// </remarks>
        /// <param name="plugin"></param>
        /// <param name="path">The relative path from the BepInEx config folder to the config file, or an absolute path to the config file.</param>
        /// <param name="saveOnInit">Will immediately save the config file to disk if true; otherwise, waits until the next interaction.</param>
        /// <returns>The newly-created <see cref="ConfigFile"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfigFile CreateConfigFile(this BaseUnityPlugin plugin, string path, bool saveOnInit = true)
        {
            return new ConfigFile(
                System.IO.Path.Combine(Paths.ConfigPath, System.IO.Path.ChangeExtension(path, ".cfg")),
                saveOnInit,
                plugin.Info.Metadata);
        }

        /// <summary>
        /// A variant of <see cref="ConfigFile.Bind{T}(string, string, T, string)"/> that directly returns <see cref="ConfigEntry{T}.Value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Value<T>(this ConfigFile config, string section, string key, T defaultValue, string description)
        {
            return config.Bind(section, key, defaultValue, description).Value;
        }

        /// <summary>
        /// A variant of <see cref="ConfigFile.Bind{T}(string, string, T, ConfigDescription)"/> that directly returns <see cref="ConfigEntry{T}.Value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Value<T>(this ConfigFile config, string section, string key, T defaultValue, ConfigDescription configDescription = null)
        {
            return config.Bind(section, key, defaultValue, configDescription).Value;
        }

        /// <summary>
        /// A variant of <see cref="ConfigFile.Bind{T}(ConfigDefinition, T, ConfigDescription)"/> that directly returns <see cref="ConfigEntry{T}.Value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Value<T>(this ConfigFile config, ConfigDefinition configDefinition, T defaultValue, ConfigDescription configDescription = null)
        {
            return config.Bind(configDefinition, defaultValue, configDescription).Value;
        }

        /// <summary>
        /// A variant of <see cref="ConfigFile.Bind{T}(string, string, T, string)"/> that takes <paramref name="value"/> as both the default value and the output value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfigEntry<T> Bind<T>(this ConfigFile config, ref T value, string section, string key, string description)
        {
            ConfigEntry<T> result = config.Bind(section, key, value, description);
            value = result.Value;
            return result;
        }

        /// <summary>
        /// A variant of <see cref="ConfigFile.Bind{T}(string, string, T, ConfigDescription)"/> that takes <paramref name="value"/> as both the default value and the output value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfigEntry<T> Bind<T>(this ConfigFile config, ref T value, string section, string key, ConfigDescription configDescription = null)
        {
            ConfigEntry<T> result = config.Bind(section, key, value, configDescription);
            value = result.Value;
            return result;
        }

        /// <summary>
        /// A variant of <see cref="ConfigFile.Bind{T}(ConfigDefinition, T, ConfigDescription)"/> that takes <paramref name="value"/> as both the default value and the output value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConfigEntry<T> Bind<T>(this ConfigFile config, ref T value, ConfigDefinition configDefinition, ConfigDescription configDescription = null)
        {
            ConfigEntry<T> result = config.Bind(configDefinition, value, configDescription);
            value = result.Value;
            return result;
        }

        /// <summary>
        /// Determines if a mod was loaded by BepInEx.
        /// </summary>
        /// <remarks>
        /// Useful for implementing soft compatibility with mods.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsModLoaded(string guid) => Chainloader.PluginInfos.ContainsKey(guid);

        /// <summary>
        /// A variant of <see cref="NamedAssetCollection{TAsset}.Add(TAsset[])"/> that takes a single asset.
        /// </summary>
        public static void Add<TAsset>(this NamedAssetCollection<TAsset> assetCollection, TAsset newAsset)
        {
            string assetName = assetCollection.nameProvider(newAsset);
            if (assetCollection.assetToName.ContainsKey(newAsset))
            {
                throw new ArgumentException(string.Format("Asset {0} is already registered!", newAsset));
            }
            if (assetCollection.nameToAsset.ContainsKey(assetName))
            {
                throw new ArgumentException("Asset name " + assetName + " is already registered!");
            }
            NamedAssetCollection<TAsset>.AssetInfo assetInfo = new NamedAssetCollection<TAsset>.AssetInfo
            {
                asset = newAsset,
                assetName = assetName,
            };
            int index = Array.BinarySearch(assetCollection.assetInfos, assetInfo);
            ArrayUtils.ArrayInsert(ref assetCollection.assetInfos, ~index, assetInfo);
            assetCollection.nameToAsset[assetName] = newAsset;
            assetCollection.assetToName[newAsset] = assetName;
        }

        /// <summary>
        /// Append a new <see cref="SkillFamily.Variant"/> to <see cref="SkillFamily.variants"/>.
        /// </summary>
        /// <returns>A reference to the new <see cref="SkillFamily.Variant"/>.</returns>
        public static ref SkillFamily.Variant AddSkill(this SkillFamily skillFamily, SkillDef skill, UnlockableDef requiredUnlockable = null)
        {
            int index = AddSkill(skillFamily, new SkillFamily.Variant
            {
                skillDef = skill,
                unlockableDef = requiredUnlockable,
                viewableNode = new ViewablesCatalog.Node(skill.skillName, false, null),
            });
            return ref skillFamily.variants[index];
        }

        /// <summary>
        /// Append this <paramref name="skillVariant"/> to <see cref="SkillFamily.variants"/>.
        /// </summary>
        /// <returns>The index of <paramref name="skillVariant"/> in <see cref="SkillFamily.variants"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddSkill(this SkillFamily skillFamily, SkillFamily.Variant skillVariant)
        {
            ArrayUtils.ArrayAppend(ref skillFamily.variants, skillVariant);
            return skillFamily.variants.Length - 1;
        }

        /// <summary>
        /// Insert a new <see cref="SkillFamily.Variant"/> into <see cref="SkillFamily.variants"/>.
        /// </summary>
        /// <returns>A reference to the new <see cref="SkillFamily.Variant"/>.</returns>
        public static ref SkillFamily.Variant InsertSkill(this SkillFamily skillFamily, int index, SkillDef skill, UnlockableDef requiredUnlockable = null)
        {
            InsertSkill(skillFamily, index, new SkillFamily.Variant
            {
                skillDef = skill,
                unlockableDef = requiredUnlockable,
                viewableNode = new ViewablesCatalog.Node(skill.skillName, false, null),
            });
            return ref skillFamily.variants[index];
        }

        /// <summary>
        /// Insert this <paramref name="skillVariant"/> into <see cref="SkillFamily.variants"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InsertSkill(this SkillFamily skillFamily, int index, SkillFamily.Variant skillVariant)
        {
            ArrayUtils.ArrayInsert(ref skillFamily.variants, index, skillVariant); 
        }

        /// <summary>
        /// Append this <paramref name="elite"/> to <see cref="CombatDirector.EliteTierDef.eliteTypes"/>.
        /// </summary>
        /// <returns>The index of <paramref name="elite"/> in <see cref="CombatDirector.EliteTierDef.eliteTypes"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddElite(this CombatDirector.EliteTierDef eliteTierDef, EliteDef elite)
        {
            ArrayUtils.ArrayAppend(ref eliteTierDef.eliteTypes, elite);
            return eliteTierDef.eliteTypes.Length - 1;
        }

        /// <inheritdoc cref="AddElite(CombatDirector.EliteTierDef, EliteDef)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddElite(this CombatDirector.EliteTierDef eliteTierDef, EliteWrapper elite)
        {
            return AddElite(eliteTierDef, elite.EliteDef);
        }

        /// <summary>
        /// Append a new <see cref="SceneCollection.SceneEntry"/> to <see cref="SceneCollection.sceneEntries"/>.
        /// </summary>
        /// <returns>A reference to the new <see cref="SceneCollection.SceneEntry"/>.</returns>
        public static ref SceneCollection.SceneEntry AddScene(this SceneCollection sceneCollection, SceneDef scene, float weight = 1f)
        {
            int index = AddScene(sceneCollection, new SceneCollection.SceneEntry
            {
                sceneDef = scene,
                weight = weight,
            });
            return ref sceneCollection._sceneEntries[index];
        }

        /// <summary>
        /// Append this <paramref name="sceneEntry"/> to <see cref="SceneCollection.sceneEntries"/>.
        /// </summary>
        /// <returns>The index of <paramref name="sceneEntry"/> in <see cref="SceneCollection.sceneEntries"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddScene(this SceneCollection sceneCollection, SceneCollection.SceneEntry sceneEntry)
        {
            ArrayUtils.ArrayAppend(ref sceneCollection._sceneEntries, sceneEntry);
            return sceneCollection._sceneEntries.Length - 1;
        }

        /// <summary>
        /// Append a new <see cref="ItemDef.Pair"/> to <see cref="ItemRelationshipProvider.relationships"/>.
        /// </summary>
        /// <returns>A reference to the new <see cref="ItemDef.Pair"/>.</returns>
        public static ref ItemDef.Pair AddRelationshipPair(this ItemRelationshipProvider itemRelationshipProvider, ItemDef item1, ItemDef item2)
        {
            int index = AddRelationshipPair(itemRelationshipProvider, new ItemDef.Pair
            {
                itemDef1 = item1,
                itemDef2 = item2
            });
            return ref itemRelationshipProvider.relationships[index];
        }

        /// <summary>
        /// Append this <paramref name="relationshipPair"/> to <see cref="ItemRelationshipProvider.relationships"/>.
        /// </summary>
        /// <returns>The index of <paramref name="relationshipPair"/> in <see cref="ItemRelationshipProvider.relationships"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AddRelationshipPair(this ItemRelationshipProvider itemRelationshipProvider, ItemDef.Pair relationshipPair)
        {
            ArrayUtils.ArrayAppend(ref itemRelationshipProvider.relationships, relationshipPair);
            return itemRelationshipProvider.relationships.Length - 1;
        }

        /// <summary>
        /// Add a new <see cref="ItemDisplayRule"/> to this <see cref="ItemDisplayRuleSet"/>.
        /// </summary>
        /// <remarks>
        /// This method will first search for an existing <see cref="ItemDisplayRuleSet.KeyAssetRuleGroup"/> that matches the key asset. 
        /// A new <see cref="ItemDisplayRuleSet.KeyAssetRuleGroup"/> is created if none are found.
        /// </remarks>
        /// <returns>A reference to the <see cref="ItemDisplayRuleSet.KeyAssetRuleGroup"/> that contains the new <see cref="ItemDisplayRule"/>.</returns>
        public static ref ItemDisplayRuleSet.KeyAssetRuleGroup AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec itemDisplay, ItemDisplayTransform itemDisplayTransform = default)
        {
            if (!itemDisplay.keyAsset)
            {
                throw new ArgumentException(nameof(itemDisplay));
            }
            ItemDisplayRule itemDisplayRule = new ItemDisplayRule
            {
                followerPrefab = itemDisplay.displayModelPrefab,
                childName = itemDisplayTransform.childName ?? "Base",
                localPos = itemDisplayTransform.localPos ?? Vector3.zero,
                localAngles = itemDisplayTransform.localAngles ?? Vector3.zero,
                localScale = itemDisplayTransform.localScale ?? Vector3.one,
                limbMask = itemDisplay.limbMask,
                ruleType = itemDisplay.limbMask > LimbFlags.None ? ItemDisplayRuleType.LimbMask : ItemDisplayRuleType.ParentedPrefab
            };
            for (int i = idrs.keyAssetRuleGroups.Length - 1; i >= 0; i--)
            {
                if (idrs.keyAssetRuleGroups[i].keyAsset == itemDisplay.keyAsset)
                {
                    idrs.keyAssetRuleGroups[i].displayRuleGroup.AddDisplayRule(itemDisplayRule);
                    return ref idrs.keyAssetRuleGroups[i];
                }
            }
            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = itemDisplay.keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new[] { itemDisplayRule }
                }
            });
            return ref idrs.keyAssetRuleGroups[idrs.keyAssetRuleGroups.Length - 1];
        }

        /// <inheritdoc cref="AddDisplayRule(ItemDisplayRuleSet, ItemDisplaySpec, ItemDisplayTransform)"/>
        public static ref ItemDisplayRuleSet.KeyAssetRuleGroup AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec itemDisplay, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            return ref AddDisplayRule(idrs, itemDisplay, new ItemDisplayTransform(childName, localPos, localAngles, localScale));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Get or create a component.
        /// </summary>
        /// <returns>The existing component of type <typeparamref name="T"/>, or a newly-added component of type <typeparamref name="T"/>.</returns>
        public static T RequireComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Get or create a component.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="component">The existing component of type <typeparamref name="T"/>, or a newly-added component of type <typeparamref name="T"/>.</param>
        public static void RequireComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Get or create a component.
        /// </summary>
        /// <returns>The existing component of type <paramref name="componentType"/>, or a newly-added component of type <paramref name="componentType"/>.</returns>
        public static Component RequireComponent(this GameObject gameObject, Type componentType)
        {
            return gameObject.GetComponent(componentType) ?? gameObject.AddComponent(componentType);
        }

        /// <inheritdoc cref="RequireComponent{T}(GameObject)"/>
        public static T RequireComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
        }

        /// <inheritdoc cref="RequireComponent{T}(GameObject, out T)"/>
        public static void RequireComponent<T>(this Component _component, out T component) where T : Component
        {
            component = _component.GetComponent<T>() ?? _component.gameObject.AddComponent<T>();
        }

        /// <inheritdoc cref="RequireComponent(GameObject, Type)"/>
        public static Component RequireComponent(this Component component, Type componentType)
        {
            return component.GetComponent(componentType) ?? component.gameObject.AddComponent(componentType);
        }

        /// <summary>
        /// Modify the value of a serialized field stored in this <see cref="EntityStateConfiguration"/>.
        /// </summary>
        public static bool TryModifyFieldValue<T>(this EntityStateConfiguration entityStateConfiguration, string fieldName, T value)
        {
            ref SerializedField serializedField = ref entityStateConfiguration.serializedFieldsCollection.GetOrCreateField(fieldName);
            Type type = typeof(T);
            if (serializedField.fieldValue.objectValue && typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                serializedField.fieldValue.objectValue = value as UnityEngine.Object;
                return true;
            }
            else if (serializedField.fieldValue.stringValue != null && StringSerializer.CanSerializeType(type))
            {
                serializedField.fieldValue.stringValue = StringSerializer.Serialize(type, value);
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFind(this Transform transform, string n, out Transform child)
        {
            return child = transform.Find(n);
        }

        public static IEnumerable<Transform> AllChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }

        public static bool HasItem(this CharacterBody characterBody, ItemIndex itemIndex, out int stack)
        {
            if (characterBody && characterBody.inventory)
            {
                return (stack = characterBody.inventory.GetItemCount(itemIndex)) > 0;
            }
            stack = 0;
            return false;
        }

        public static bool HasItem(this CharacterBody characterBody, ItemDef itemDef, out int stack) => HasItem(characterBody, itemDef ? itemDef.itemIndex : ItemIndex.None, out stack);

        public static bool HasItem(this CharacterBody characterBody, ItemIndex itemIndex) => characterBody && characterBody.inventory && characterBody.inventory.GetItemCount(itemIndex) > 0;

        public static bool HasItem(this CharacterBody characterBody, ItemDef itemDef) => HasItem(characterBody, itemDef.itemIndex);

        public static bool HasItem(this CharacterMaster characterMaster, ItemIndex itemIndex, out int stack)
        {
            if (characterMaster && characterMaster.inventory)
            {
                return (stack = characterMaster.inventory.GetItemCount(itemIndex)) > 0;
            }
            stack = 0;
            return false;
        }

        public static bool HasItem(this CharacterMaster characterMaster, ItemDef itemDef, out int stack) => HasItem(characterMaster, itemDef ? itemDef.itemIndex : ItemIndex.None, out stack);
        
        public static bool HasItem(this CharacterMaster characterMaster, ItemIndex itemIndex) => characterMaster && characterMaster.inventory && characterMaster.inventory.GetItemCount(itemIndex) > 0;

        public static bool HasItem(this CharacterMaster characterMaster, ItemDef itemDef) => HasItem(characterMaster, itemDef.itemIndex);

        public static bool HasBuff(this CharacterBody characterBody, BuffIndex buffType, out int count)
        {
            if (characterBody)
            {
                count = characterBody.GetBuffCount(buffType);
                return count > 0;
            }
            count = 0;
            return false;
        }

        public static bool HasBuff(this CharacterBody characterBody, BuffDef buffDef, out int count) => HasBuff(characterBody, buffDef ? buffDef.buffIndex : BuffIndex.None, out count);

        public static void ClearDotStacksForType(this DotController dotController, DotController.DotIndex dotIndex)
        {
            for (int i = dotController.dotStackList.Count - 1; i >= 0; i--)
            {
                if (dotController.dotStackList[i].dotIndex == dotIndex)
                {
                    dotController.RemoveDotStackAtServer(i);
                }
            }
        }

        /// <summary>
        /// Add this <see cref="IOnIncomingDamageServerReceiver"/> to <see cref="HealthComponent.onIncomingDamageReceivers"/>.
        /// </summary>
        public static void AddIncomingDamageReceiver(this HealthComponent healthComponent, IOnIncomingDamageServerReceiver onIncomingDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onIncomingDamageReceivers, onIncomingDamageReceiver) < 0)
            {
                ArrayUtils.ArrayAppend(ref healthComponent.onIncomingDamageReceivers, onIncomingDamageReceiver);
            }
        }

        /// <summary>
        /// Remove this <see cref="IOnIncomingDamageServerReceiver"/> from <see cref="HealthComponent.onIncomingDamageReceivers"/>.
        /// </summary>
        public static void RemoveIncomingDamageReceiver(this HealthComponent healthComponent, IOnIncomingDamageServerReceiver onIncomingDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onIncomingDamageReceivers, onIncomingDamageReceiver) is var index && index >= 0)
            {
                ArrayUtils.ArrayRemoveAtAndResize(ref healthComponent.onIncomingDamageReceivers, index);
            }
        }

        /// <summary>
        /// Add this <see cref="IOnTakeDamageServerReceiver"/> to <see cref="HealthComponent.onTakeDamageReceivers"/>.
        /// </summary>
        public static void AddTakeDamageReceiver(this HealthComponent healthComponent, IOnTakeDamageServerReceiver onTakeDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onTakeDamageReceivers, onTakeDamageReceiver) < 0)
            {
                ArrayUtils.ArrayAppend(ref healthComponent.onTakeDamageReceivers, onTakeDamageReceiver);
            }
        }

        /// <summary>
        /// Remove this <see cref="IOnTakeDamageServerReceiver"/> from <see cref="HealthComponent.onTakeDamageReceivers"/>.
        /// </summary>
        public static void RemoveTakeDamageReceiver(this HealthComponent healthComponent, IOnTakeDamageServerReceiver onTakeDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onTakeDamageReceivers, onTakeDamageReceiver) is var index && index >= 0)
            {
                ArrayUtils.ArrayRemoveAtAndResize(ref healthComponent.onTakeDamageReceivers, index);
            }
        }

        /// <summary>
        /// Asynchronously set the displayed <paramref name="artifactCode"/> of an <see cref="ArtifactFormulaDisplay"/> prefab.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> to be yielded in an <see cref="IContentPackProvider"/>.</returns>
        public static IEnumerator SetupArtifactFormulaDisplayAsync(ArtifactFormulaDisplay artifactFormulaDisplay, ArtifactCode artifactCode)
        {
            List<AsyncOperationHandle> loadArtifactCompoundsOperations = new List<AsyncOperationHandle>
            {
                FindArtifactCompoundDefAsync(artifactCode.topRow.Item1),
                FindArtifactCompoundDefAsync(artifactCode.topRow.Item2),
                FindArtifactCompoundDefAsync(artifactCode.topRow.Item3),
                FindArtifactCompoundDefAsync(artifactCode.middleRow.Item1),
                FindArtifactCompoundDefAsync(artifactCode.middleRow.Item2),
                FindArtifactCompoundDefAsync(artifactCode.middleRow.Item3),
                FindArtifactCompoundDefAsync(artifactCode.bottomRow.Item1),
                FindArtifactCompoundDefAsync(artifactCode.bottomRow.Item2),
                FindArtifactCompoundDefAsync(artifactCode.bottomRow.Item3),
            };
            var loadArtifactCompounds = Addressables.ResourceManager.CreateGenericGroupOperation(loadArtifactCompoundsOperations);
            if (!loadArtifactCompounds.IsDone)
            {
                yield return loadArtifactCompounds;
            }

            artifactFormulaDisplay.artifactCompoundDisplayInfos = new[]
            {
                GetDecalInfo("Slot 1,1"),
                GetDecalInfo("Slot 1,2"),
                GetDecalInfo("Slot 1,3"),
                GetDecalInfo("Slot 2,1"),
                GetDecalInfo("Slot 2,2"),
                GetDecalInfo("Slot 2,3"),
                GetDecalInfo("Slot 3,1"),
                GetDecalInfo("Slot 3,2"),
                GetDecalInfo("Slot 3,3"),
            };

            for (int i = 0; i < loadArtifactCompounds.Result.Count; i++)
            {
                artifactFormulaDisplay.artifactCompoundDisplayInfos[i].artifactCompoundDef = (ArtifactCompoundDef)loadArtifactCompounds.Result[i].Result;
            }

            static AsyncOperationHandle<ArtifactCompoundDef> FindArtifactCompoundDefAsync(ArtifactCompound artifactCompound) => artifactCompound switch
            {
                ArtifactCompound.Circle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdCircle.asset"),
                ArtifactCompound.Triangle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdTriangle.asset"),
                ArtifactCompound.Diamond => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdDiamond.asset"),
                ArtifactCompound.Square => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdSquare.asset"),
                ArtifactCompound.Empty => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdEmpty.asset"),
                _ => ArtifactCodeAPI.artifactCompounds.FirstOrDefault(x => x.value == (int)artifactCompound) is var artifactCompoundDef && artifactCompoundDef 
                ? Addressables.ResourceManager.CreateCompletedOperation(artifactCompoundDef, null) 
                : throw new ArgumentOutOfRangeException(nameof(artifactCompound))
            };

            ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo GetDecalInfo(string decalPath) => new ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo
            {
                decal = artifactFormulaDisplay.transform.Find(decalPath)?.GetComponent<Decal>()
            };
        }

        /// <summary>
        /// Immediately set the displayed <paramref name="artifactCode"/> of an <see cref="ArtifactFormulaDisplay"/> prefab.
        /// </summary>
        /// <remarks>
        /// This method will block the main thread until completed. <see cref="SetupArtifactFormulaDisplayAsync(ArtifactFormulaDisplay, ArtifactCode)"/> should be used instead.
        /// </remarks>
        [Obsolete($"{nameof(SetupArtifactFormulaDisplay)} is not asynchronous and may stall loading. {nameof(SetupArtifactFormulaDisplayAsync)} is preferred.", false)]
        public static void SetupArtifactFormulaDisplay(ArtifactFormulaDisplay artifactFormulaDisplay, ArtifactCode artifactCode)
        {
            artifactFormulaDisplay.artifactCompoundDisplayInfos = new[]
            {
                GetDisplayInfo(artifactCode.topRow.Item1, "Slot 1,1"),
                GetDisplayInfo(artifactCode.topRow.Item2, "Slot 1,2"),
                GetDisplayInfo(artifactCode.topRow.Item3, "Slot 1,3"),
                GetDisplayInfo(artifactCode.middleRow.Item1, "Slot 2,1"),
                GetDisplayInfo(artifactCode.middleRow.Item2, "Slot 2,2"),
                GetDisplayInfo(artifactCode.middleRow.Item3, "Slot 2,3"),
                GetDisplayInfo(artifactCode.bottomRow.Item1, "Slot 3,1"),
                GetDisplayInfo(artifactCode.bottomRow.Item2, "Slot 3,2"),
                GetDisplayInfo(artifactCode.bottomRow.Item3, "Slot 3,3"),
            };

            ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo GetDisplayInfo(ArtifactCompound artifactCompound, string decalPath) => new ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo
            {
                artifactCompoundDef = artifactCompound switch
                {
                    ArtifactCompound.Circle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdCircle.asset").WaitForCompletion(),
                    ArtifactCompound.Triangle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdTriangle.asset").WaitForCompletion(),
                    ArtifactCompound.Diamond => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdDiamond.asset").WaitForCompletion(),
                    ArtifactCompound.Square => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdSquare.asset").WaitForCompletion(),
                    ArtifactCompound.Empty => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdEmpty.asset").WaitForCompletion(),
                    _ => ArtifactCodeAPI.artifactCompounds.FirstOrDefault(x => x.value == (int)artifactCompound) is var artifactCompoundDef && artifactCompoundDef
                    ? artifactCompoundDef
                    : throw new ArgumentOutOfRangeException(nameof(artifactCompound))
                },
                decal = artifactFormulaDisplay.transform.Find(decalPath)?.GetComponent<Decal>()
            };
        }
    }
}