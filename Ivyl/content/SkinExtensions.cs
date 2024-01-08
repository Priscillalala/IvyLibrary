using RoR2;
using UnityEngine;

namespace IvyLibrary
{
    public static class SkinExtensions
    {
        public static TSkinDef SetBaseSkins<TSkinDef>(this TSkinDef skinDef, params SkinDef[] baseSkins) where TSkinDef : SkinDef
        {
            skinDef.baseSkins = baseSkins;
            return skinDef;
        }

        public static TSkinDef SetIconSprite<TSkinDef>(this TSkinDef skinDef, Sprite iconSprite) where TSkinDef : SkinDef
        {
            skinDef.icon = iconSprite;
            return skinDef;
        }

        public static TSkinDef SetNameToken<TSkinDef>(this TSkinDef skinDef, string nameToken) where TSkinDef : SkinDef
        {
            skinDef.nameToken = nameToken;
            return skinDef;
        }

        public static TSkinDef SetRootObject<TSkinDef>(this TSkinDef skinDef, GameObject rootObject) where TSkinDef : SkinDef
        {
            skinDef.rootObject = rootObject;
            return skinDef;
        }

        public static TSkinDef SetRendererInfos<TSkinDef>(this TSkinDef skinDef, params CharacterModel.RendererInfo[] rendererInfos) where TSkinDef : SkinDef
        {
            skinDef.rendererInfos = rendererInfos;
            return skinDef;
        }

        public static TSkinDef SetGameObjectActivations<TSkinDef>(this TSkinDef skinDef, params SkinDef.GameObjectActivation[] gameObjectActivations) where TSkinDef : SkinDef
        {
            skinDef.gameObjectActivations = gameObjectActivations;
            return skinDef;
        }

        public static TSkinDef SetMeshReplacements<TSkinDef>(this TSkinDef skinDef, params SkinDef.MeshReplacement[] meshReplacements) where TSkinDef : SkinDef
        {
            skinDef.meshReplacements = meshReplacements;
            return skinDef;
        }

        public static TSkinDef SetProjectileGhostReplacements<TSkinDef>(this TSkinDef skinDef, params SkinDef.ProjectileGhostReplacement[] projectileGhostReplacements) where TSkinDef : SkinDef
        {
            skinDef.projectileGhostReplacements = projectileGhostReplacements;
            return skinDef;
        }

        public static TSkinDef SetMinionSkinReplacements<TSkinDef>(this TSkinDef skinDef, params SkinDef.MinionSkinReplacement[] minionSkinReplacements) where TSkinDef : SkinDef
        {
            skinDef.minionSkinReplacements = minionSkinReplacements;
            return skinDef;
        }
    }
}