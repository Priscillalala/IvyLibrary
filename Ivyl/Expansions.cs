using System;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2.ExpansionManagement;

namespace Ivyl
{
    public static class Expansions
    {
        public static ExpansionDef DLC1 => Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();
    }
}