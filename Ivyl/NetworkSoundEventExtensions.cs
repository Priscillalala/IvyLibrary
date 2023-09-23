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
    public static class NetworkSoundEventExtensions
    {
        public static TNetworkSoundEventDef SetFlags<TNetworkSoundEventDef>(this TNetworkSoundEventDef networkSoundEventDef, string eventName) where TNetworkSoundEventDef : NetworkSoundEventDef
        {
            networkSoundEventDef.eventName = eventName;
            return networkSoundEventDef;
        }
    }
}