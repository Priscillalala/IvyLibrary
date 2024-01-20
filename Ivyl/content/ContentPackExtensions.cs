using System;
using RoR2;
using UnityEngine;
using RoR2.ContentManagement;
using RoR2.Skills;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using R2API;
using System.Reflection;
using System.Linq;
using EntityStates;
using R2API.ScriptableObjects;
using HG;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using UnityEngine.AddressableAssets;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for interacting with a <see cref="ContentPack"/>, primarily defining new content. 
    /// </summary>
    public static class ContentPackExtensions
    {
        /// <summary>
        /// Generate unique asset ids for all networked objects in this <see cref="ContentPack"/>.
        /// </summary>
        public static void PopulateNetworkedObjectAssetIds(this ContentPack contentPack)
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            using (MD5 hasher = MD5.Create())
            {
                PopulateAssetIds(contentPack.bodyPrefabs, contentPack.identifier, nameof(ContentPack.bodyPrefabs), stringBuilder, hasher);
                PopulateAssetIds(contentPack.masterPrefabs, contentPack.identifier, nameof(ContentPack.masterPrefabs), stringBuilder, hasher);
                PopulateAssetIds(contentPack.projectilePrefabs, contentPack.identifier, nameof(ContentPack.projectilePrefabs), stringBuilder, hasher);
                PopulateAssetIds(contentPack.networkedObjectPrefabs, contentPack.identifier, nameof(ContentPack.networkedObjectPrefabs), stringBuilder, hasher);
                PopulateAssetIds(contentPack.gameModePrefabs, contentPack.identifier, nameof(ContentPack.gameModePrefabs), stringBuilder, hasher);
            }

            static void PopulateAssetIds(NamedAssetCollection<GameObject> assets, string contentIdentifier, string collectionIdentifier, StringBuilder stringBuilder, HashAlgorithm hasher)
            {
                for (int i = 0; i < assets.Length; i++)
                {
                    NamedAssetCollection<GameObject>.AssetInfo assetInfo = assets.assetInfos[i];
                    if (assetInfo.asset.TryGetComponent(out NetworkIdentity networkIdentity) && !networkIdentity.assetId.IsValid())
                    {
                        stringBuilder.Clear();
                        foreach (byte b in hasher.ComputeHash(Encoding.UTF8.GetBytes(assetInfo.assetName + contentIdentifier + collectionIdentifier)))
                        {
                            stringBuilder.Append(b.ToString("x2"));
                        }
                        networkIdentity.SetDynamicAssetId(NetworkHash128.Parse(stringBuilder.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Add all types that inherit from <see cref="EntityState"/> in <paramref name="assembly"/> to <see cref="ContentPack.entityStateTypes"/>.
        /// </summary>
        public static void AddEntityStatesFromAssembly(this ContentPack contentPack, Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsSubclassOf(typeof(EntityState)))
                {
                    contentPack.entityStateTypes.Add(types[i]);
                }
            }
        }

        /// <summary>
        /// Add content from an <see cref="R2APISerializableContentPack"/> to this <see cref="ContentPack"/>.
        /// </summary>
        public static void AddSerializedContent(this ContentPack contentPack, R2APISerializableContentPack serializableContent)
        {
            typeof(R2APISerializableContentPack).GetMethod("EnsureNoFieldsAreNull", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(serializableContent, null);

            contentPack.bodyPrefabs.Add(serializableContent.bodyPrefabs);
            contentPack.masterPrefabs.Add(serializableContent.masterPrefabs);
            contentPack.projectilePrefabs.Add(serializableContent.projectilePrefabs);
            contentPack.gameModePrefabs.Add(serializableContent.gameModePrefabs);
            contentPack.effectDefs.Add(serializableContent.effectPrefabs.Select(x => new EffectDef(x)).ToArray());
            contentPack.networkedObjectPrefabs.Add(serializableContent.networkedObjectPrefabs);
            contentPack.skillDefs.Add(serializableContent.skillDefs);
            contentPack.skillFamilies.Add(serializableContent.skillFamilies);
            contentPack.sceneDefs.Add(serializableContent.sceneDefs);
            contentPack.itemDefs.Add(serializableContent.itemDefs);
            contentPack.itemTierDefs.Add(serializableContent.itemTierDefs);
            contentPack.itemRelationshipTypes.Add(serializableContent.itemRelationshipTypes);
            contentPack.equipmentDefs.Add(serializableContent.equipmentDefs);
            contentPack.buffDefs.Add(serializableContent.buffDefs);
            contentPack.eliteDefs.Add(serializableContent.eliteDefs);
            contentPack.unlockableDefs.Add(serializableContent.unlockableDefs);
            contentPack.survivorDefs.Add(serializableContent.survivorDefs);
            contentPack.artifactDefs.Add(serializableContent.artifactDefs);
            contentPack.surfaceDefs.Add(serializableContent.surfaceDefs);
            contentPack.networkSoundEventDefs.Add(serializableContent.networkSoundEventDefs);
            contentPack.musicTrackDefs.Add(serializableContent.musicTrackDefs);
            contentPack.gameEndingDefs.Add(serializableContent.gameEndingDefs);
            contentPack.entityStateConfigurations.Add(serializableContent.entityStateConfigurations);
            contentPack.expansionDefs.Add(serializableContent.expansionDefs);
            contentPack.entitlementDefs.Add(serializableContent.entitlementDefs);
            contentPack.miscPickupDefs.Add(serializableContent.miscPickupDefs);

            HashSet<Type> entityStateTypes = new HashSet<Type>();
            for (int i = 0; i < serializableContent.entityStateTypes.Length; i++)
            {
                Type stateType = serializableContent.entityStateTypes[i].stateType;
                if (stateType != null)
                {
                    entityStateTypes.Add(stateType);
                }
                else
                {
                    Debug.LogWarning("SerializableContentPack \"" + serializableContent.name + "\" could not resolve type with name \"" + serializableContent.entityStateTypes[i].typeName + "\". The type will not be available in the content pack.");
                }
            }
            contentPack.entityStateTypes.Add(entityStateTypes.ToArray());
        }

        /// <summary>
        /// Create an <see cref="EffectDef"/> for <paramref name="effectPrefab"/> and add it to <see cref="ContentPack.effectDefs"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddEffectPrefab(this ContentPack contentPack, GameObject effectPrefab)
        {
            contentPack.effectDefs.Add(new EffectDef(effectPrefab));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPrefix(ContentPack contentPack)
        {
            return contentPack.identifier.Substring(contentPack.identifier.LastIndexOf('.') + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string FormatToken(string baseToken, string tokenPrefix)
        {
            return tokenPrefix + '_' + baseToken;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string FormatAssetIdentifier(string baseIdentifier, string prefix)
        {
            return prefix + '.' + baseIdentifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetUnlockableIdentifier(string baseIdentifier, UnlockableType unlockableType)
        {
            if (!string.IsNullOrWhiteSpace((string)unlockableType))
            {
                baseIdentifier = (string)unlockableType + '.' + baseIdentifier;
            }
            return baseIdentifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssignRequiredExpansion(ref ExpansionDef requiredExpansion, ContentPack contentPack)
        {
            if (contentPack.expansionDefs.Length > 0)
            {
                requiredExpansion = contentPack.expansionDefs[0];
            }
        }

        /// <summary>
        /// Defines an <see cref="ExpansionDef"/> for this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Multiple calls on the same <see cref="ContentPack"/> will fail. Additional expansions must be defined manually.
        /// </remarks>
        /// <returns>The newly-created <see cref="ExpansionDef"/>.</returns>
        public static ExpansionDef DefineExpansion(this ContentPack contentPack) => DefineExpansion<ExpansionDef>(contentPack);

        /// <inheritdoc cref="DefineExpansion(ContentPack)"/>
        public static TExpansionDef DefineExpansion<TExpansionDef>(this ContentPack contentPack) where TExpansionDef : ExpansionDef
        {
            TExpansionDef expansion = ScriptableObject.CreateInstance<TExpansionDef>();
            expansion.name = contentPack.identifier;
            string token = contentPack.identifier.ToUpperInvariant().Replace('.', '_');
            expansion.nameToken = token + "_NAME";
            expansion.descriptionToken = token + "_DESCRIPTION";
            Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texUnlockIcon.png").Completed += texUnlockIcon =>
            {
                expansion.disabledIconSprite ??= texUnlockIcon.Result;
            };
            contentPack.expansionDefs.Add(expansion);
            return expansion;
        }

        /// <summary>
        /// Defines a new <see cref="ItemDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the item.
        /// The item is added to the first <see cref="ExpansionDef"/> in <see cref="ContentPack.expansionDefs"/>, if present.
        /// The item is added to <see cref="ContentPack.itemDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this item.</param>
        /// <returns>The newly-created <see cref="ItemDef"/>.</returns>
        public static ItemDef DefineItem(this ContentPack contentPack, string identifier) => DefineItem<ItemDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineItem(ContentPack, string)"/>
        public static TItemDef DefineItem<TItemDef>(this ContentPack contentPack, string identifier) where TItemDef : ItemDef
        {
            TItemDef item = ScriptableObject.CreateInstance<TItemDef>();
            string prefix = GetPrefix(contentPack);
            item.name = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            item.nameToken = FormatToken($"ITEM_{token}_NAME", tokenPrefix);
            item.pickupToken = FormatToken($"ITEM_{token}_PICKUP", tokenPrefix);
            item.descriptionToken = FormatToken($"ITEM_{token}_DESC", tokenPrefix);
            item.loreToken = FormatToken($"ITEM_{token}_LORE", tokenPrefix);
            AssignRequiredExpansion(ref item.requiredExpansion, contentPack);
            contentPack.itemDefs.Add(item);
            return item;
        }

        /// <summary>
        /// Defines a new <see cref="EquipmentDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the equipment.
        /// The equipment is added to the first <see cref="ExpansionDef"/> in <see cref="ContentPack.expansionDefs"/>, if present.
        /// The equipment is added to <see cref="ContentPack.equipmentDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this equipment.</param>
        /// <returns>The newly-created <see cref="EquipmentDef"/>.</returns>
        public static EquipmentDef DefineEquipment(this ContentPack contentPack, string identifier) => DefineEquipment<EquipmentDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineEquipment(ContentPack, string)"/>
        public static TEquipmentDef DefineEquipment<TEquipmentDef>(this ContentPack contentPack, string identifier) where TEquipmentDef : EquipmentDef
        {
            TEquipmentDef equipment = ScriptableObject.CreateInstance<TEquipmentDef>();
            string prefix = GetPrefix(contentPack);
            equipment.name = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            equipment.nameToken = FormatToken($"EQUIPMENT_{token}_NAME", tokenPrefix);
            equipment.pickupToken = FormatToken($"EQUIPMENT_{token}_PICKUP", tokenPrefix);
            equipment.descriptionToken = FormatToken($"EQUIPMENT_{token}_DESC", tokenPrefix);
            equipment.loreToken = FormatToken($"EQUIPMENT_{token}_LORE", tokenPrefix);
            equipment.canDrop = true;
            equipment.enigmaCompatible = true;
            AssignRequiredExpansion(ref equipment.requiredExpansion, contentPack);
            contentPack.equipmentDefs.Add(equipment);
            return equipment;
        }

        /// <summary>
        /// Defines a new <see cref="BuffDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// The buff is added to <see cref="ContentPack.buffDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this buff.</param>
        /// <returns>The newly-created <see cref="BuffDef"/>.</returns>
        public static BuffDef DefineBuff(this ContentPack contentPack, string identifier) => DefineBuff<BuffDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineBuff(ContentPack, string)"/>
        public static TBuffDef DefineBuff<TBuffDef>(this ContentPack contentPack, string identifier) where TBuffDef : BuffDef
        {
            TBuffDef buff = ScriptableObject.CreateInstance<TBuffDef>();
            buff.name = FormatAssetIdentifier(identifier, GetPrefix(contentPack));
            contentPack.buffDefs.Add(buff);
            return buff;
        }

        /// <summary>
        /// Defines a new <see cref="ArtifactDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the artifact.
        /// The artifact is added to the first <see cref="ExpansionDef"/> in <see cref="ContentPack.expansionDefs"/>, if present.
        /// The artifact is added to <see cref="ContentPack.artifactDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this artifact.</param>
        /// <returns>The newly-created <see cref="ArtifactDef"/>.</returns>
        public static ArtifactDef DefineArtifact(this ContentPack contentPack, string identifier) => DefineArtifact<ArtifactDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineArtifact(ContentPack, string)"/>
        public static TArtifactDef DefineArtifact<TArtifactDef>(this ContentPack contentPack, string identifier) where TArtifactDef : ArtifactDef
        {
            TArtifactDef artifact = ScriptableObject.CreateInstance<TArtifactDef>();
            string prefix = GetPrefix(contentPack);
            artifact.cachedName = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            artifact.nameToken = FormatToken($"ARTIFACT_{token}_NAME", tokenPrefix);
            artifact.descriptionToken = FormatToken($"ARTIFACT_{token}_DESCRIPTION", tokenPrefix);
            AssignRequiredExpansion(ref artifact.requiredExpansion, contentPack);
            contentPack.artifactDefs.Add(artifact);
            return artifact;
        }

        /// <summary>
        /// Defines a new <see cref="SkillDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the skill.
        /// The skill is added to <see cref="ContentPack.skillDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this skill.</param>
        /// <returns>The newly-created <see cref="SkillDef"/>.</returns>
        public static SkillDef DefineSkill(this ContentPack contentPack, string identifier) => DefineSkill<SkillDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineSkill(ContentPack, string)"/>
        public static TSceneDef DefineSkill<TSceneDef>(this ContentPack contentPack, string identifier) where TSceneDef : SkillDef
        {
            TSceneDef skill = ScriptableObject.CreateInstance<TSceneDef>();
            string prefix = GetPrefix(contentPack);
            skill.skillName = FormatAssetIdentifier(identifier, prefix);
            (skill as ScriptableObject).name = skill.skillName;
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            skill.skillNameToken = FormatToken($"SKILL_{token}_NAME", tokenPrefix);
            skill.skillDescriptionToken = FormatToken($"SKILL_{token}_DESC", tokenPrefix);
            contentPack.skillDefs.Add(skill);
            return skill;
        }

        /// <summary>
        /// Defines a new <see cref="GameEndingDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the game ending.
        /// The game ending is added to <see cref="ContentPack.gameEndingDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this game ending.</param>
        /// <returns>The newly-created <see cref="GameEndingDef"/>.</returns>
        public static GameEndingDef DefineGameEnding(this ContentPack contentPack, string identifier) => DefineGameEnding<GameEndingDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineGameEnding(ContentPack, string)"/>
        public static TGameEndingDef DefineGameEnding<TGameEndingDef>(this ContentPack contentPack, string identifier) where TGameEndingDef : GameEndingDef
        {
            TGameEndingDef gameEnding = ScriptableObject.CreateInstance<TGameEndingDef>();
            string prefix = GetPrefix(contentPack);
            gameEnding.cachedName = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            gameEnding.endingTextToken = FormatToken($"GAME_RESULT_{token}", tokenPrefix);
            contentPack.gameEndingDefs.Add(gameEnding);
            return gameEnding;
        }

        /// <summary>
        /// Defines a new <see cref="SurfaceDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// The surface is added to <see cref="ContentPack.surfaceDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this surface.</param>
        /// <returns>The newly-created <see cref="SurfaceDef"/>.</returns>
        public static SurfaceDef DefineSurfaceDef(this ContentPack contentPack, string identifier) => DefineSurfaceDef<SurfaceDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineSurfaceDef(ContentPack, string)"/>
        public static TSurfaceDef DefineSurfaceDef<TSurfaceDef>(this ContentPack contentPack, string identifier) where TSurfaceDef : SurfaceDef
        {
            TSurfaceDef surface = ScriptableObject.CreateInstance<TSurfaceDef>();
            surface.name = FormatAssetIdentifier(identifier, GetPrefix(contentPack));
            contentPack.surfaceDefs.Add(surface);
            return surface;
        }

        /// <summary>
        /// Defines a new <see cref="SurvivorDef"/> in this <see cref="ContentPack"/> from the specified <paramref name="bodyPrefab"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the survivor.
        /// The survivor is added to <see cref="ContentPack.survivorDefs"/>.
        /// The survivor's body prefab is set to <paramref name="bodyPrefab"/>.
        /// The survivor's color is inferred from the <paramref name="bodyPrefab"/> color.
        /// <paramref name="bodyPrefab"/> is given an <see cref="ExpansionRequirementComponent"/> for the first <see cref="ExpansionDef"/> in <see cref="ContentPack.expansionDefs"/>, if present.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this survivor.</param>
        /// <param name="bodyPrefab">The body prefab of this survivor, with a <see cref="CharacterBody"/> component.</param>
        /// <returns>The newly-created <see cref="SurvivorDef"/>.</returns>
        public static SurvivorDef DefineSurvivorFromBodyPrefab(this ContentPack contentPack, string identifier, GameObject bodyPrefab) => DefineSurvivorFromBodyPrefab<SurvivorDef>(contentPack, identifier, bodyPrefab);

        /// <inheritdoc cref="DefineSurvivorFromBodyPrefab(ContentPack, string, GameObject)"/>
        public static TSurvivorDef DefineSurvivorFromBodyPrefab<TSurvivorDef>(this ContentPack contentPack, string identifier, GameObject bodyPrefab) where TSurvivorDef : SurvivorDef
        {
            TSurvivorDef survivor = DefineSurvivorImpl<TSurvivorDef>(identifier, contentPack);
            survivor.bodyPrefab = bodyPrefab;
            if (survivor.bodyPrefab.TryGetComponent(out CharacterBody characterBody))
            {
                survivor.displayNameToken = characterBody.baseNameToken;
                survivor.primaryColor = characterBody.bodyColor;
            }
            ExpansionDef requiredExpansion = null;
            AssignRequiredExpansion(ref requiredExpansion, contentPack);
            if (requiredExpansion != null)
            {
                (survivor.bodyPrefab.GetComponent<ExpansionRequirementComponent>() ?? survivor.bodyPrefab.AddComponent<ExpansionRequirementComponent>()).requiredExpansion = requiredExpansion;
            }
            return survivor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TSurvivorDef DefineSurvivorImpl<TSurvivorDef>(string identifier, ContentPack contentPack) where TSurvivorDef : SurvivorDef
        {
            TSurvivorDef survivor = ScriptableObject.CreateInstance<TSurvivorDef>();
            string prefix = GetPrefix(contentPack);
            survivor.cachedName = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            survivor.displayNameToken = FormatToken($"{token}_BODY_NAME", tokenPrefix);
            survivor.descriptionToken = FormatToken($"{token}_DESCRIPTION", tokenPrefix);
            survivor.outroFlavorToken = FormatToken($"{token}_OUTRO_FLAVOR", tokenPrefix);
            survivor.outroFlavorToken = FormatToken($"{token}_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR", tokenPrefix);
            contentPack.survivorDefs.Add(survivor);
            return survivor;
        }

        /// <summary>
        /// Defines a new <see cref="SkinDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="bodyPrefab"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the skin.
        /// The root object of the skin is set to the model transform of <paramref name="bodyPrefab"/>.
        /// The skin is appended to the <paramref name="bodyPrefab"/> <see cref="ModelSkinController"/>.
        /// If the <see cref="ModelSkinController"/> is missing or has no skins, the new skin is set as the default skin.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this skin.</param>
        /// <param name="bodyPrefab">The body prefab to recieve this skin, with a valid <see cref="ModelLocator"/> component.</param>
        /// <returns>The newly-created <see cref="SkinDef"/>.</returns>
        public static SkinDef DefineSkinForBodyPrefab(this ContentPack contentPack, string identifier, GameObject bodyPrefab) => DefineSkinForBodyPrefab<SkinDef>(contentPack, identifier, bodyPrefab);

        /// <inheritdoc cref="DefineSkinForBodyPrefab(ContentPack, string, GameObject)"/>
        public static TSkinDef DefineSkinForBodyPrefab<TSkinDef>(this ContentPack contentPack, string identifier, GameObject bodyPrefab) where TSkinDef : SkinDef
        {
            if (!bodyPrefab.TryGetComponent(out ModelLocator modelLocator) || !modelLocator.modelTransform)
            {
                throw new ArgumentException(nameof(bodyPrefab));
            }
            TSkinDef skin = DefineSkin<TSkinDef>(contentPack, identifier);
            skin.rootObject = modelLocator.modelTransform.gameObject;
            if (modelLocator.modelTransform.TryGetComponent(out ModelSkinController modelSkinController) && modelSkinController.skins != null)
            {
                ArrayUtils.ArrayAppend(ref modelSkinController.skins, skin);
            }
            else
            {
                modelSkinController ??= modelLocator.modelTransform.gameObject.AddComponent<ModelSkinController>();
                modelSkinController.skins = new[] { skin };
                skin.nameToken = "DEFAULT_SKIN";
            }
            return skin;
        }

        /// <summary>
        /// Defines a new <see cref="SkinDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the skin.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this skin.</param>
        /// <returns>The newly-created <see cref="SkinDef"/>.</returns>
        public static SkinDef DefineSkin(this ContentPack contentPack, string identifier) => DefineSkin<SkinDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineSkin(ContentPack, string)"/>
        public static TSkinDef DefineSkin<TSkinDef>(this ContentPack contentPack, string identifier) where TSkinDef : SkinDef
        {
            static void _(On.RoR2.SkinDef.orig_Awake orig, SkinDef self) { }

            On.RoR2.SkinDef.Awake += _;
            TSkinDef skin = ScriptableObject.CreateInstance<TSkinDef>();
            On.RoR2.SkinDef.Awake -= _;
            string prefix = GetPrefix(contentPack);
            skin.name = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            skin.nameToken = FormatToken($"SKIN_{token}_NAME", tokenPrefix);
            return skin;
        }

        /// <summary>
        /// Defines a new <see cref="ItemTierDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ItemTierDef.tier"/> is set to <see cref="ItemTier.AssignedAtRuntime"/>.
        /// The item tier is added to <see cref="ContentPack.itemTierDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this item tier.</param>
        /// <returns>The newly-created <see cref="ItemTierDef"/>.</returns>
        public static ItemTierDef DefineItemTier(this ContentPack contentPack, string identifier) => DefineItemTier<ItemTierDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineItemTier(ContentPack, string)"/>
        public static TItemTierDef DefineItemTier<TItemTierDef>(this ContentPack contentPack, string identifier) where TItemTierDef : ItemTierDef
        {
            TItemTierDef itemTier = ScriptableObject.CreateInstance<TItemTierDef>();
            itemTier.name = FormatAssetIdentifier(identifier, GetPrefix(contentPack));
            itemTier.tier = ItemTier.AssignedAtRuntime;
            itemTier.isDroppable = true;
            itemTier.canScrap = true;
            itemTier.canRestack = true;
            contentPack.itemTierDefs.Add(itemTier);
            return itemTier;
        }

        /// <summary>
        /// Defines a new <see cref="NetworkSoundEventDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// The network sound event is added to <see cref="ContentPack.networkSoundEventDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this network sound event.</param>
        /// <returns>The newly-created <see cref="NetworkSoundEventDef"/>.</returns>
        public static NetworkSoundEventDef DefineNetworkSoundEvent(this ContentPack contentPack, string identifier) => DefineNetworkSoundEvent<NetworkSoundEventDef>(contentPack, identifier);

        /// <inheritdoc cref="DefineNetworkSoundEvent(ContentPack, string)"/>
        public static TNetworkSoundEventDef DefineNetworkSoundEvent<TNetworkSoundEventDef>(this ContentPack contentPack, string identifier) where TNetworkSoundEventDef : NetworkSoundEventDef
        {
            TNetworkSoundEventDef networkSoundEvent = ScriptableObject.CreateInstance<TNetworkSoundEventDef>();
            networkSoundEvent.name = FormatAssetIdentifier(identifier, GetPrefix(contentPack));
            contentPack.networkSoundEventDefs.Add(networkSoundEvent);
            return networkSoundEvent;
        }

        /// <summary>
        /// Defines a new <typeparamref name="TMiscPickupDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the pickup.
        /// The pickup is added to <see cref="ContentPack.miscPickupDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this pickup.</param>
        /// <returns>The newly-created <typeparamref name="TMiscPickupDef"/>.</returns>
        public static TMiscPickupDef DefineMiscPickup<TMiscPickupDef>(this ContentPack contentPack, string identifier) where TMiscPickupDef : MiscPickupDef, new()
        {
            TMiscPickupDef miscPickup = ScriptableObject.CreateInstance<TMiscPickupDef>();
            string prefix = GetPrefix(contentPack);
            miscPickup.name = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            miscPickup.nameToken= FormatToken($"PICKUP_{token}", tokenPrefix);
            miscPickup.interactContextToken = FormatToken($"{token}_PICKUP_CONTEXT", tokenPrefix);
            contentPack.miscPickupDefs.Add(miscPickup);
            return miscPickup;
        }

        /// <summary>
        /// Defines a new <see cref="SceneDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the scene.
        /// The scene is added to the first <see cref="ExpansionDef"/> in <see cref="ContentPack.expansionDefs"/>, if present.
        /// The scene is added to <see cref="ContentPack.sceneDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="sceneIdentifier">The actual name of this scene.</param>
        /// <returns>The newly-created <see cref="SceneDef"/>.</returns>
        public static SceneDef DefineScene(this ContentPack contentPack, string sceneIdentifier) => DefineScene<SceneDef>(contentPack, sceneIdentifier);

        /// <inheritdoc cref="DefineScene(ContentPack, string)"/>
        public static TSceneDef DefineScene<TSceneDef>(this ContentPack contentPack, string sceneIdentifier) where TSceneDef : SceneDef
        {
            TSceneDef scene = ScriptableObject.CreateInstance<TSceneDef>();
            scene.cachedName = sceneIdentifier;
            string token = sceneIdentifier.ToUpperInvariant();
            string tokenPrefix = GetPrefix(contentPack).ToUpperInvariant();
            scene.nameToken = FormatToken($"MAP_{token}_NAME", tokenPrefix);
            scene.subtitleToken= FormatToken($"MAP_{token}_SUBTITLE", tokenPrefix);
            scene.loreToken = FormatToken($"MAP_{token}_LORE", tokenPrefix);
            scene.portalSelectionMessageString = FormatToken($"BAZAAR_SEER_{token}", tokenPrefix);
            scene.sceneType = SceneType.Stage;
            AssignRequiredExpansion(ref scene.requiredExpansion, contentPack);
            contentPack.sceneDefs.Add(scene);
            return scene;
        }

        /// <summary>
        /// Defines a new <see cref="ItemRelationshipType"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// The item relationship type is added to <see cref="ContentPack.itemRelationshipTypes"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this item relationship type.</param>
        /// <returns>The newly-created <see cref="ItemRelationshipType"/>.</returns>
        public static ItemRelationshipType DefineItemRelationshipType(this ContentPack contentPack, string identifier) => DefineItemRelationshipType<ItemRelationshipType>(contentPack, identifier);

        /// <inheritdoc cref="DefineItemRelationshipType(ContentPack, string)"/>
        public static TItemRelationshipType DefineItemRelationshipType<TItemRelationshipType>(this ContentPack contentPack, string identifier) where TItemRelationshipType : ItemRelationshipType
        {
            TItemRelationshipType itemRelationshipType = ScriptableObject.CreateInstance<TItemRelationshipType>();
            itemRelationshipType.name = FormatAssetIdentifier(identifier, GetPrefix(contentPack));
            contentPack.itemRelationshipTypes.Add(itemRelationshipType);
            return itemRelationshipType;
        }

        /// <summary>
        /// Defines a new <see cref="ItemRelationshipProvider"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// The item relationship provider is added to <see cref="ContentPack.itemRelationshipProviders"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this item relationship provider.</param>
        /// <returns>The newly-created <see cref="ItemRelationshipProvider"/>.</returns>
        public static ItemRelationshipProvider DefineItemRelationshipProvider(this ContentPack contentPack, string identifier) => DefineItemRelationshipProvider<ItemRelationshipProvider>(contentPack, identifier);

        /// <inheritdoc cref="DefineItemRelationshipProvider(ContentPack, string)"/>
        public static TItemRelationshipProvider DefineItemRelationshipProvider<TItemRelationshipProvider>(this ContentPack contentPack, string identifier) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            TItemRelationshipProvider itemRelationshipProvider = ScriptableObject.CreateInstance<TItemRelationshipProvider>();
            itemRelationshipProvider.name = FormatAssetIdentifier(identifier, GetPrefix(contentPack));
            contentPack.itemRelationshipProviders.Add(itemRelationshipProvider);
            return itemRelationshipProvider;
        }

        /// <summary>
        /// Defines a new <see cref="DifficultyDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// <para>Language tokens are generated for the difficulty.</para>
        /// <para>The internal implementation of this method uses <see cref="DifficultyAPI"/>.</para>
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this difficulty.</param>
        /// <param name="preferPositiveIndex">Prefer a positive <see cref="DifficultyIndex"/> over negative, placing this difficulty above Eclipse.</param>
        /// <returns>A <see cref="DifficultyWrapper"/> with the newly-created <see cref="DifficultyDef"/> and registered <see cref="DifficultyIndex"/>.</returns>
        public static DifficultyWrapper DefineDifficulty(this ContentPack contentPack, string identifier, bool preferPositiveIndex = false)
        {
            (DifficultyDef difficulty, DifficultyIndex difficultyIndex) = DefineDifficultyImpl<DifficultyDef>(identifier, preferPositiveIndex, contentPack);
            return new DifficultyWrapper(difficulty, difficultyIndex);
        }

        /// <inheritdoc cref="DefineDifficulty(ContentPack, string, bool)"/>
        public static DifficultyWrapper<TDifficultyDef> DefineDifficulty<TDifficultyDef>(this ContentPack contentPack, string identifier, bool preferPositiveIndex = false) where TDifficultyDef : DifficultyDef
        {
            (TDifficultyDef difficulty, DifficultyIndex difficultyIndex) = DefineDifficultyImpl<TDifficultyDef>(identifier, preferPositiveIndex, contentPack);
            return new DifficultyWrapper<TDifficultyDef>(difficulty, difficultyIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (TDifficultyDef, DifficultyIndex) DefineDifficultyImpl<TDifficultyDef>(string identifier, bool preferPositiveIndex, ContentPack contentPack) where TDifficultyDef : DifficultyDef
        {
            TDifficultyDef difficulty = Activator.CreateInstance<TDifficultyDef>();
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = GetPrefix(contentPack).ToUpperInvariant();
            difficulty.nameToken = FormatToken($"DIFFICULTY_{token}_NAME", tokenPrefix);
            difficulty.descriptionToken = FormatToken($"DIFFICULTY_{token}_DESCRIPTION", tokenPrefix);
            difficulty.foundIconSprite = true;
            return (difficulty, DifficultyAPI.AddDifficulty(difficulty, preferPositiveIndex));
        }

        /// <summary>
        /// Defines a new <see cref="EliteDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the elite.
        /// The elite is added to the first <see cref="ExpansionDef"/> in <see cref="ContentPack.expansionDefs"/>, if present.
        /// An elite equipment is created and added to <see cref="ContentPack.equipmentDefs"/>.
        /// A passive elite buff is created and added to <see cref="ContentPack.buffDefs"/>.
        /// The elite is added to <see cref="ContentPack.eliteDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this elite.</param>
        /// <returns>An <see cref="EliteWrapper"/> with the newly-created <see cref="EliteDef"/>, elite <see cref="EquipmentDef"/>, and elite <see cref="BuffDef"/>.</returns>
        public static EliteWrapper DefineElite(this ContentPack contentPack, string identifier)
        {
            (EliteDef elite, EquipmentDef eliteEquipment, BuffDef eliteBuff) = DefineEliteImpl<EliteDef, EquipmentDef, BuffDef>(identifier, contentPack);
            EliteWrapper result = new EliteWrapper(elite, new List<EliteDef>(), eliteEquipment, eliteBuff)
            {
                registerSubEliteCallback = x => contentPack?.eliteDefs.Add(x),
                subElitePrefix = GetPrefix(contentPack),
            };
            return result; 
        }

        /// <inheritdoc cref="DefineElite(ContentPack, string)"/>
        public static EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef> DefineElite<TEliteDef, TEquipmentDef, TBuffDef>(this ContentPack contentPack, string identifier)
            where TEliteDef : EliteDef
            where TEquipmentDef : EquipmentDef
            where TBuffDef : BuffDef
        {
            (TEliteDef elite, TEquipmentDef eliteEquipment, TBuffDef eliteBuff) = DefineEliteImpl<TEliteDef, TEquipmentDef, TBuffDef>(identifier, contentPack);
            EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef> result = new EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef>(elite, new List<TEliteDef>(), eliteEquipment, eliteBuff)
            {
                registerSubEliteCallback = x => contentPack?.eliteDefs.Add(x),
                subElitePrefix = GetPrefix(contentPack)
            };
            return result;
        }

        private static (TEliteDef, TEquipmentDef, TBuffDef) DefineEliteImpl<TEliteDef, TEquipmentDef, TBuffDef>(string identifier, ContentPack contentPack)
            where TEliteDef : EliteDef
            where TEquipmentDef : EquipmentDef
            where TBuffDef : BuffDef
        {
            TBuffDef eliteBuff = DefineBuff<TBuffDef>(contentPack, $"Elite{identifier}");
            TEquipmentDef eliteEquipment = DefineEquipment<TEquipmentDef>(contentPack, $"Affix{identifier}")
                .SetAvailability(EquipmentAvailability.Never)
                .SetFlags(EquipmentFlags.NeverRandomlyTriggered | EquipmentFlags.EnigmaIncompatible)
                .SetPassiveBuff(eliteBuff);
            eliteEquipment.dropOnDeathChance = 0.00025f;
            TEliteDef elite = ScriptableObject.CreateInstance<TEliteDef>();
            string prefix = GetPrefix(contentPack);
            elite.name = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            elite.modifierToken = FormatToken($"ELITE_MODIFIER_{token}", tokenPrefix);
            elite.eliteEquipmentDef = eliteEquipment;
            elite.shaderEliteRampIndex = 0;
            eliteBuff.eliteDef = elite;
            contentPack.eliteDefs.Add(elite);
            return (elite, eliteEquipment, eliteBuff);
        }

        /// <summary>
        /// Defines a new <see cref="UnlockableDef"/> in this <see cref="ContentPack"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the unlockable.
        /// The unlockable is added to <see cref="ContentPack.unlockableDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="unlockableType">The <see cref="UnlockableType"/> of this unlockable.</param>
        /// <param name="identifier">An internal identifier for this unlockable.</param>
        /// <returns>The newly-created <see cref="UnlockableDef"/>.</returns>
        public static UnlockableDef DefineUnlockable(this ContentPack contentPack, UnlockableType unlockableType, string identifier) => DefineUnlockable<UnlockableDef>(contentPack, unlockableType, identifier);

        /// <inheritdoc cref="DefineUnlockable(ContentPack, UnlockableType, string)"/>
        public static TUnlockableDef DefineUnlockable<TUnlockableDef>(this ContentPack contentPack, UnlockableType unlockableType, string identifier) where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = ScriptableObject.CreateInstance<TUnlockableDef>();
            unlockable.cachedName = GetUnlockableIdentifier(identifier, unlockableType);
            string token = identifier.ToUpperInvariant().Replace('.', '_');
            string tokenPrefix = GetPrefix(contentPack).ToUpperInvariant();
            unlockable.nameToken = FormatToken($"UNLOCKABLE_{((string)unlockableType).ToUpperInvariant()}_{token}", tokenPrefix);
            contentPack.unlockableDefs.Add(unlockable);
            return unlockable;
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="item"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the achievement.
        /// An unlockable is created and added to <see cref="ContentPack.unlockableDefs"/>.
        /// The achievement is set to be registered during <see cref="RoR2BepInExPack.VanillaFixes.SaferAchievementManager.OnCollectAchievementDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this achievement.</param>
        /// <param name="item">The <see cref="ItemDef"/> to be unlocked by this achievement.</param>
        /// <returns>An <see cref="AchievementWrapper"/> with the newly-created <see cref="AchievementDef"/> and <see cref="UnlockableDef"/>.</returns>
        public static AchievementWrapper DefineAchievementForItem(this ContentPack contentPack, string identifier, ItemDef item)
        {
            UnlockableDef unlockable = DefineUnlockable(contentPack, UnlockableType.Items, item.name).SetNameToken(item.nameToken);
            item.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForItem<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, ItemDef item)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(contentPack, UnlockableType.Items, item.name).SetNameToken(item.nameToken);
            item.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="equipment"/>.
        /// </summary>
        /// <param name="equipment">The <see cref="EquipmentDef"/> to be unlocked by this achievement.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForEquipment(this ContentPack contentPack, string identifier, EquipmentDef equipment)
        {
            UnlockableDef unlockable = DefineUnlockable(contentPack, UnlockableType.Items, equipment.name).SetNameToken(equipment.nameToken);
            equipment.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForEquipment(ContentPack, string, EquipmentDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForEquipment<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, EquipmentDef equipment)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(contentPack, UnlockableType.Items, equipment.name).SetNameToken(equipment.nameToken);
            equipment.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="survivor"/>.
        /// </summary>
        /// <param name="survivor">The <see cref="SurvivorDef"/> to be unlocked by this achievement.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForSurvivor(this ContentPack contentPack, string identifier, SurvivorDef survivor)
        {
            UnlockableDef unlockable = DefineUnlockable(contentPack, UnlockableType.Characters, survivor.cachedName).SetNameToken(survivor.displayNameToken);
            survivor.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForSurvivor(ContentPack, string, SurvivorDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSurvivor<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, SurvivorDef survivor)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(contentPack, UnlockableType.Characters, survivor.cachedName).SetNameToken(survivor.displayNameToken); ;
            survivor.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="artifact"/>.
        /// </summary>
        /// <param name="artifact">The <see cref="ArtifactDef"/> to be unlocked by this achievement.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForArtifact(this ContentPack contentPack, string identifier, ArtifactDef artifact)
        {
            UnlockableDef unlockable = DefineUnlockable(contentPack, UnlockableType.Artifacts, artifact.cachedName).SetNameToken(artifact.nameToken);
            artifact.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForArtifact(ContentPack, string, ArtifactDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForArtifact<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, ArtifactDef artifact)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(contentPack, UnlockableType.Artifacts, artifact.cachedName).SetNameToken(artifact.nameToken);
            artifact.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="skillVariant"/>.
        /// </summary>
        /// <param name="skillVariant">The <see cref="SkillFamily.Variant"/> to be unlocked by this achievement.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForSkill(this ContentPack contentPack, string identifier, ref SkillFamily.Variant skillVariant)
        {
            if (skillVariant.skillDef == null)
            {
                throw new ArgumentException(nameof(skillVariant));
            }
            UnlockableDef unlockable = DefineUnlockableForSkillImpl<UnlockableDef>(skillVariant.skillDef, contentPack);
            skillVariant.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForSkill(ContentPack, string, ref SkillFamily.Variant)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSkill<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, ref SkillFamily.Variant skillVariant)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            if (skillVariant.skillDef == null)
            {
                throw new ArgumentException(nameof(skillVariant));
            }
            TUnlockableDef unlockable = DefineUnlockableForSkillImpl<TUnlockableDef>(skillVariant.skillDef, contentPack);
            skillVariant.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for two skill variants with the same <see cref="SkillFamily.Variant.skillDef"/>.
        /// </summary>
        /// <param name="skillVariant1">The first <see cref="SkillFamily.Variant"/> to be unlocked by this achievement.</param>
        /// <param name="skillVariant2">The second <see cref="SkillFamily.Variant"/> to be unlocked by this achievement.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForSkill(this ContentPack contentPack, string identifier, ref SkillFamily.Variant skillVariant1, ref SkillFamily.Variant skillVariant2)
        {
            if (skillVariant1.skillDef == null)
            {
                throw new ArgumentException(nameof(skillVariant1));
            }
            UnlockableDef unlockable = DefineUnlockableForSkillImpl<UnlockableDef>(skillVariant1.skillDef, contentPack);
            skillVariant1.unlockableDef = unlockable;
            skillVariant2.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForSkill(ContentPack, string, ref SkillFamily.Variant, ref SkillFamily.Variant)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSkill<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, ref SkillFamily.Variant skillVariant1, ref SkillFamily.Variant skillVariant2)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            if (skillVariant1.skillDef == null)
            {
                throw new ArgumentException(nameof(skillVariant1));
            }
            TUnlockableDef unlockable = DefineUnlockableForSkillImpl<TUnlockableDef>(skillVariant1.skillDef, contentPack);
            skillVariant1.unlockableDef = unlockable;
            skillVariant2.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="skill"/>.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="skill"/> must be manually associated with the created <see cref="UnlockableDef"/> as part of a <see cref="SkillFamily.Variant"/>.</para>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// </remarks>
        /// <param name="skill">The <see cref="SkillDef"/> to generate this achievement for.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForSkill(this ContentPack contentPack, string identifier, SkillDef skill)
        {
            UnlockableDef unlockable = DefineUnlockableForSkillImpl<UnlockableDef>(skill, contentPack);
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForSkill(ContentPack, string, SkillDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSkill<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, SkillDef skill)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockableForSkillImpl<TUnlockableDef>(skill, contentPack);
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TUnlockableDef DefineUnlockableForSkillImpl<TUnlockableDef>(SkillDef skill, ContentPack contentPack) where TUnlockableDef : UnlockableDef
        {
            return DefineUnlockable<TUnlockableDef>(contentPack, UnlockableType.Skills, skill.skillName)
                .SetNameToken(skill.skillNameToken);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="skin"/>.
        /// </summary>
        /// <param name="skin">The <see cref="SkinDef"/> to be unlocked by this achievement.</param>
        /// <inheritdoc cref="DefineAchievementForItem(ContentPack, string, ItemDef)"/>
        /// <param name="contentPack"></param>
        /// <param name="identifier"></param>
        public static AchievementWrapper DefineAchievementForSkin(this ContentPack contentPack, string identifier, SkinDef skin)
        {
            UnlockableDef unlockable = DefineUnlockable(contentPack, UnlockableType.Skins, skin.name).SetNameToken(skin.nameToken);
            skin.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForSkin(ContentPack, string, SkinDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSkin<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, SkinDef skin)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(contentPack, UnlockableType.Skins, skin.name).SetNameToken(skin.nameToken);
            skin.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <summary>
        /// Defines a new <see cref="AchievementDef"/> in this <see cref="ContentPack"/> for the specified <paramref name="unlockable"/>.
        /// </summary>
        /// <remarks>
        /// Language tokens are generated for the achievement.
        /// The achievement is set to be registered during <see cref="RoR2BepInExPack.VanillaFixes.SaferAchievementManager.OnCollectAchievementDefs"/>.
        /// </remarks>
        /// <param name="contentPack"></param>
        /// <param name="identifier">An internal identifier for this achievement.</param>
        /// <param name="unlockable">The <see cref="UnlockableDef"/> to be unlocked by this achievement.</param>
        /// <returns>An <see cref="AchievementWrapper"/> with the newly-created <see cref="AchievementDef"/> and existing <see cref="UnlockableDef"/>.</returns>
        public static AchievementWrapper DefineAchievementForUnlockable(this ContentPack contentPack, string identifier, UnlockableDef unlockable)
        {
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        /// <inheritdoc cref="DefineAchievementForUnlockable(ContentPack, string, UnlockableDef)"/>
        public static AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForUnlockable<TAchievementDef, TUnlockableDef>(this ContentPack contentPack, string identifier, TUnlockableDef unlockable)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable, contentPack), unlockable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TAchievementDef DefineAchievementImpl<TAchievementDef>(string identifier, UnlockableDef unlockable, ContentPack contentPack)
            where TAchievementDef : AchievementDef
        {
            TAchievementDef achievement = Activator.CreateInstance<TAchievementDef>();
            string prefix = GetPrefix(contentPack);
            achievement.identifier = FormatAssetIdentifier(identifier, prefix);
            string token = identifier.ToUpperInvariant();
            string tokenPrefix = prefix.ToUpperInvariant();
            achievement.nameToken = FormatToken($"ACHIEVEMENT_{token}_NAME", tokenPrefix);
            achievement.descriptionToken = FormatToken($"ACHIEVEMENT_{token}_DESCRIPTION", tokenPrefix);
            achievement.unlockableRewardIdentifier = unlockable.cachedName;
            unlockable.getHowToUnlockString = () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT",
                Language.GetString(achievement.nameToken),
                Language.GetString(achievement.descriptionToken)
            );
            unlockable.getUnlockedString = () => Language.GetStringFormatted("UNLOCKED_FORMAT",
                Language.GetString(achievement.nameToken),
                Language.GetString(achievement.descriptionToken)
            );

            void OnCollectAchievementDefs(List<string> identifiers, Dictionary<string, AchievementDef> identifierToAchievementDef, List<AchievementDef> achievementDefs) 
            {
                if (identifierToAchievementDef.ContainsKey(achievement.identifier))
                {
                    Debug.LogError($"Class {achievement.type.FullName} attempted to register as achievement {achievement.identifier}, but class {identifierToAchievementDef[achievement.identifier].type.FullName} has already registered as that achievement.");
                }
                else
                {
                    identifiers.Add(achievement.identifier);
                    identifierToAchievementDef.Add(achievement.identifier, achievement);
                    achievementDefs.Add(achievement);
                }
                RoR2BepInExPack.VanillaFixes.SaferAchievementManager.OnCollectAchievementDefs -= OnCollectAchievementDefs;
            }

            RoR2BepInExPack.VanillaFixes.SaferAchievementManager.OnCollectAchievementDefs += OnCollectAchievementDefs;
            return achievement;
        }
    }
}