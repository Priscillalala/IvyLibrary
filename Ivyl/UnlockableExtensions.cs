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

namespace Ivyl
{
    public static class UnlockableExtensions
    {
        public static TUnlockableDef SetFlags<TUnlockableDef>(this TUnlockableDef unlockableDef, UnlockableFlags flags) where TUnlockableDef : UnlockableDef
        {
            unlockableDef.hidden = (flags & UnlockableFlags.Hidden) > UnlockableFlags.None;
            return unlockableDef;
        }

        public static TUnlockableDef SetNameToken<TUnlockableDef>(this TUnlockableDef unlockableDef, string nameToken) where TUnlockableDef : UnlockableDef
        {
            unlockableDef.nameToken = nameToken;
            return unlockableDef;
        }

        /*public static TUnlockableDef SetUnlockStringFunctions<TUnlockableDef>(this TUnlockableDef unlockableDef, Func<string> howToUnlockStringFunction, Func<string> unlockedStringFunction) where TUnlockableDef : UnlockableDef
        {
            unlockableDef.getHowToUnlockString = howToUnlockStringFunction;
            unlockableDef.getUnlockedString = unlockedStringFunction;
            return unlockableDef;
        }*/
    }
}