using BepInEx;
using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using MonoMod.Cil;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using UnityEngine;
using System.Collections.Generic;
using R2API.ScriptableObjects;
using UnityEngine.AddressableAssets;
using System.Linq;
using R2API;
using System.Runtime.CompilerServices;
using System.Collections;
using RoR2.ContentManagement;
using UnityEngine.Events;
using BepInEx.Configuration;
using RoR2;
using ThreeEyedGames;

namespace Ivyl
{
    public record class DifficultyWrapper(DifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex) 
        : DifficultyWrapper<DifficultyWrapper, DifficultyDef>(DifficultyDef, DifficultyIndex) 
    { }

    public record class DifficultyWrapper<TDifficultyDef>(TDifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex) 
        : DifficultyWrapper<DifficultyWrapper<TDifficultyDef>, TDifficultyDef>(DifficultyDef, DifficultyIndex) where TDifficultyDef : DifficultyDef
    { }

    public abstract record class DifficultyWrapper<TDifficultyWrapper, TDifficultyDef>(TDifficultyDef DifficultyDef, DifficultyIndex DifficultyIndex)
        where TDifficultyWrapper : DifficultyWrapper<TDifficultyWrapper, TDifficultyDef>
        where TDifficultyDef : DifficultyDef
    {
        public TDifficultyWrapper SetScalingValue(float scalingValue)
        {
            DifficultyDef.scalingValue = scalingValue;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetIconSprite(Sprite iconSprite)
        {
            DifficultyDef.iconSprite = iconSprite;
            DifficultyDef.foundIconSprite = true;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetColor(Color color)
        {
            DifficultyDef.color = color;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetServerTag(string serverTag)
        {
            DifficultyDef.serverTag = serverTag;
            return this as TDifficultyWrapper;
        }

        public TDifficultyWrapper SetFlags(DifficultyFlags flags)
        {
            DifficultyDef.countsAsHardMode = (flags & DifficultyFlags.HardMode) > 0;
            if ((flags & DifficultyFlags.Hidden) > 0)
            {
                DifficultyAPI.hiddenDifficulties.Add(DifficultyDef);
            } 
            else
            {
                DifficultyAPI.hiddenDifficulties.Remove(DifficultyDef);
            }
            return this as TDifficultyWrapper;
        }

        public string GetNameToken() => DifficultyDef.nameToken;

        public string GetDescriptionToken() => DifficultyDef.descriptionToken;
    }
}