using UnityEngine;
using RoR2.ExpansionManagement;
using RoR2.EntitlementManagement;

namespace IvyLibrary
{
    public static class ExpansionExtensions
    {
        public static TExpansionDef SetRequiredEntitlement<TExpansionDef>(this TExpansionDef expansionDef, EntitlementDef requiredEntitlement) where TExpansionDef : ExpansionDef
        {
            expansionDef.requiredEntitlement = requiredEntitlement;
            return expansionDef;
        }

        public static TExpansionDef SetIconSprite<TExpansionDef>(this TExpansionDef expansionDef, Sprite iconSprite) where TExpansionDef : ExpansionDef
        {
            expansionDef.iconSprite = iconSprite;
            return expansionDef;
        }

        public static TExpansionDef SetRunBehaviorPrefab<TExpansionDef>(this TExpansionDef expansionDef, GameObject runBehaviorPrefab) where TExpansionDef : ExpansionDef
        {
            expansionDef.runBehaviorPrefab = runBehaviorPrefab;
            return expansionDef;
        }
    }
}