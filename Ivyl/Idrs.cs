using System;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Linq;
using HG;

namespace Ivyl
{
    public static class Idrs
    {
        public static ItemDisplayRuleSet Commando => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Commando/idrsCommando.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Huntress => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Huntress/idrsHuntress.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Bandit => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Bandit2/idrsBandit2.asset").WaitForCompletion();
        public static ItemDisplayRuleSet MULT => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Toolbot/idrsToolbot.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Engineer => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Engi/idrsEngi.asset").WaitForCompletion();
        public static ItemDisplayRuleSet EngineerTurret => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Engi/idrsEngiTurret.asset").WaitForCompletion();
        public static ItemDisplayRuleSet EngineerWalkerTurret => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Engi/idrsEngiWalkerTurret.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Artificer => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Mage/idrsMage.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Mercenary => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Merc/idrsMerc.asset").WaitForCompletion();
        public static ItemDisplayRuleSet REX => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Treebot/idrsTreebot.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Loader => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Loader/idrsLoader.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Acrid => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Croco/idrsCroco.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Captain => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Captain/idrsCaptain.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Railgunner => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/Railgunner/idrsRailGunner.asset").WaitForCompletion();
        public static ItemDisplayRuleSet VoidFiend => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/DLC1/VoidSurvivor/idrsVoidSurvivor.asset").WaitForCompletion();
        public static ItemDisplayRuleSet Scavenger => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Scav/idrsScav.asset").WaitForCompletion();
        public static ItemDisplayRuleSet EquipmentDrone => Addressables.LoadAssetAsync<ItemDisplayRuleSet>("RoR2/Base/Drones/idrsEquipmentDrone.asset").WaitForCompletion();

        private struct InheritedItemDisplay
        {
            public ItemDisplaySpec itemDisplay;
            public UnityEngine.Object parentKeyAsset;
            public bool alwaysApply;
        }

        private static List<InheritedItemDisplay> inheritedItemDisplays;
        private static bool _init;

        [SystemInitializer]
        private static void Init()
        {
            if (inheritedItemDisplays != null && inheritedItemDisplays.Count > 0)
            {
                Lookup<UnityEngine.Object, InheritedItemDisplay> inheritedItemDisplayLookup = (Lookup<UnityEngine.Object, InheritedItemDisplay>)inheritedItemDisplays.ToLookup(x => x.parentKeyAsset);
                Dictionary<UnityEngine.Object, DisplayRuleGroup> inheritedRuleGroups = new Dictionary<UnityEngine.Object, DisplayRuleGroup>();
                foreach (ItemDisplayRuleSet idrs in ItemDisplayRuleSet.instancesList)
                {
                    if (idrs.keyAssetRuleGroups == null)
                    {
                        continue;
                    }
                    ApplyInheritedItemDisplays(idrs, inheritedRuleGroups);
                    if (inheritedRuleGroups.Count > 0)
                    {
                        ItemDisplayRuleSet.KeyAssetRuleGroup[] inheritedKeyAssetRuleGroups = inheritedRuleGroups.Select(x => new ItemDisplayRuleSet.KeyAssetRuleGroup
                        {
                            keyAsset = x.Key,
                            displayRuleGroup = x.Value,
                        }).ToArray();
                        idrs.keyAssetRuleGroups = ArrayUtils.Join(idrs.keyAssetRuleGroups, inheritedKeyAssetRuleGroups);
                    }
                    inheritedRuleGroups.Clear();
                }
            }
            _init = true;
        }

        private static void ApplyInheritedItemDisplays(ItemDisplayRuleSet idrs, Dictionary<UnityEngine.Object, DisplayRuleGroup> inheritedRuleGroups)
        {
            Dictionary<UnityEngine.Object, int> keyAssetRuleGroupsDict = new Dictionary<UnityEngine.Object, int>();
            for (int i = 0; i < idrs.keyAssetRuleGroups.Length; i++)
            {
                keyAssetRuleGroupsDict[idrs.keyAssetRuleGroups[i].keyAsset] = i;
            }
            foreach (InheritedItemDisplay inheritedItemDisplay in inheritedItemDisplays)
            {
                if (keyAssetRuleGroupsDict.TryGetValue(inheritedItemDisplay.parentKeyAsset, out int parentIndex))
                {
                    ItemDisplayRule[] inheritedRules = ArrayUtils.Clone(idrs.keyAssetRuleGroups[parentIndex].displayRuleGroup.rules);
                    for (int j = 0; j < inheritedRules.Length; j++)
                    {
                        ref ItemDisplayRule idr = ref inheritedRules[j];
                        idr.followerPrefab = inheritedItemDisplay.itemDisplay.displayModelPrefab;
                        idr.limbMask = inheritedItemDisplay.itemDisplay.limbMask;
                        idr.ruleType = idr.limbMask > LimbFlags.None ? ItemDisplayRuleType.LimbMask : ItemDisplayRuleType.ParentedPrefab;
                    }
                    if (keyAssetRuleGroupsDict.TryGetValue(inheritedItemDisplay.itemDisplay.keyAsset, out int index))
                    {
                        if (inheritedItemDisplay.alwaysApply)
                        {
                            ref ItemDisplayRule[] rules = ref idrs.keyAssetRuleGroups[index].displayRuleGroup.rules;
                            rules = ArrayUtils.Join(rules, inheritedRules);
                        }
                    }
                    else if (inheritedRuleGroups.TryGetValue(inheritedItemDisplay.itemDisplay.keyAsset, out DisplayRuleGroup displayRuleGroup))
                    {
                        displayRuleGroup.rules = ArrayUtils.Join(displayRuleGroup.rules, inheritedRules);
                        inheritedRuleGroups[inheritedItemDisplay.itemDisplay.keyAsset] = displayRuleGroup;
                    }
                    else
                    {
                        inheritedRuleGroups.Add(inheritedItemDisplay.itemDisplay.keyAsset, new DisplayRuleGroup
                        {
                            rules = inheritedRules
                        });
                    }
                }
            }
        }

        public static void RegisterInheritedItemDisplay(ItemDisplaySpec itemDisplay, UnityEngine.Object parentKeyAsset, bool alwaysApply = false)
        {
            if (_init)
            {
                throw new InvalidOperationException();
            }
            if (itemDisplay.keyAsset == parentKeyAsset)
            {
                throw new ArgumentException();
            }
            (inheritedItemDisplays ??= new List<InheritedItemDisplay>()).Add(new InheritedItemDisplay
            {
                itemDisplay = itemDisplay,
                parentKeyAsset = parentKeyAsset,
                alwaysApply = alwaysApply,
            });
        }
    }
}