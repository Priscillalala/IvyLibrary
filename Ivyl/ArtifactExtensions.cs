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

namespace Ivyl
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

        /*public static TArtifactDef SetRequiredUnlockable<TArtifactDef>(this TArtifactDef artifactDef, UnlockableDef requiredUnlockable) where TArtifactDef : ArtifactDef
        {
            artifactDef.unlockableDef = requiredUnlockable;
            return artifactDef;
        }*/

        public static TArtifactDef SetArtifactCode<TArtifactDef>(this TArtifactDef artifactDef, ArtifactCode artifactCode) where TArtifactDef : ArtifactDef
        {
            ArtifactCodeAPI.artifactCodes.RemoveAll(x => x.Item1 == artifactDef);
            R2API.ScriptableObjects.ArtifactCode _artifactCode = artifactCode.GetInstance();
            ArtifactCodeAPI.AddCode(artifactDef, _artifactCode);
            UnityEngine.Object.Destroy(_artifactCode);
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
                enabledActions.onEnabledAction();
            }
        }

        private static void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactEnabledActions.TryGetValue(artifactDef, out (Action onEnabledAction, Action onDisabledAction) enabledActions))
            {
                enabledActions.onDisabledAction();
            }
        }
    }
}