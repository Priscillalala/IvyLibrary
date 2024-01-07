using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.ContentManagement;
using System.Collections;
using RoR2.Skills;
using RoR2.ExpansionManagement;
using RoR2.EntitlementManagement;
using System.Collections.Generic;
using BepInEx.Logging;
using R2API;
using System.Reflection;
using System.Linq;
using EntityStates;
using R2API.ScriptableObjects;
using HG;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.Networking;

namespace Ivyl
{
    public class ContentPackage : IContentPackProvider
    {
        public NamedAssetCollection<GameObject> BodyPrefabs => _contentPack.bodyPrefabs;
        public NamedAssetCollection<GameObject> MasterPrefabs => _contentPack.masterPrefabs;
        public NamedAssetCollection<GameObject> ProjectilePrefabs => _contentPack.projectilePrefabs;
        public NamedAssetCollection<GameObject> GameModePrefabs => _contentPack.gameModePrefabs;
        public NamedAssetCollection<GameObject> NetworkedObjectPrefabs => _contentPack.networkedObjectPrefabs;
        public NamedAssetCollection<SkillDef> SkillDefs => _contentPack.skillDefs;
        public NamedAssetCollection<SkillFamily> SkillFamilies => _contentPack.skillFamilies;
        public NamedAssetCollection<SceneDef> SceneDefs => _contentPack.sceneDefs;
        public NamedAssetCollection<ItemDef> ItemDefs => _contentPack.itemDefs;
        public NamedAssetCollection<ItemTierDef> ItemTierDefs => _contentPack.itemTierDefs;
        public NamedAssetCollection<ItemRelationshipProvider> ItemRelationshipProviders => _contentPack.itemRelationshipProviders;
        public NamedAssetCollection<ItemRelationshipType> ItemRelationshipTypes => _contentPack.itemRelationshipTypes;
        public NamedAssetCollection<EquipmentDef> EquipmentDefs => _contentPack.equipmentDefs;
        public NamedAssetCollection<BuffDef> BuffDefs => _contentPack.buffDefs;
        public NamedAssetCollection<EliteDef> EliteDefs => _contentPack.eliteDefs;
        public NamedAssetCollection<UnlockableDef> UnlockableDefs => _contentPack.unlockableDefs;
        public NamedAssetCollection<SurvivorDef> SurvivorDefs => _contentPack.survivorDefs;
        public NamedAssetCollection<ArtifactDef> ArtifactDefs => _contentPack.artifactDefs;
        public NamedAssetCollection<EffectDef> EffectDefs => _contentPack.effectDefs;
        public NamedAssetCollection<SurfaceDef> SurfaceDefs => _contentPack.surfaceDefs;
        public NamedAssetCollection<NetworkSoundEventDef> NetworkSoundEventDefs => _contentPack.networkSoundEventDefs;
        public NamedAssetCollection<MusicTrackDef> MusicTrackDefs => _contentPack.musicTrackDefs;
        public NamedAssetCollection<GameEndingDef> GameEndingDefs => _contentPack.gameEndingDefs;
        public NamedAssetCollection<EntityStateConfiguration> EntityStateConfigurations => _contentPack.entityStateConfigurations;
        public NamedAssetCollection<ExpansionDef> ExpansionDefs => _contentPack.expansionDefs;
        public NamedAssetCollection<EntitlementDef> EntitlementDefs => _contentPack.entitlementDefs;
        public NamedAssetCollection<MiscPickupDef> MiscPickupDefs => _contentPack.miscPickupDefs;
        public NamedAssetCollection<Type> EntityStateTypes => _contentPack.entityStateTypes;

        public event LoadStaticContentAsyncDelegate onLoadStaticContent;
        public event GenerateContentPackAsyncDelegate onGenerateContentPack;
        public event FinalizeAsyncDelegate onFinalize;
        public delegate IEnumerator LoadStaticContentAsyncDelegate(LoadStaticContentAsyncArgs args);
        public delegate IEnumerator GenerateContentPackAsyncDelegate(GetContentPackAsyncArgs args);
        public delegate IEnumerator FinalizeAsyncDelegate(FinalizeAsyncArgs args);

        private readonly string _identifier;
        private readonly string _prefix;
        private readonly string _prefixUpperInvariant;
        private readonly ContentPack _contentPack;

