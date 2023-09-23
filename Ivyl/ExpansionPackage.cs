using BepInEx;
using System;
using RoR2;
using System.Security.Permissions;
using System.Security;
using UnityEngine.ResourceManagement;
using UnityEngine;
using RoR2.Skills;
using RoR2.ExpansionManagement;
using UnityEngine.AddressableAssets;
using RoR2.EntitlementManagement;

namespace Ivyl
{
    public class ExpansionPackage : ExpansionPackage<ExpansionPackage, ExpansionDef>
    {
        public ExpansionPackage(string expansionIdentifier, string contentPrefix = null) : base(expansionIdentifier, contentPrefix) { }
    }

    public class ExpansionPackage<TExpansionDef> : ExpansionPackage<ExpansionPackage<TExpansionDef>, TExpansionDef>
        where TExpansionDef : ExpansionDef
    {
        public ExpansionPackage(string expansionIdentifier, string contentPrefix = null) : base(expansionIdentifier, contentPrefix) { }
    }

    public abstract class ExpansionPackage<TIvylExpansionPack, TExpansionDef> : ContentPackage 
        where TIvylExpansionPack : ExpansionPackage<TIvylExpansionPack, TExpansionDef>
        where TExpansionDef : ExpansionDef
    {
        public ExpansionDef ExpansionDef { get; }

        public ExpansionPackage(string expansionIdentifier, string contentPrefix) : base(expansionIdentifier, contentPrefix)
        {
            ExpansionDef = ScriptableObject.CreateInstance<TExpansionDef>();
            ExpansionDef.name = expansionIdentifier;
            string token = expansionIdentifier.ToUpperInvariant().Replace('.', '_');
            ExpansionDef.nameToken = token + "_NAME";
            ExpansionDef.descriptionToken = token + "_DESCRIPTION";
            ExpansionDef.disabledIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texUnlockIcon.png").WaitForCompletion();
            ExpansionDefs.Add(ExpansionDef);
        }

        public TIvylExpansionPack SetRequiredEntitlement(EntitlementDef requiredEntitlement)
        {
            ExpansionDef.requiredEntitlement = requiredEntitlement;
            return this as TIvylExpansionPack;
        }

        public TIvylExpansionPack SetIconSprite(Sprite iconSprite)
        {
            ExpansionDef.iconSprite = iconSprite;
            return this as TIvylExpansionPack;
        }

        public TIvylExpansionPack SetRunBehaviorPrefab(GameObject runBehaviorPrefab)
        {
            ExpansionDef.runBehaviorPrefab = runBehaviorPrefab;
            return this as TIvylExpansionPack;
        }

        protected override void AssignRequiredExpansion(ref ExpansionDef requiredExpansion)
        {
            requiredExpansion = ExpansionDef ?? requiredExpansion;
        }

        public string GetNameToken() => ExpansionDef.nameToken;

        public string GetDescriptionToken() => ExpansionDef.descriptionToken;
    }
}