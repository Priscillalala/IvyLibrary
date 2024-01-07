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
using System.Threading.Tasks;

namespace IvyLibrary
{
    public struct ArtifactCode
    {
        public (ArtifactCompound, ArtifactCompound, ArtifactCompound) topRow;
        public (ArtifactCompound, ArtifactCompound, ArtifactCompound) middleRow;
        public (ArtifactCompound, ArtifactCompound, ArtifactCompound) bottomRow;

        public ArtifactCode(
            ArtifactCompound topLeft, ArtifactCompound topCenter, ArtifactCompound topRight, 
            ArtifactCompound middleLeft, ArtifactCompound middleCenter, ArtifactCompound middleRight, 
            ArtifactCompound bottomLeft, ArtifactCompound bottomCenter, ArtifactCompound bottomRight)
        {
            topRow = (topLeft, topCenter, topRight);
            middleRow = (middleLeft, middleCenter, middleRight);
            bottomRow = (bottomLeft, bottomCenter, bottomRight);
        }

        public ArtifactCode(
            (ArtifactCompound, ArtifactCompound, ArtifactCompound) topRow, 
            (ArtifactCompound, ArtifactCompound, ArtifactCompound) middleRow, 
            (ArtifactCompound, ArtifactCompound, ArtifactCompound) bottomRow)
        {
            this.topRow = topRow;
            this.middleRow = middleRow;
            this.bottomRow = bottomRow;
        }

        public ArtifactCode SetTopRow(ArtifactCompound left, ArtifactCompound center, ArtifactCompound right)
        {
            topRow = (left, center, right);
            return this;
        }

        public ArtifactCode SetMiddleRow(ArtifactCompound left, ArtifactCompound center, ArtifactCompound right)
        {
            middleRow = (left, center, right);
            return this;
        }

        public ArtifactCode SetBottomRow(ArtifactCompound left, ArtifactCompound center, ArtifactCompound right)
        {
            bottomRow = (left, center, right);
            return this;
        }

        public R2API.ScriptableObjects.ArtifactCode GetInstance()
        {
            R2API.ScriptableObjects.ArtifactCode _artifactCode = ScriptableObject.CreateInstance<R2API.ScriptableObjects.ArtifactCode>();
            _artifactCode.topRow = new Vector3Int((int)topRow.Item1, (int)topRow.Item2, (int)topRow.Item3);
            _artifactCode.middleRow = new Vector3Int((int)middleRow.Item1, (int)middleRow.Item2, (int)middleRow.Item3);
            _artifactCode.bottomRow = new Vector3Int((int)bottomRow.Item1, (int)bottomRow.Item2, (int)bottomRow.Item3);
            return _artifactCode;
        }

        public void CopyToFormulaDisplay(ArtifactFormulaDisplay formulaDisplay)
        {
            formulaDisplay.artifactCompoundDisplayInfos = new[]
            {
                GetDisplayInfo(formulaDisplay, topRow.Item1, "Slot 1,1"),
                GetDisplayInfo(formulaDisplay, topRow.Item2, "Slot 1,2"),
                GetDisplayInfo(formulaDisplay, topRow.Item3, "Slot 1,3"),
                GetDisplayInfo(formulaDisplay, middleRow.Item1, "Slot 2,1"),
                GetDisplayInfo(formulaDisplay, middleRow.Item2, "Slot 2,2"),
                GetDisplayInfo(formulaDisplay, middleRow.Item3, "Slot 2,3"),
                GetDisplayInfo(formulaDisplay, bottomRow.Item1, "Slot 3,1"),
                GetDisplayInfo(formulaDisplay, bottomRow.Item2, "Slot 3,2"),
                GetDisplayInfo(formulaDisplay, bottomRow.Item3, "Slot 3,3"),
            };
        }

        private ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo GetDisplayInfo(ArtifactFormulaDisplay formulaDisplay, ArtifactCompound compound, string decalPath)
        {
            return new ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo
            {
                artifactCompoundDef = compound.FindArtifactCompoundDef(),
                decal = formulaDisplay.transform.Find(decalPath)?.GetComponent<Decal>()
            };
        }

        public async Task CopyToFormulaDisplayAsync(ArtifactFormulaDisplay formulaDisplay)
        {
            formulaDisplay.artifactCompoundDisplayInfos = new[]
            {
                await GetDisplayInfoAsync(formulaDisplay, topRow.Item1, "Slot 1,1"),
                await GetDisplayInfoAsync(formulaDisplay, topRow.Item2, "Slot 1,2"),
                await GetDisplayInfoAsync(formulaDisplay, topRow.Item3, "Slot 1,3"),
                await GetDisplayInfoAsync(formulaDisplay, middleRow.Item1, "Slot 2,1"),
                await GetDisplayInfoAsync(formulaDisplay, middleRow.Item2, "Slot 2,2"),
                await GetDisplayInfoAsync(formulaDisplay, middleRow.Item3, "Slot 2,3"),
                await GetDisplayInfoAsync(formulaDisplay, bottomRow.Item1, "Slot 3,1"),
                await GetDisplayInfoAsync(formulaDisplay, bottomRow.Item2, "Slot 3,2"),
                await GetDisplayInfoAsync(formulaDisplay, bottomRow.Item3, "Slot 3,3"),
            };
        }

        private async Task<ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo> GetDisplayInfoAsync(ArtifactFormulaDisplay formulaDisplay, ArtifactCompound compound, string decalPath)
        {
            return new ArtifactFormulaDisplay.ArtifactCompoundDisplayInfo
            {
                artifactCompoundDef = await compound.FindArtifactCompoundDefAsync(),
                decal = formulaDisplay.transform.Find(decalPath)?.GetComponent<Decal>()
            };
        }
    }
}