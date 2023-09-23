using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System.Reflection;
using System;
using System.IO;
using System.ComponentModel;
using UnityEngine;

namespace Ivyl
{
    public abstract class AssetBundleInPlace<TSelf> : BaseInstanceInPlace<TSelf> where TSelf : AssetBundleInPlace<TSelf>, new()
    {
        public abstract string RelativePath { get; }

        public abstract bool SwapStubbedShaders { get; }

        public static AssetBundle AssetBundle { get; protected set; }

        protected sealed override bool Init()
        {
            string path = Path.Combine(Path.GetDirectoryName(typeof(TSelf).Assembly.Location), RelativePath);
            AssetBundle = AssetBundle.LoadFromFile(path);
            if (SwapStubbedShaders)
            {
                StubbedShaderSwapper.Dispatch(AssetBundle);
            }
            return AssetBundle;
        }

        public static UnityEngine.Object[] LoadAllAssets()
        {
            TryInit();
            return AssetBundle.LoadAllAssets();
        }

        public static T[] LoadAllAssets<T>() where T : UnityEngine.Object
        {
            TryInit();
            return AssetBundle.LoadAllAssets<T>();
        }

        public static UnityEngine.Object[] LoadAllAssets(Type type)
        {
            TryInit();
            return AssetBundle.LoadAllAssets(type);
        }

        public static AssetBundleRequest LoadAllAssetsAsync()
        {
            TryInit();
            return AssetBundle.LoadAllAssetsAsync();
        }

        public static AssetBundleRequest LoadAllAssetsAsync<T>() where T : UnityEngine.Object
        {
            TryInit();
            return AssetBundle.LoadAllAssetsAsync<T>();
        }

        public static AssetBundleRequest LoadAllAssetsAsync(Type type)
        {
            TryInit();
            return AssetBundle.LoadAllAssetsAsync(type);
        }

        public static UnityEngine.Object LoadAsset(string name)
        {
            TryInit();
            return AssetBundle.LoadAsset(name);
        }

        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            TryInit();
            return AssetBundle.LoadAsset<T>(name);
        }

        public static UnityEngine.Object LoadAsset(string name, Type type)
        {
            TryInit();
            return AssetBundle.LoadAsset(name, type);
        }

        public static AssetBundleRequest LoadAssetAsync(string name)
        {
            TryInit();
            return AssetBundle.LoadAssetAsync(name);
        }

        public static AssetBundleRequest LoadAssetAsync<T>(string name) where T : UnityEngine.Object
        {
            TryInit();
            return AssetBundle.LoadAssetAsync<T>(name);
        }

        public static AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            TryInit();
            return AssetBundle.LoadAssetAsync(name, type);
        }

        public static UnityEngine.Object[] LoadAssetWithSubAssets(string name)
        {
            TryInit();
            return AssetBundle.LoadAssetWithSubAssets(name);
        }

        public static T[] LoadAssetWithSubAssets<T>(string name) where T : UnityEngine.Object
        {
            TryInit();
            return AssetBundle.LoadAssetWithSubAssets<T>(name);
        }

        public static UnityEngine.Object[] LoadAssetWithSubAssets(string name, Type type)
        {
            TryInit();
            return AssetBundle.LoadAssetWithSubAssets(name, type);
        }

        public static AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
        {
            TryInit();
            return AssetBundle.LoadAssetWithSubAssetsAsync(name);
        }

        public static AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name) where T : UnityEngine.Object
        {
            TryInit();
            return AssetBundle.LoadAssetWithSubAssetsAsync<T>(name);
        }

        public static AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
        {
            TryInit();
            return AssetBundle.LoadAssetWithSubAssetsAsync(name, type);
        }
    }
}