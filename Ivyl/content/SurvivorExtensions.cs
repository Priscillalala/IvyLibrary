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

namespace IvyLibrary
{
    public static class SurvivorExtensions
    {
        public static TSurvivorDef SetBodyPrefab<TSurvivorDef>(this TSurvivorDef survivorDef, GameObject bodyPrefab) where TSurvivorDef : SurvivorDef
        {
            survivorDef.bodyPrefab = bodyPrefab;
            return survivorDef;
        }

        public static TSurvivorDef SetDisplayPrefab<TSurvivorDef>(this TSurvivorDef survivorDef, GameObject displayPrefab) where TSurvivorDef : SurvivorDef
        {
            survivorDef.displayPrefab = displayPrefab;
            return survivorDef;
        }

        public static TSurvivorDef SetPrimaryColor<TSurvivorDef>(this TSurvivorDef survivorDef, Color primaryColor) where TSurvivorDef : SurvivorDef
        {
            survivorDef.primaryColor = primaryColor;
            return survivorDef;
        }

        public static TSurvivorDef SetDesiredSortPosition<TSurvivorDef>(this TSurvivorDef survivorDef, float desiredSortPosition) where TSurvivorDef : SurvivorDef
        {
            survivorDef.desiredSortPosition = desiredSortPosition;
            return survivorDef;
        }

        public static TSurvivorDef SetFlags<TSurvivorDef>(this TSurvivorDef survivorDef, SurvivorFlags flags) where TSurvivorDef : SurvivorDef
        {
            survivorDef.hidden = (flags & SurvivorFlags.Hidden) > SurvivorFlags.None;
            return survivorDef;
        }
    }
}