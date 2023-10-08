using System;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Ivyl
{
    public static class RoR2AssetGroups
    {
        public static RoR2AssetGroup<ItemDisplayRuleSet> ItemDisplayRuleSets => new RoR2AssetGroup<ItemDisplayRuleSet>("ContentPack:RoR2.BaseContent", "ContentPack:RoR2.DLC1");
    }
}