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

namespace IvyLibrary
{
    public static class NetworkSoundEventExtensions
    {
        public static TNetworkSoundEventDef SetEventName<TNetworkSoundEventDef>(this TNetworkSoundEventDef networkSoundEventDef, string eventName) where TNetworkSoundEventDef : NetworkSoundEventDef
        {
            networkSoundEventDef.eventName = eventName;
            return networkSoundEventDef;
        }
    }
}