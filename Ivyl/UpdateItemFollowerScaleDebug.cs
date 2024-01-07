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
using RoR2;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace IvyLibrary
{
    public class UpdateItemFollowerScaleDebug : MonoBehaviour
    {
        private ItemFollower itemFollower;

        public void Awake()
        {
            itemFollower = base.GetComponent<ItemFollower>();
        }

        public void Update()
        {
            if (itemFollower && itemFollower.followerInstance)
            {
                itemFollower.followerInstance.transform.localScale = itemFollower.transform.localScale;
            }
        }
    }

}
