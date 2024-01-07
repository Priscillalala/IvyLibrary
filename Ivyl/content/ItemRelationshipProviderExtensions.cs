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
    public static class ItemRelationshipProviderExtensions
    {
        public static TItemRelationshipProvider SetRelationshipType<TItemRelationshipProvider>(this TItemRelationshipProvider itemRelationshipProvider, ItemRelationshipType relationshipType) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            itemRelationshipProvider.relationshipType = relationshipType;
            return itemRelationshipProvider;
        }

        public static TItemRelationshipProvider SetRelationships<TItemRelationshipProvider>(this TItemRelationshipProvider itemRelationshipProvider, params ItemDef.Pair[] relationships) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            itemRelationshipProvider.relationships = relationships;
            return itemRelationshipProvider;
        }
    }
}