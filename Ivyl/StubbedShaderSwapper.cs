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
using HG;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Ivyl
{
    public class StubbedShaderSwapper : MonoBehaviour
    {
        public static void Dispatch(AssetBundle assetBundle)
        {
            GameObject instance = new GameObject("StubbedShaderSwapper");
            DontDestroyOnLoad(instance);
            StubbedShaderSwapper stubbedShaderSwapper = instance.AddComponent<StubbedShaderSwapper>();
            stubbedShaderSwapper.StartCoroutine(stubbedShaderSwapper.SwapStubbedShaders(assetBundle));
        }

        private IEnumerator SwapStubbedShaders(AssetBundle assetBundle)
        {
            AssetBundleRequest allMatsRequest = assetBundle.LoadAllAssetsAsync<Material>();
            yield return new WaitUntil(() => allMatsRequest.isDone);
            int length = allMatsRequest.allAssets.Length;
            foreach (Material mat in allMatsRequest.allAssets)
            {
                string name = mat.shader.name;
                if (name.StartsWith("Stubbed"))
                {
                    AsyncOperationHandle<Shader> loadRealShaderOperation = default;
                    string path = name.Substring(7) + ".shader";
                    try
                    {
                        loadRealShaderOperation = Addressables.LoadAssetAsync<Shader>(path);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                    if (loadRealShaderOperation.IsValid())
                    {
                        yield return loadRealShaderOperation;
                        mat.shader = loadRealShaderOperation.Result ?? mat.shader;
                    }
                }
            }
            Destroy(base.gameObject);
        }
    }
}