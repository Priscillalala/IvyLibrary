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
using System.Threading.Tasks;

[module: UnverifiableCode]
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace Ivyl
{
    public static class Ivyl
    {
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

        public static SkillFamily FindSkillFamily(GameObject bodyPrefab, SkillSlot slot)
        {
            if (bodyPrefab.TryGetComponent(out SkillLocator skillLocator))
            {
                return skillLocator.GetSkill(slot)?.skillFamily;
            }
            return null;
        }

        public static float StackScaling(float baseValue, float stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0f;
        }

        public static int StackScaling(int baseValue, int stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0;
        }

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

        public static int AddScene(this SceneCollection sceneCollection, SceneDef scene, float weight = 1f)
        {
            ArrayUtils.ArrayAppend(ref sceneCollection._sceneEntries, new SceneCollection.SceneEntry
            {
                sceneDef = scene,
                weight = weight,
            });
            return sceneCollection._sceneEntries.Length - 1;
        }

        public static int AddRelationshipPair(this ItemRelationshipProvider itemRelationshipProvider, ItemDef item1, ItemDef item2)
        {
            return AddRelationshipPair(itemRelationshipProvider, new ItemDef.Pair
            {
                itemDef1 = item1,
                itemDef2 = item2
            });
        }

        public static int AddRelationshipPair(this ItemRelationshipProvider itemRelationshipProvider, ItemDef.Pair relationshipPair)
        {
            ArrayUtils.ArrayAppend(ref itemRelationshipProvider.relationships, relationshipPair);
            return itemRelationshipProvider.relationships.Length - 1;
        }

        public static int AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec itemDisplay, ItemDisplayTransform itemDisplayTransform = default)
        {
            if (!itemDisplay.keyAsset)
            {
                throw new ArgumentException(nameof(itemDisplay));
            }
            ItemDisplayRule itemDisplayRule = new ItemDisplayRule
            {
                followerPrefab = itemDisplay.displayModelPrefab,
                childName = itemDisplayTransform.childName ?? (idrs == Idrs.EquipmentDrone ? "GunBarrelBase" : "Base"),
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
                    return i;
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
            return idrs.keyAssetRuleGroups.Length - 1;
        }

        public static int AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec itemDisplay, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            return AddDisplayRule(idrs, itemDisplay, new ItemDisplayTransform(childName, localPos, localAngles, localScale));
        }

        public static int[] AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec[] itemDisplays, ItemDisplayTransform itemDisplayTransform = default)
        {
            int[] result = new int[itemDisplays.Length];
            for (int i = 0; i < itemDisplays.Length; i++)
            {
                result[i] = AddDisplayRule(idrs, itemDisplays[i], itemDisplayTransform);
            }
            return result;
        }

        public static int[] AddDisplayRule(this ItemDisplayRuleSet idrs, ItemDisplaySpec[] itemDisplays, string childName, Vector3 localPos, Vector3 localAngles, Vector3 localScale)
        {
            ItemDisplayTransform itemDisplayTransform = new ItemDisplayTransform(childName, localPos, localAngles, localScale);
            int[] result = new int[itemDisplays.Length];
            for (int i = 0; i < itemDisplays.Length; i++)
            {
                result[i] = AddDisplayRule(idrs, itemDisplays[i], itemDisplayTransform);
            }
            return result;
        }

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

        public static bool HasItem(this CharacterBody characterBody, ItemDef itemDef, out int stack) => HasItem(characterBody, itemDef ? itemDef.itemIndex : ItemIndex.None, out stack);

        public static bool HasItem(this CharacterBody characterBody, ItemIndex itemIndex, out int stack)
        {
            if (characterBody && characterBody.inventory)
            {
                stack = characterBody.inventory.GetItemCount(itemIndex);
                return stack > 0;
            }
            stack = 0;
            return false;
        }

        public static bool HasItem(this CharacterBody characterBody, ItemDef itemDef) => HasItem(characterBody, itemDef.itemIndex);

        public static bool HasItem(this CharacterBody characterBody, ItemIndex itemIndex) => characterBody && characterBody.inventory && characterBody.inventory.GetItemCount(itemIndex) > 0;

        public static bool HasItem(this CharacterMaster characterMaster, ItemDef itemDef, out int stack) => HasItem(characterMaster, itemDef ? itemDef.itemIndex : ItemIndex.None, out stack);

        public static bool HasItem(this CharacterMaster characterMaster, ItemIndex itemIndex, out int stack)
        {
            if (characterMaster && characterMaster.inventory)
            {
                stack = characterMaster.inventory.GetItemCount(itemIndex);
                return stack > 0;
            }
            stack = 0;
            return false;
        }

        public static bool HasBuff(this CharacterBody characterBody, BuffDef buffDef, out int count) => HasBuff(characterBody, buffDef ? buffDef.buffIndex : BuffIndex.None, out count);

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

        public static bool HasItem(this CharacterMaster characterMaster, ItemDef itemDef) => HasItem(characterMaster, itemDef.itemIndex);

        public static bool HasItem(this CharacterMaster characterMaster, ItemIndex itemIndex) => characterMaster && characterMaster.inventory && characterMaster.inventory.GetItemCount(itemIndex) > 0;

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

        public static AssetBundle LoadAssetBundle(this BaseUnityPlugin plugin, string relativePath)
        {
            return AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(plugin.Info.Location), relativePath));
        }

        public static AssetBundleCreateRequest LoadAssetBundleAsync(this BaseUnityPlugin plugin, string relativePath)
        {
            return AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(plugin.Info.Location), relativePath));
        }

        public static ArtifactCompoundDef FindArtifactCompoundDef(this ArtifactCompound artifactCompound)
        {
            return artifactCompound switch
            {
                ArtifactCompound.Circle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdCircle.asset").WaitForCompletion(),
                ArtifactCompound.Triangle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdTriangle.asset").WaitForCompletion(),
                ArtifactCompound.Diamond => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdDiamond.asset").WaitForCompletion(),
                ArtifactCompound.Square => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdSquare.asset").WaitForCompletion(),
                ArtifactCompound.Empty => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdEmpty.asset").WaitForCompletion(),
                _ => ArtifactCodeAPI.artifactCompounds.FirstOrDefault(x => x.value == (int)artifactCompound)
            };
        }

        public static async Task<ArtifactCompoundDef> FindArtifactCompoundDefAsync(this ArtifactCompound artifactCompound)
        {
            return artifactCompound switch
            {
                ArtifactCompound.Circle => await Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdCircle.asset").Task,
                ArtifactCompound.Triangle => await Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdTriangle.asset").Task,
                ArtifactCompound.Diamond => await Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdDiamond.asset").Task,
                ArtifactCompound.Square => await Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdSquare.asset").Task,
                ArtifactCompound.Empty => await Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdEmpty.asset").Task,
                _ => ArtifactCodeAPI.artifactCompounds.FirstOrDefault(x => x.value == (int)artifactCompound)
            };
        }

        public static bool TryFindArtifactCompoundDef(ArtifactCompound artifactCompound, out ArtifactCompoundDef artifactCompoundDef)
        {
            return artifactCompoundDef = FindArtifactCompoundDef(artifactCompound);
        }
    }
}