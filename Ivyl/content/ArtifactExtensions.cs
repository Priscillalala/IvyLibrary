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
using System.Collections.Generic;

namespace IvyLibrary
{
    public static class ArtifactExtensions
    {
        public static TArtifactDef SetIconSprites<TArtifactDef>(this TArtifactDef artifactDef, Sprite enabledIconSprite, Sprite disabledIconSprite) where TArtifactDef : ArtifactDef
        {
            artifactDef.smallIconSelectedSprite = enabledIconSprite;
            artifactDef.smallIconDeselectedSprite = disabledIconSprite;
            return artifactDef;
        }

        public static TArtifactDef SetPickupModelPrefab<TArtifactDef>(this TArtifactDef artifactDef, GameObject pickupModelPrefab) where TArtifactDef : ArtifactDef
        {
            artifactDef.pickupModelPrefab = pickupModelPrefab;
            return artifactDef;
        }

        public static TArtifactDef SetArtifactCode<TArtifactDef>(this TArtifactDef artifactDef, ArtifactCode artifactCode) where TArtifactDef : ArtifactDef
        {
            for (int i = ArtifactCodeAPI.artifactCodes.Count - 1; i >= 0; i--)
            {
                var code = ArtifactCodeAPI.artifactCodes[i];
                if (code.Item1 == artifactDef)
                {
                    UnityEngine.Object.Destroy(code.Item2);
                    ArtifactCodeAPI.artifactCodes.RemoveAt(i);
                }
            }
            Sha256HashAsset hashAsset = ScriptableObject.CreateInstance<Sha256HashAsset>();
            hashAsset.value = artifactCode.CreateCodeHash();
            ArtifactCodeAPI.AddCode(artifactDef, hashAsset);
            return artifactDef;
        }

        private static Dictionary<ArtifactDef, (Action onEnabledAction, Action onDisabledAction)> artifactEnabledActions;

        public static TArtifactDef SetEnabledActions<TArtifactDef>(this TArtifactDef artifactDef, Action onEnabledAction, Action onDisabledAction) where TArtifactDef : ArtifactDef
        {
            if (artifactEnabledActions == null)
            {
                artifactEnabledActions = new Dictionary<ArtifactDef, (Action onEnabledAction, Action onDisabledAction)>();
                RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
                RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
            }
            artifactEnabledActions[artifactDef] = (onEnabledAction, onDisabledAction);
            return artifactDef;
        }

        private static void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactEnabledActions.TryGetValue(artifactDef, out (Action onEnabledAction, Action onDisabledAction) enabledActions))
            {
                enabledActions.onEnabledAction?.Invoke();
            }
        }

        private static void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactEnabledActions.TryGetValue(artifactDef, out (Action onEnabledAction, Action onDisabledAction) enabledActions))
            {
                enabledActions.onDisabledAction?.Invoke();
            }
        }
    }
}