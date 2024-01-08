using RoR2;
using UnityEngine;

namespace IvyLibrary
{
    public static class SurfaceDefExtensions
    {
        public static TSurfaceDef SetApproximateColor<TSurfaceDef>(this TSurfaceDef surfaceDef, Color approximateColor) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.approximateColor = approximateColor;
            return surfaceDef;
        }

        public static TSurfaceDef SetImpactEffectPrefab<TSurfaceDef>(this TSurfaceDef surfaceDef, GameObject impactEffectPrefab) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.impactEffectPrefab = impactEffectPrefab;
            return surfaceDef;
        }

        public static TSurfaceDef SetFootstepEffectPrefab<TSurfaceDef>(this TSurfaceDef surfaceDef, GameObject footstepEffectPrefab) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.footstepEffectPrefab = footstepEffectPrefab;
            return surfaceDef;
        }

        public static TSurfaceDef SetImpactSoundString<TSurfaceDef>(this TSurfaceDef surfaceDef, string impactSoundString) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.impactSoundString = impactSoundString;
            return surfaceDef;
        }

        public static TSurfaceDef SetMaterialSwitchSoundString<TSurfaceDef>(this TSurfaceDef surfaceDef, string materialSwitchSoundString) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.materialSwitchString = materialSwitchSoundString;
            return surfaceDef;
        }

        public static TSurfaceDef SetFlags<TSurfaceDef>(this TSurfaceDef surfaceDef, SurfaceFlags flags) where TSurfaceDef : SurfaceDef
        {
            surfaceDef.isSlippery = (flags & SurfaceFlags.Slippery) > SurfaceFlags.None;
            return surfaceDef;
        }
    }
}