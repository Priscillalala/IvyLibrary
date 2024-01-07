using System;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;
using UnityEngine.Networking;

namespace IvyLibrary
{
    public static class Prefabs
    {
        private static Transform _prefabParent;

        private static void InitPrefabParent()
        {
            if (_prefabParent)
            {
                return;
            }
            _prefabParent = new GameObject("IVYLPrefabs").transform;
            _prefabParent.gameObject.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(_prefabParent.gameObject);
            On.RoR2.Util.IsPrefab += (orig, gameObject) => gameObject.transform.parent == _prefabParent || orig(gameObject);
        }

        public static GameObject CreatePrefab(string name)
        {
            InitPrefabParent();
            GameObject prefab = new GameObject(name);
            prefab.transform.SetParent(_prefabParent);
            return prefab;
        }

        public static GameObject ClonePrefab(GameObject original, string name)
        {
            InitPrefabParent();
            GameObject prefab = UnityEngine.Object.Instantiate(original, _prefabParent);
            prefab.name = name;
            if (prefab.TryGetComponent(out NetworkIdentity networkIdentity))
            {
                networkIdentity.m_AssetId.Reset();
            }
            return prefab;
        }
    }
}