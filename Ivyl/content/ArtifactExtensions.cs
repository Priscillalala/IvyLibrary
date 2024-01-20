using System;
using RoR2;
using UnityEngine;
using R2API;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating an <see cref="ArtifactDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class ArtifactExtensions
    {
        /// <summary>
        /// Set the enabled and disabled sprites of this artifact in the lobby.
        /// </summary>
        /// <remarks>
        /// Artifact sprites are usually 64px.
        /// </remarks>
        /// <returns><paramref name="artifactDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TArtifactDef SetIconSprites<TArtifactDef>(this TArtifactDef artifactDef, Sprite enabledIconSprite, Sprite disabledIconSprite) where TArtifactDef : ArtifactDef
        {
            artifactDef.smallIconSelectedSprite = enabledIconSprite;
            artifactDef.smallIconDeselectedSprite = disabledIconSprite;
            return artifactDef;
        }

        /// <summary>
        /// Set the physical model of this artifact in the world (e.g., Bulwark's Ambry).
        /// </summary>
        /// <returns><paramref name="artifactDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TArtifactDef SetPickupModelPrefab<TArtifactDef>(this TArtifactDef artifactDef, GameObject pickupModelPrefab) where TArtifactDef : ArtifactDef
        {
            artifactDef.pickupModelPrefab = pickupModelPrefab;
            return artifactDef;
        }

        /// <summary>
        /// Associate an artifact code with this <see cref="ArtifactDef"/> as input for an artifact portal (e.g., on Sky Meadow).
        /// </summary>
        /// <remarks>
        /// The internal implementation of this method uses <see cref="ArtifactCodeAPI"/>.
        /// </remarks>
        /// <returns><paramref name="artifactDef"/>, to continue a method chain.</returns>
        public static TArtifactDef SetArtifactCode<TArtifactDef>(this TArtifactDef artifactDef, ArtifactCode? artifactCode) where TArtifactDef : ArtifactDef
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
            if (artifactCode.HasValue)
            {
                Sha256HashAsset hashAsset = ScriptableObject.CreateInstance<Sha256HashAsset>();
                hashAsset.value = artifactCode.Value.CreateCodeHash();
                ArtifactCodeAPI.AddCode(artifactDef, hashAsset);
            }
            return artifactDef;
        }

        private static Dictionary<ArtifactDef, (Action onEnabledAction, Action onDisabledAction)> artifactEnabledActions;

        /// <summary>
        /// Register callbacks that are invoked when this <see cref="ArtifactDef"/> is enabled or disabled in a <see cref="Run"/>.
        /// </summary>
        /// <remarks>
        /// These callbacks are useful for settings hooks that are only relevant when this artifact is active.
        /// </remarks>
        /// <returns><paramref name="artifactDef"/>, to continue a method chain.</returns>
        public static TArtifactDef SetEnabledActions<TArtifactDef>(this TArtifactDef artifactDef, Action onEnabledAction, Action onDisabledAction) where TArtifactDef : ArtifactDef
        {
            if (artifactEnabledActions == null)
            {
                artifactEnabledActions = new Dictionary<ArtifactDef, (Action onEnabledAction, Action onDisabledAction)>();
                RunArtifactManager.onArtifactEnabledGlobal += OnArtifactEnabledGlobal;
                RunArtifactManager.onArtifactDisabledGlobal += OnArtifactDisabledGlobal;
            }
            artifactEnabledActions[artifactDef] = (onEnabledAction, onDisabledAction);
            return artifactDef;

            static void OnArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
            {
                if (artifactEnabledActions.TryGetValue(artifactDef, out (Action onEnabledAction, Action onDisabledAction) enabledActions))
                {
                    enabledActions.onEnabledAction?.Invoke();
                }
            }

            static void OnArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
            {
                if (artifactEnabledActions.TryGetValue(artifactDef, out (Action onEnabledAction, Action onDisabledAction) enabledActions))
                {
                    enabledActions.onDisabledAction?.Invoke();
                }
            }
        }
    }
}