        public ContentPackage(string contentIdentifier, string contentPrefix = null)
        {
            _identifier = contentIdentifier;
            _prefix = contentPrefix;
            _prefixUpperInvariant = _prefix.ToUpperInvariant();
            _contentPack = new ContentPack 
            {
                identifier = contentIdentifier
            };
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        public string identifier => _identifier;

        public virtual IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            if (onLoadStaticContent != null)
            {
                foreach (LoadStaticContentAsyncDelegate func in onLoadStaticContent.GetInvocationList())
                {
                    yield return func(args);
                }
                onLoadStaticContent = null;
            }
            args.ReportProgress(1f);
            yield break;
        }

        public virtual IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            if (onGenerateContentPack != null)
            {
                foreach (GenerateContentPackAsyncDelegate func in onGenerateContentPack.GetInvocationList())
                {
                    yield return func(args);
                }
                onGenerateContentPack = null;
            }

            ContentPack.Copy(_contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public virtual IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            MD5 hashAlgorithm = MD5.Create();
            PopulateAssetIds(BodyPrefabs, nameof(BodyPrefabs), stringBuilder, hashAlgorithm);
            PopulateAssetIds(MasterPrefabs, nameof(MasterPrefabs), stringBuilder, hashAlgorithm);
            PopulateAssetIds(ProjectilePrefabs, nameof(ProjectilePrefabs), stringBuilder, hashAlgorithm);
            PopulateAssetIds(NetworkedObjectPrefabs, nameof(NetworkedObjectPrefabs), stringBuilder, hashAlgorithm);
            PopulateAssetIds(GameModePrefabs, nameof(GameModePrefabs), stringBuilder, hashAlgorithm);
            hashAlgorithm.Dispose();

            if (onFinalize != null)
            {
                foreach (FinalizeAsyncDelegate func in onFinalize.GetInvocationList())
                {
                    yield return func(args);
                }
                onFinalize = null;
            }
            args.ReportProgress(1f);
            yield break;
        }

