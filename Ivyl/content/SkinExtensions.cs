using RoR2;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="SkinDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class SkinExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetBaseSkins<TSkinDef>(this TSkinDef skinDef, params SkinDef[] baseSkins) where TSkinDef : SkinDef
        {
            skinDef.baseSkins = baseSkins;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetIconSprite<TSkinDef>(this TSkinDef skinDef, Sprite iconSprite) where TSkinDef : SkinDef
        {
            skinDef.icon = iconSprite;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetNameToken<TSkinDef>(this TSkinDef skinDef, string nameToken) where TSkinDef : SkinDef
        {
            skinDef.nameToken = nameToken;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetRootObject<TSkinDef>(this TSkinDef skinDef, GameObject rootObject) where TSkinDef : SkinDef
        {
            skinDef.rootObject = rootObject;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetRendererInfos<TSkinDef>(this TSkinDef skinDef, params CharacterModel.RendererInfo[] rendererInfos) where TSkinDef : SkinDef
        {
            skinDef.rendererInfos = rendererInfos;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetGameObjectActivations<TSkinDef>(this TSkinDef skinDef, params SkinDef.GameObjectActivation[] gameObjectActivations) where TSkinDef : SkinDef
        {
            skinDef.gameObjectActivations = gameObjectActivations;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetMeshReplacements<TSkinDef>(this TSkinDef skinDef, params SkinDef.MeshReplacement[] meshReplacements) where TSkinDef : SkinDef
        {
            skinDef.meshReplacements = meshReplacements;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetProjectileGhostReplacements<TSkinDef>(this TSkinDef skinDef, params SkinDef.ProjectileGhostReplacement[] projectileGhostReplacements) where TSkinDef : SkinDef
        {
            skinDef.projectileGhostReplacements = projectileGhostReplacements;
            return skinDef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSkinDef SetMinionSkinReplacements<TSkinDef>(this TSkinDef skinDef, params SkinDef.MinionSkinReplacement[] minionSkinReplacements) where TSkinDef : SkinDef
        {
            skinDef.minionSkinReplacements = minionSkinReplacements;
            return skinDef;
        }
    }
}