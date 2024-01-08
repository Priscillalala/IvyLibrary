using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.ContentManagement;
using HG;
using UnityEngine.AddressableAssets;
using R2API;
using System.Linq;
using RoR2.Skills;
using R2API.ScriptableObjects;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace IvyLibrary
{
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
            On.RoR2.Util.IsPrefab += (orig, gameObject) => gameObject.transform.parent == _prefabParent || orig(gameObject);
        }

        public static GameObject CreatePrefab(string name)
        {
            InitPrefabParent();
            GameObject prefab = new GameObject(name);
            prefab.transform.SetParent(_prefabParent);
            return prefab;
        }

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

        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, Vector3 modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
        {
            return SetupModelPanelParameters(model, Quaternion.Euler(modelRotation), minDistance, maxDistance, focusPoint, cameraPosition);
        }

        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, ModelPanelParams info)
        {
            return SetupModelPanelParameters(model, info.modelRotation, info.minDistance, info.maxDistance, info.focusPoint, info.cameraPosition);
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float StackScaling(float baseValue, float stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StackScaling(int baseValue, int stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0;
        }

        public static AssetBundleRequest<T> Convert<T>(this AssetBundleRequest request) where T : UnityEngine.Object
        {
            return new AssetBundleRequest<T>(request);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncOperationHandle LoadAddressableAssetAsync<TObject>(object key, out AsyncOperationHandle<TObject> handle)
        {
            return handle = Addressables.LoadAssetAsync<TObject>(key);
        }

        public static AsyncOperationHandle LoadAddressableAssetsAsync<TObject>(IEnumerable keys, out AsyncOperationHandle<IDictionary<string, TObject>> handle)
        {
            var loadLocations = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Union, typeof(TObject));
            var loadAssets = Addressables.ResourceManager.CreateChainOperation<IList<TObject>>(loadLocations, x =>
            {
                return Addressables.LoadAssetsAsync<TObject>(loadLocations.Result, null, false);
            });
            return handle = Addressables.ResourceManager.CreateChainOperation<IDictionary<string, TObject>>(loadAssets, x =>
            {
                Dictionary<string, TObject> dict = new Dictionary<string, TObject>();
                for (int i = 0; i < loadAssets.Result.Count; i++)
                {
                    string key = loadLocations.Result[i].PrimaryKey;
                    dict[key] = dict[System.IO.Path.GetFileNameWithoutExtension(key)] = loadAssets.Result[i];
                }
                return Addressables.ResourceManager.CreateCompletedOperation<IDictionary<string, TObject>>(dict, null);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssetBundleRequest LoadAssetAsync<T>(this AssetBundle assetBundle, string name, out AssetBundleRequest<T> request) where T : UnityEngine.Object
        {
            return request = assetBundle.LoadAssetAsync<T>(name).Convert<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Value<T>(this ConfigFile config, string section, string key, T defaultValue, string description)
        {
            return config.Bind(section, key, defaultValue, description).Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Value<T>(this ConfigFile config, string section, string key, T defaultValue, ConfigDescription configDescription = null)
        {
            return config.Bind(section, key, defaultValue, configDescription).Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Value<T>(this ConfigFile config, ConfigDefinition configDefinition, T defaultValue, ConfigDescription configDescription = null)
        {
            return config.Bind(configDefinition, defaultValue, configDescription).Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsModLoaded(string guid) => Chainloader.PluginInfos.ContainsKey(guid);

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

        public static ref SkillFamily.Variant AddSkill(this SkillFamily skillFamily, SkillDef skillDef, UnlockableDef requiredUnlockable = null)
        {
            ArrayUtils.ArrayAppend(ref skillFamily.variants, new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = requiredUnlockable,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillName, false, null),
            });
            return ref skillFamily.variants[skillFamily.variants.Length - 1];
        }

        public static ref SkillFamily.Variant InsertSkill(this SkillFamily skillFamily, int index, SkillDef skillDef, UnlockableDef requiredUnlockable = null)
        {
            ArrayUtils.ArrayInsert(ref skillFamily.variants, index, new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = requiredUnlockable,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillName, false, null),
            });
            return ref skillFamily.variants[index];
        }

        public static int AddElite(this CombatDirector.EliteTierDef eliteTierDef, EliteDef elite)
        {
            ArrayUtils.ArrayAppend(ref eliteTierDef.eliteTypes, elite);
            return eliteTierDef.eliteTypes.Length - 1;
        }

        public static int AddElite(this CombatDirector.EliteTierDef eliteTierDef, EliteWrapper elite)
        {
            return AddElite(eliteTierDef, elite.EliteDef);
        }

        public static ref SceneCollection.SceneEntry AddScene(this SceneCollection sceneCollection, SceneDef scene, float weight = 1f)
        {
            ArrayUtils.ArrayAppend(ref sceneCollection._sceneEntries, new SceneCollection.SceneEntry
            {
                sceneDef = scene,
                weight = weight,
            });
            return ref sceneCollection._sceneEntries[sceneCollection._sceneEntries.Length - 1];
        }

        public static ref ItemDef.Pair AddRelationshipPair(this ItemRelationshipProvider itemRelationshipProvider, ItemDef item1, ItemDef item2)
        {
            return ref itemRelationshipProvider.relationships[AddRelationshipPair(itemRelationshipProvider, new ItemDef.Pair
            {
                itemDef1 = item1,
                itemDef2 = item2
            })];
        }

        public static int AddRelationshipPair(this ItemRelationshipProvider itemRelationshipProvider, ItemDef.Pair relationshipPair)
        {
            ArrayUtils.ArrayAppend(ref itemRelationshipProvider.relationships, relationshipPair);
            return itemRelationshipProvider.relationships.Length - 1;
        }

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
            for (int i = 0; i < idrs.keyAssetRuleGroups.Length; i++)
            {
                if (idrs.keyAssetRuleGroups[i].keyAsset == itemDisplay.keyAsset)
                {
                    idrs.keyAssetRuleGroups[i].displayRuleGroup.AddDisplayRule(itemDisplayRule);
                    return ref idrs.keyAssetRuleGroups[i];
                }
            }
            ItemDisplayRuleSet.KeyAssetRuleGroup keyAssetRuleGroup = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = itemDisplay.keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new[] { itemDisplayRule }
                }
            };
            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, keyAssetRuleGroup);
            return ref idrs.keyAssetRuleGroups[idrs.keyAssetRuleGroups.Length - 1];
        }

        public static ref ItemDisplayRuleSet.KeyAssetRuleGroup AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec itemDisplay, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            return ref AddDisplayRule(idrs, itemDisplay, new ItemDisplayTransform(childName, localPos, localAngles, localScale));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.AddComponent<T>();
        }

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

        public static void AddIncomingDamageReceiver(this HealthComponent healthComponent, IOnIncomingDamageServerReceiver onIncomingDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onIncomingDamageReceivers, onIncomingDamageReceiver) < 0)
            {
                ArrayUtils.ArrayAppend(ref healthComponent.onIncomingDamageReceivers, onIncomingDamageReceiver);
            }
        }

        public static void RemoveIncomingDamageReceiver(this HealthComponent healthComponent, IOnIncomingDamageServerReceiver onIncomingDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onIncomingDamageReceivers, onIncomingDamageReceiver) is var index && index >= 0)
            {
                ArrayUtils.ArrayRemoveAtAndResize(ref healthComponent.onIncomingDamageReceivers, index);
            }
        }

        public static void AddTakeDamageReceiver(this HealthComponent healthComponent, IOnTakeDamageServerReceiver onTakeDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onTakeDamageReceivers, onTakeDamageReceiver) < 0)
            {
                ArrayUtils.ArrayAppend(ref healthComponent.onTakeDamageReceivers, onTakeDamageReceiver);
            }
        }

        public static void RemoveTakeDamageReceiver(this HealthComponent healthComponent, IOnTakeDamageServerReceiver onTakeDamageReceiver)
        {
            if (healthComponent && Array.IndexOf(healthComponent.onTakeDamageReceivers, onTakeDamageReceiver) is var index && index >= 0)
            {
                ArrayUtils.ArrayRemoveAtAndResize(ref healthComponent.onTakeDamageReceivers, index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssetBundle LoadAssetBundle(this BaseUnityPlugin plugin, string relativePath)
        {
            return AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(plugin.Info.Location), relativePath));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AssetBundleCreateRequest LoadAssetBundleAsync(this BaseUnityPlugin plugin, string relativePath)
        {
            return AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(plugin.Info.Location), relativePath));
        }

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