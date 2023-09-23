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
using System.Linq;
using RoR2.Skills;
using R2API.ScriptableObjects;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BepInEx.Bootstrap;
using HG.GeneralSerializer;
using UnityEngine.Rendering;

[module: UnverifiableCode]
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace Ivyl
{
    public static class IvyLibrary
    {
        [SystemInitializer]
        private static void InitIOnGetStatCoefficientsReciever()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            List<IOnGetStatCoefficientsReciever> getStatCoefficientsRecievers = GetComponentsCache<IOnGetStatCoefficientsReciever>.GetGameObjectComponents(sender.gameObject);
            foreach (IOnGetStatCoefficientsReciever reciever in getStatCoefficientsRecievers)
            {
                reciever.OnGetStatCoefficients(args);
            }
            GetComponentsCache<IOnGetStatCoefficientsReciever>.ReturnBuffer(getStatCoefficientsRecievers);
        }

        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, Vector3 modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
        {
            return SetupModelPanelParameters(model, Quaternion.Euler(modelRotation), minDistance, maxDistance, focusPoint, cameraPosition);
        }

        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, ModelPanelParams info)
        {
            return SetupModelPanelParameters(model, info.modelRotation, info.minDistance, info.maxDistance, info.focusPoint, info.cameraPosition);
        }

        public static ModelPanelParameters SetupModelPanelParameters(GameObject model, Quaternion modelRotation, float minDistance, float maxDistance, Transform focusPoint = null, Transform cameraPosition = null)
        {
            ModelPanelParameters parameters = model.AddComponent<ModelPanelParameters>();
            parameters.modelRotation = modelRotation;
            parameters.minDistance = minDistance;
            parameters.maxDistance = maxDistance;
            parameters.focusPointTransform = focusPoint ?? model.transform.Find("FocusPoint") ?? model.transform.Find("Focus Point");
            if (!parameters.focusPointTransform)
            {
                Transform newFocusPoint = new GameObject("FocusPoint").transform;
                newFocusPoint.SetParent(model.transform);
                parameters.focusPointTransform = newFocusPoint;
            }
            parameters.cameraPositionTransform = cameraPosition ?? model.transform.Find("CameraPosition") ?? model.transform.Find("Camera Position");
            if (!parameters.cameraPositionTransform)
            {
                Transform newCameraPosition = new GameObject("CameraPosition").transform;
                newCameraPosition.SetParent(model.transform);
                newCameraPosition.localPosition = model.transform.forward;
                parameters.cameraPositionTransform = newCameraPosition;
            }
            return parameters;
        }

        public static ItemDisplay SetupItemDisplay(GameObject displayModelPrefab)
        {
            ItemDisplay itemDisplay = displayModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = displayModelPrefab.GetComponentsInChildren<Renderer>()
                .Where(x => x is MeshRenderer or SkinnedMeshRenderer)
                .Select(x => new CharacterModel.RendererInfo
                {
                    defaultMaterial = x.sharedMaterial,
                    renderer = x,
                    defaultShadowCastingMode = ShadowCastingMode.On,
                    ignoreOverlays = false
                })
                .ToArray();
            return itemDisplay;
        }

        public static ArtifactCompoundDef FindArtifactCompoundDef(this ArtifactCompound artifactCompound)
        {
            return artifactCompound switch
            {
                ArtifactCompound.Circle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdCircle.asset").WaitForCompletion(),
                ArtifactCompound.Triangle => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdTriangle.asset").WaitForCompletion(),
                ArtifactCompound.Diamond => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdDiamond.asset").WaitForCompletion(),
                ArtifactCompound.Square => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdSquare.asset").WaitForCompletion(),
                ArtifactCompound.Empty => Addressables.LoadAssetAsync<ArtifactCompoundDef>("RoR2/Base/ArtifactCompounds/acdEmpty.asset").WaitForCompletion(),
                _ => ArtifactCodeAPI.artifactCompounds.FirstOrDefault(x => x.value == (int)artifactCompound)
            };
        }

        public static bool TryFindArtifactCompoundDef(ArtifactCompound artifactCompound, out ArtifactCompoundDef artifactCompoundDef)
        {
            return artifactCompoundDef = FindArtifactCompoundDef(artifactCompound);
        }

        public static string Nicify(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            char[] charArray = Regex.Replace(str, @"((?<!^)((?<=[^_])[\p{Lu}][\p{Ll}]|(?<=[\p{Ll}])[\p{Lu}])|(?<=[\p{Ll}])[\d])", @" $1", RegexOptions.Compiled).ToCharArray();
            int i = 0;
            if (char.IsLower(charArray[0]))
            {
                i = 1;
                charArray[0] = char.ToUpperInvariant(charArray[0]);
            }
            while (i < charArray.Length - 1)
            {
                if (charArray[i] == '_')
                {
                    charArray[i] = ' ';
                    if (char.IsLower(charArray[i + 1]))
                    {
                        i++;
                        charArray[i + 1] = char.ToUpperInvariant(charArray[i + 1]);
                    }
                }
                i++;
            }
            if (charArray[charArray.Length - 1] == '_')
            {
                charArray[charArray.Length - 1] = ' ';
            }
            return new string(charArray).Trim();
        }

        public static SkillFamily FindSkillFamily(GameObject bodyPrefab, SkillSlot slot)
        {
            if (bodyPrefab.TryGetComponent(out SkillLocator skillLocator))
            {
                return skillLocator.GetSkill(slot)?.skillFamily;
            }
            return null;
        }

        public static float StackScaling(float baseValue, float stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0f;
        }

        public static int StackScaling(int baseValue, int stackValue, int stack)
        {
            if (stack > 0)
            {
                return baseValue + ((stack - 1) * stackValue);
            }
            return 0;
        }

        public static bool IsModLoaded(string guid) => Chainloader.PluginInfos.ContainsKey(guid);
    }
}