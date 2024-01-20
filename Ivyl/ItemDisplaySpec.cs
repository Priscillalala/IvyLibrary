using RoR2;
using UnityEngine;

namespace IvyLibrary
{
    /// <summary>
    /// Groups an asset with an item display prefab.
    /// </summary>
    public struct ItemDisplaySpec
    {
        public UnityEngine.Object keyAsset;
        public GameObject displayModelPrefab;
        public LimbFlags limbMask;

        public ItemDisplaySpec(ItemDef keyAsset, GameObject displayModelPrefab, LimbFlags limbMask = LimbFlags.None)
        {
            this.keyAsset = keyAsset;
            this.displayModelPrefab = displayModelPrefab;
            this.limbMask = limbMask;
        }

        public ItemDisplaySpec(EquipmentDef keyAsset, GameObject displayModelPrefab, LimbFlags limbMask = LimbFlags.None)
        {
            this.keyAsset = keyAsset;
            this.displayModelPrefab = displayModelPrefab;
            this.limbMask = limbMask;
        }

        public ItemDisplaySpec(UnityEngine.Object keyAsset, GameObject displayModelPrefab, LimbFlags limbMask = LimbFlags.None)
        {
            this.keyAsset = keyAsset;
            this.displayModelPrefab = displayModelPrefab;
            this.limbMask = limbMask;
        }
    }
}