using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using System.Collections.Generic;
using HG;
using HG.Reflection;
using JetBrains.Annotations;
using System.Reflection;
using BepInEx.Logging;

namespace Ivyl
{
	internal static class AssetAssociatedBehaviorUtil 
	{
		internal static void CommenceAttributeSearch<TAttribute, TAsset>(Type behaviourType, Action<TAttribute, MethodInfo, TAsset> onAssetFound) where TAttribute : HG.Reflection.SearchableAttribute
		{
			List<TAttribute> attributeList = new List<TAttribute>();
			HG.Reflection.SearchableAttribute.GetInstances(attributeList);

			foreach (TAttribute attribute in attributeList)
			{
				if (attribute.target is not MethodInfo methodInfo)
				{
					Debug.LogError($"{nameof(TAttribute)} cannot be applied to object of type '{attribute.target?.GetType().Name}'");
					continue;
				}
				string cannotBeAppliedToMethod = $"{nameof(TAttribute)} cannot be applied to method {methodInfo.DeclaringType.FullName}.{methodInfo.Name}: ";
				if (!methodInfo.IsStatic)
				{
					Debug.LogError(cannotBeAppliedToMethod + $"Method is not static.");
				}
				else if (!behaviourType.IsAssignableFrom(methodInfo.DeclaringType))
				{
					Debug.LogError(cannotBeAppliedToMethod + $"{methodInfo.DeclaringType.FullName} does not derive from {behaviourType.FullName}.");
				}
				else if (methodInfo.DeclaringType.IsAbstract)
				{
					Debug.LogError(cannotBeAppliedToMethod + $"{methodInfo.DeclaringType.FullName} is an abstract type");
				}
				else if (!typeof(TAsset).IsAssignableFrom(methodInfo.ReturnType))
				{
					Debug.LogError(cannotBeAppliedToMethod + $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returns type '{methodInfo.ReturnType?.FullName ?? "void"}' instead of {typeof(TAsset).FullName}.");
				}
				else if (methodInfo.GetGenericArguments().Length != 0)
				{
					Debug.LogError(cannotBeAppliedToMethod + $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} must take no arguments.");
				}
				else
				{
					TAsset asset = (TAsset)methodInfo.Invoke(null, Array.Empty<object>());
					if (asset == null)
					{
						Debug.LogError($"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returned null.");
					} 
					else
                    {
						onAssetFound(attribute, methodInfo, asset);
                    }
				}
			}
		}

		internal static ref T GetNetworkContext<T>(ref T server, ref T client, ref T shared) where T : struct
		{
			if (NetworkServer.active)
			{
				if (NetworkClient.active)
				{
					return ref shared;
				}
				return ref server;
			}
			else
			{
				if (NetworkClient.active)
				{
					return ref client;
				}
				throw new InvalidOperationException("Neither server nor client is running.");
			}
		}
	}
}