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
    public static class BuffExtensions
    {
        public static TBuffDef SetIconSprite<TBuffDef>(this TBuffDef buffDef, Sprite iconSprite, Color spriteColor) where TBuffDef : BuffDef
        {
            buffDef.iconSprite = iconSprite;
            buffDef.buffColor = spriteColor;
            return buffDef;
        }

        public static TBuffDef SetIconSprite<TBuffDef>(this TBuffDef buffDef, Sprite iconSprite) where TBuffDef : BuffDef
        {
            buffDef.iconSprite = iconSprite;
            return buffDef;
        }

        public static TBuffDef SetFlags<TBuffDef>(this TBuffDef buffDef, BuffFlags flags) where TBuffDef : BuffDef
        {
            buffDef.canStack = (flags & BuffFlags.Stackable) > BuffFlags.None;
            buffDef.isDebuff = (flags & BuffFlags.Debuff) > BuffFlags.None;
            buffDef.isCooldown = (flags & BuffFlags.Cooldown) > BuffFlags.None;
            buffDef.isHidden = (flags & BuffFlags.Hidden) > BuffFlags.None;
            return buffDef;
        }

        public static TBuffDef SetStartSfx<TBuffDef>(this TBuffDef buffDef, NetworkSoundEventDef startSfx) where TBuffDef : BuffDef
        {
            buffDef.startSfx = startSfx;
            return buffDef;
        }
    }
}