        protected virtual void PopulateAssetIds(NamedAssetCollection<GameObject> assets, string collectionIdentifier, StringBuilder stringBuilder, HashAlgorithm hashAlgorithm)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                NamedAssetCollection<GameObject>.AssetInfo assetInfo = assets.assetInfos[i];
                if (assetInfo.asset.TryGetComponent(out NetworkIdentity networkIdentity) && !networkIdentity.assetId.IsValid())
                {
                    stringBuilder.Clear();
                    foreach (byte b in hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(assetInfo.assetName + _identifier + collectionIdentifier)))
                    {
                        stringBuilder.Append(b.ToString("x2"));
                    }
                    networkIdentity.SetDynamicAssetId(NetworkHash128.Parse(stringBuilder.ToString()));
                }
            }
        }

        public void AddEntityStatesFromAssembly(Assembly assembly)
        {
            EntityStateTypes.Add(assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EntityState))).ToArray());
        }

        public void AddFromSerializableContentPack(R2APISerializableContentPack serializableContentPack)
        {
            serializableContentPack.AddTo(_contentPack);
        }

        public void AddEffectPrefab(GameObject effectPrefab)
        {
            EffectDefs.Add(new EffectDef(effectPrefab));
        }

        protected virtual string GetToken(string baseToken)
        {
            if (string.IsNullOrWhiteSpace(_prefix))
            {
                return baseToken;
            }
            return _prefixUpperInvariant + '_' + baseToken;
        }

        protected virtual string GetIdentifier(string baseIdentifier)
        {
            if (string.IsNullOrWhiteSpace(_prefix))
            {
                return baseIdentifier;
            }
            return _prefix + '_' + baseIdentifier;
        }

        protected virtual string RevertIdentifier(string identifier)
        {
            string prefix = _prefix + '_';
            if (identifier.StartsWith(prefix))
            {
                return identifier.Substring(prefix.Length);
            }
            return identifier;
        }

        protected virtual string GetUnlockableIdentifier(string baseIdentifier, UnlockableType unlockableType)
        {
            baseIdentifier = RevertIdentifier(baseIdentifier);
            if (!string.IsNullOrWhiteSpace(_prefix))
            {
                baseIdentifier = _prefix + '.' + baseIdentifier;
            }
            if (!string.IsNullOrWhiteSpace((string)unlockableType))
            {
                baseIdentifier = (string)unlockableType + '.' + baseIdentifier;
            }
            return baseIdentifier;
        }

        protected virtual void AssignRequiredExpansion(ref ExpansionDef requiredExpansion) { }

        public ItemDef DefineItem(string identifier) => DefineItem<ItemDef>(identifier);

        public virtual TItemDef DefineItem<TItemDef>(string identifier) where TItemDef : ItemDef
        {
            TItemDef item = ScriptableObject.CreateInstance<TItemDef>();
            item.name = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            item.nameToken = GetToken($"ITEM_{token}_NAME");
            item.pickupToken = GetToken($"ITEM_{token}_PICKUP");
            item.descriptionToken = GetToken($"ITEM_{token}_DESC");
            item.loreToken = GetToken($"ITEM_{token}_LORE");
            AssignRequiredExpansion(ref item.requiredExpansion);
            ItemDefs.Add(item);
            return item;
        }

        public EquipmentDef DefineEquipment(string identifier) => DefineEquipment<EquipmentDef>(identifier);

        public virtual TEquipmentDef DefineEquipment<TEquipmentDef>(string identifier) where TEquipmentDef : EquipmentDef
        {
            TEquipmentDef equipment = ScriptableObject.CreateInstance<TEquipmentDef>();
            equipment.name = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            equipment.nameToken = GetToken($"EQUIPMENT_{token}_NAME");
            equipment.pickupToken = GetToken($"EQUIPMENT_{token}_PICKUP");
            equipment.descriptionToken = GetToken($"EQUIPMENT_{token}_DESC");
            equipment.loreToken = GetToken($"EQUIPMENT_{token}_LORE");
            equipment.canDrop = true;
            equipment.enigmaCompatible = true;
            AssignRequiredExpansion(ref equipment.requiredExpansion);
            EquipmentDefs.Add(equipment);
            return equipment;
        }

        public BuffDef DefineBuff(string identifier) => DefineBuff<BuffDef>(identifier);

        public virtual TBuffDef DefineBuff<TBuffDef>(string identifier) where TBuffDef : BuffDef
        {
            TBuffDef buff = ScriptableObject.CreateInstance<TBuffDef>();
            buff.name = GetIdentifier(identifier);
            BuffDefs.Add(buff);
            return buff;
        }

        public ArtifactDef DefineArtifact(string identifier) => DefineArtifact<ArtifactDef>(identifier);

        public virtual TArtifactDef DefineArtifact<TArtifactDef>(string identifier) where TArtifactDef : ArtifactDef
        {
            TArtifactDef artifact = ScriptableObject.CreateInstance<TArtifactDef>();
            artifact.cachedName = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            artifact.nameToken = GetToken($"ARTIFACT_{token}_NAME");
            artifact.descriptionToken = GetToken($"ARTIFACT_{token}_DESCRIPTION");
            AssignRequiredExpansion(ref artifact.requiredExpansion);
            ArtifactDefs.Add(artifact);
            return artifact;
        }

        public SkillDef DefineSkill(string identifier) => DefineSkill<SkillDef>(identifier);

        public virtual TSceneDef DefineSkill<TSceneDef>(string identifier) where TSceneDef : SkillDef
        {
            TSceneDef skill = ScriptableObject.CreateInstance<TSceneDef>();
            skill.skillName = GetIdentifier(identifier);
            (skill as ScriptableObject).name = skill.skillName;
            string token = identifier.ToUpperInvariant();
            skill.skillNameToken = GetToken($"SKILL_{token}_NAME");
            skill.skillDescriptionToken = GetToken($"SKILL_{token}_DESC");
            SkillDefs.Add(skill);
            return skill;
        }

        public GameEndingDef DefineGameEnding(string identifier) => DefineGameEnding<GameEndingDef>(identifier);

        public virtual TGameEndingDef DefineGameEnding<TGameEndingDef>(string identifier) where TGameEndingDef : GameEndingDef
        {
            TGameEndingDef gameEnding = ScriptableObject.CreateInstance<TGameEndingDef>();
            gameEnding.cachedName = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            gameEnding.endingTextToken = GetToken($"GAME_RESULT_{token}");
            GameEndingDefs.Add(gameEnding);
            return gameEnding;
        }

        public SurfaceDef DefineSurfaceDef(string identifier) => DefineSurfaceDef<SurfaceDef>(identifier);

        public virtual TSurfaceDef DefineSurfaceDef<TSurfaceDef>(string identifier) where TSurfaceDef : SurfaceDef
        {
            TSurfaceDef surface = ScriptableObject.CreateInstance<TSurfaceDef>();
            surface.name = GetIdentifier(identifier);
            SurfaceDefs.Add(surface);
            return surface;
        }

        public SurvivorDef DefineSurvivorFromBodyPrefab(string identifier, GameObject bodyPrefab) => DefineSurvivorFromBodyPrefab<SurvivorDef>(identifier, bodyPrefab);

        public virtual TSurvivorDef DefineSurvivorFromBodyPrefab<TSurvivorDef>(string identifier, GameObject bodyPrefab) where TSurvivorDef : SurvivorDef
        {
            TSurvivorDef survivor = DefineSurvivorImpl<TSurvivorDef>(identifier);
            survivor.bodyPrefab = bodyPrefab;
            if (survivor.bodyPrefab.TryGetComponent(out CharacterBody characterBody))
            {
                survivor.displayNameToken = characterBody.baseNameToken;
                survivor.primaryColor = characterBody.bodyColor;
            }
            if (survivor.bodyPrefab.TryGetComponent(out ExpansionRequirementComponent expansionRequirementComponent))
            {
                AssignRequiredExpansion(ref expansionRequirementComponent.requiredExpansion);
            }
            return survivor;
        }

        protected virtual TSurvivorDef DefineSurvivorImpl<TSurvivorDef>(string identifier) where TSurvivorDef : SurvivorDef
        {
            TSurvivorDef survivor = ScriptableObject.CreateInstance<TSurvivorDef>();
            survivor.cachedName = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            survivor.displayNameToken = GetToken($"{token}_BODY_NAME");
            survivor.descriptionToken = GetToken($"{token}_DESCRIPTION");
            survivor.outroFlavorToken = GetToken($"{token}_OUTRO_FLAVOR");
            survivor.outroFlavorToken = GetToken($"{token}_MAIN_ENDING_ESCAPE_FAILURE_FLAVOR");
            SurvivorDefs.Add(survivor);
            return survivor;
        }

        public SkinDef DefineSkinForBodyPrefab(string identifier, GameObject bodyPrefab) => DefineSkinForBodyPrefab<SkinDef>(identifier, bodyPrefab);

        public virtual TSkinDef DefineSkinForBodyPrefab<TSkinDef>(string identifier, GameObject bodyPrefab) where TSkinDef : SkinDef
        {
            if (!bodyPrefab.TryGetComponent(out ModelLocator modelLocator) || !modelLocator.modelTransform)
            {
                throw new ArgumentException(nameof(bodyPrefab));
            }
            TSkinDef skin = DefineSkin<TSkinDef>(identifier);
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

        public SkinDef DefineSkin(string identifier) => DefineSkin<SkinDef>(identifier);

        public virtual TSkinDef DefineSkin<TSkinDef>(string identifier) where TSkinDef : SkinDef
        {
            static void _(On.RoR2.SkinDef.orig_Awake orig, SkinDef self) { }

            On.RoR2.SkinDef.Awake += _;
            TSkinDef skin = ScriptableObject.CreateInstance<TSkinDef>();
            On.RoR2.SkinDef.Awake -= _;
            skin.name = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            skin.nameToken = GetToken($"SKIN_{token}_NAME");
            return skin;
        }

        public ItemTierDef DefineItemTier(string identifier) => DefineItemTier<ItemTierDef>(identifier);

        public virtual TItemTierDef DefineItemTier<TItemTierDef>(string identifier) where TItemTierDef : ItemTierDef
        {
            TItemTierDef itemTier = ScriptableObject.CreateInstance<TItemTierDef>();
            itemTier.name = GetIdentifier(identifier);
            itemTier.tier = ItemTier.AssignedAtRuntime;
            itemTier.isDroppable = true;
            itemTier.canScrap = true;
            itemTier.canRestack = true;
            ItemTierDefs.Add(itemTier);
            return itemTier;
        }

        public NetworkSoundEventDef DefineNetworkSoundEvent(string identifier) => DefineNetworkSoundEvent<NetworkSoundEventDef>(identifier);

        public virtual TNetworkSoundEventDef DefineNetworkSoundEvent<TNetworkSoundEventDef>(string identifier) where TNetworkSoundEventDef : NetworkSoundEventDef
        {
            TNetworkSoundEventDef networkSoundEvent = ScriptableObject.CreateInstance<TNetworkSoundEventDef>();
            networkSoundEvent.name = GetIdentifier(identifier);
            NetworkSoundEventDefs.Add(networkSoundEvent);
            return networkSoundEvent;
        }

        public virtual TMiscPickupDef DefineMiscPickup<TMiscPickupDef>(string identifier) where TMiscPickupDef : MiscPickupDef, new()
        {
            TMiscPickupDef miscPickup = ScriptableObject.CreateInstance<TMiscPickupDef>();
            miscPickup.name = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            miscPickup.nameToken= GetToken($"PICKUP_{token}");
            miscPickup.interactContextToken = GetToken($"{token}_PICKUP_CONTEXT");
            MiscPickupDefs.Add(miscPickup);
            return miscPickup;
        }

        public SceneDef DefineScene(string sceneIdentifier) => DefineScene<SceneDef>(sceneIdentifier);

        public virtual TSceneDef DefineScene<TSceneDef>(string sceneIdentifier) where TSceneDef : SceneDef
        {
            TSceneDef scene = ScriptableObject.CreateInstance<TSceneDef>();
            scene.cachedName = sceneIdentifier;
            string token = sceneIdentifier.ToUpperInvariant();
            scene.nameToken = GetToken($"MAP_{token}_NAME");
            scene.subtitleToken= GetToken($"MAP_{token}_SUBTITLE");
            scene.loreToken = GetToken($"MAP_{token}_LORE");
            scene.portalSelectionMessageString = GetToken($"BAZAAR_SEER_{token}");
            scene.sceneType = SceneType.Stage;
            AssignRequiredExpansion(ref scene.requiredExpansion);
            SceneDefs.Add(scene);
            return scene;
        }

        public ItemRelationshipType DefineItemRelationshipType(string identifier) => DefineItemRelationshipType<ItemRelationshipType>(identifier);

        public virtual TItemRelationshipType DefineItemRelationshipType<TItemRelationshipType>(string identifier) where TItemRelationshipType : ItemRelationshipType
        {
            TItemRelationshipType itemRelationshipType = ScriptableObject.CreateInstance<TItemRelationshipType>();
            itemRelationshipType.name = GetIdentifier(identifier);
            ItemRelationshipTypes.Add(itemRelationshipType);
            return itemRelationshipType;
        }

        public ItemRelationshipProvider DefineItemRelationshipProvider(string identifier) => DefineItemRelationshipProvider<ItemRelationshipProvider>(identifier);

        public virtual TItemRelationshipProvider DefineItemRelationshipProvider<TItemRelationshipProvider>(string identifier) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            TItemRelationshipProvider itemRelationshipProvider = ScriptableObject.CreateInstance<TItemRelationshipProvider>();
            itemRelationshipProvider.name = GetIdentifier(identifier);
            ItemRelationshipProviders.Add(itemRelationshipProvider);
            return itemRelationshipProvider;
        }

        public DifficultyWrapper DefineDifficulty(string identifier, bool preferPositiveIndex = false)
        {
            (DifficultyDef difficulty, DifficultyIndex difficultyIndex) = DefineDifficultyImpl<DifficultyDef>(identifier, preferPositiveIndex);
            return new DifficultyWrapper(difficulty, difficultyIndex);
        }

        public DifficultyWrapper<TDifficultyDef> DefineDifficulty<TDifficultyDef>(string identifier, bool preferPositiveIndex = false) where TDifficultyDef : DifficultyDef
        {
            (TDifficultyDef difficulty, DifficultyIndex difficultyIndex) = DefineDifficultyImpl<TDifficultyDef>(identifier, preferPositiveIndex);
            return new DifficultyWrapper<TDifficultyDef>(difficulty, difficultyIndex);
        }

        protected virtual (TDifficultyDef, DifficultyIndex) DefineDifficultyImpl<TDifficultyDef>(string identifier, bool preferPositiveIndex) where TDifficultyDef : DifficultyDef
        {
            TDifficultyDef difficulty = Activator.CreateInstance<TDifficultyDef>();
            string token = identifier.ToUpperInvariant();
            difficulty.nameToken = GetToken($"DIFFICULTY_{token}_NAME");
            difficulty.descriptionToken = GetToken($"DIFFICULTY_{token}_DESCRIPTION");
            difficulty.foundIconSprite = true;
            return (difficulty, DifficultyAPI.AddDifficulty(difficulty, preferPositiveIndex));
        }

        public EliteWrapper DefineElite(string identifier) 
        {
            (EliteDef elite, EquipmentDef eliteEquipment, BuffDef eliteBuff) = DefineEliteImpl<EliteDef, EquipmentDef, BuffDef>(identifier);
            EliteWrapper result = new EliteWrapper(elite, new List<EliteDef>(), eliteEquipment, eliteBuff);
            result.registerSubEliteCallback = x => EliteDefs.Add(x);
            result.subEliteTokenPrefix = _prefix;
            return result; 
        }

        public EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef> DefineElite<TEliteDef, TEquipmentDef, TBuffDef>(string identifier)
            where TEliteDef : EliteDef
            where TEquipmentDef : EquipmentDef
            where TBuffDef : BuffDef
        {
            (TEliteDef elite, TEquipmentDef eliteEquipment, TBuffDef eliteBuff) = DefineEliteImpl<TEliteDef, TEquipmentDef, TBuffDef>(identifier);
            EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef> result = new EliteWrapper<TEliteDef, TEquipmentDef, TBuffDef>(elite, new List<TEliteDef>(), eliteEquipment, eliteBuff);
            result.registerSubEliteCallback = x => EliteDefs.Add(x);
            result.subEliteTokenPrefix = _prefix;
            return result;
        }

        protected virtual (TEliteDef, TEquipmentDef, TBuffDef) DefineEliteImpl<TEliteDef, TEquipmentDef, TBuffDef>(string identifier)
            where TEliteDef : EliteDef
            where TEquipmentDef : EquipmentDef
            where TBuffDef : BuffDef
        {
            TBuffDef eliteBuff = DefineBuff<TBuffDef>($"Elite{identifier}");
            TEquipmentDef eliteEquipment = DefineEquipment<TEquipmentDef>($"Affix{identifier}")
                .SetAvailability(EquipmentAvailability.Never)
                .SetFlags(EquipmentFlags.NeverRandomlyTriggered | EquipmentFlags.EnigmaIncompatible)
                .SetPassiveBuff(eliteBuff);
            eliteEquipment.dropOnDeathChance = 0.00025f;
            TEliteDef elite = ScriptableObject.CreateInstance<TEliteDef>();
            elite.name = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            elite.modifierToken = GetToken($"ELITE_MODIFIER_{token}");
            elite.eliteEquipmentDef = eliteEquipment;
            elite.shaderEliteRampIndex = 0;
            eliteBuff.eliteDef = elite;
            EliteDefs.Add(elite);
            return (elite, eliteEquipment, eliteBuff);
        }

        public UnlockableDef DefineUnlockable(UnlockableType unlockableType, string identifier) => DefineUnlockable<UnlockableDef>(unlockableType, identifier);

        public virtual TUnlockableDef DefineUnlockable<TUnlockableDef>(UnlockableType unlockableType, string identifier) where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = ScriptableObject.CreateInstance<TUnlockableDef>();
            unlockable.cachedName = GetUnlockableIdentifier(identifier, unlockableType);
            string token = identifier.ToUpperInvariant().Replace('.', '_');
            unlockable.nameToken = GetToken($"UNLOCKABLE_{((string)unlockableType).ToUpperInvariant()}_{token}");
            UnlockableDefs.Add(unlockable);
            return unlockable;
        }

        public AchievementWrapper DefineAchievementForItem(string identifier, ItemDef item)
        {
            UnlockableDef unlockable = DefineUnlockable(UnlockableType.Items, item.name).SetNameToken(item.nameToken);
            item.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForItem<TAchievementDef, TUnlockableDef>(string identifier, ItemDef item)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(UnlockableType.Items, item.name).SetNameToken(item.nameToken);
            item.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper DefineAchievementForEquipment(string identifier, EquipmentDef equipment)
        {
            UnlockableDef unlockable = DefineUnlockable(UnlockableType.Items, equipment.name).SetNameToken(equipment.nameToken);
            equipment.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForEquipment<TAchievementDef, TUnlockableDef>(string identifier, EquipmentDef equipment)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(UnlockableType.Items, equipment.name).SetNameToken(equipment.nameToken);
            equipment.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper DefineAchievementForSurvivor(string identifier, SurvivorDef survivor)
        {
            UnlockableDef unlockable = DefineUnlockable(UnlockableType.Characters, survivor.cachedName).SetNameToken(survivor.displayNameToken);
            survivor.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSurvivor<TAchievementDef, TUnlockableDef>(string identifier, SurvivorDef survivor)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(UnlockableType.Characters, survivor.cachedName).SetNameToken(survivor.displayNameToken); ;
            survivor.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper DefineAchievementForArtifact(string identifier, ArtifactDef artifact)
        {
            UnlockableDef unlockable = DefineUnlockable(UnlockableType.Artifacts, artifact.cachedName).SetNameToken(artifact.nameToken);
            artifact.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForArtifact<TAchievementDef, TUnlockableDef>(string identifier, ArtifactDef artifact)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(UnlockableType.Artifacts, artifact.cachedName).SetNameToken(artifact.nameToken);
            artifact.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper DefineAchievementForSkill(string identifier, SkillDef skill, ref SkillFamily.Variant[] unlockableSkillVariants)
        {
            UnlockableDef unlockable = DefineUnlockable(UnlockableType.Skills, skill.skillName).SetNameToken(skill.skillNameToken);
            for (int i = 0; i < unlockableSkillVariants.Length; i++)
            {
                unlockableSkillVariants[i].unlockableDef = unlockable;
            }
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSkill<TAchievementDef, TUnlockableDef>(string identifier, SkillDef skill, ref SkillFamily.Variant[] unlockableSkillVariants)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(UnlockableType.Skills, skill.skillName).SetNameToken(skill.skillNameToken);
            for (int i = 0; i < unlockableSkillVariants.Length; i++)
            {
                unlockableSkillVariants[i].unlockableDef = unlockable;
            }
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper DefineAchievementForSkin(string identifier, SkinDef skin)
        {
            UnlockableDef unlockable = DefineUnlockable(UnlockableType.Skins, skin.name).SetNameToken(skin.nameToken);
            skin.unlockableDef = unlockable;
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForSkin<TAchievementDef, TUnlockableDef>(string identifier, SkinDef skin)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            TUnlockableDef unlockable = DefineUnlockable<TUnlockableDef>(UnlockableType.Skins, skin.name).SetNameToken(skin.nameToken);
            skin.unlockableDef = unlockable;
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper DefineAchievementForUnlockable(string identifier, UnlockableDef unlockable)
        {
            return new AchievementWrapper(DefineAchievementImpl<AchievementDef>(identifier, unlockable), unlockable);
        }

        public AchievementWrapper<TAchievementDef, TUnlockableDef> DefineAchievementForUnlockable<TAchievementDef, TUnlockableDef>(string identifier, TUnlockableDef unlockable)
            where TAchievementDef : AchievementDef
            where TUnlockableDef : UnlockableDef
        {
            return new AchievementWrapper<TAchievementDef, TUnlockableDef>(DefineAchievementImpl<TAchievementDef>(identifier, unlockable), unlockable);
        }

        protected virtual TAchievementDef DefineAchievementImpl<TAchievementDef>(string identifier, UnlockableDef unlockable)
            where TAchievementDef : AchievementDef
        {
            TAchievementDef achievement = Activator.CreateInstance<TAchievementDef>();
            achievement.identifier = GetIdentifier(identifier);
            string token = identifier.ToUpperInvariant();
            achievement.nameToken = GetToken($"ACHIEVEMENT_{token}_NAME");
            achievement.descriptionToken = GetToken($"ACHIEVEMENT_{token}_DESCRIPTION");
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