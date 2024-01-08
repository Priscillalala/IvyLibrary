using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using HG.Reflection;
using System.Reflection;

namespace IvyLibrary
{
    public abstract class BaseAssetAssociatedBehavior<TAssociationAttribute, TNetworkContext> : MonoBehaviour
		where TAssociationAttribute : SearchableAttribute
		where TNetworkContext : struct
	{
		protected static TNetworkContext server;
		protected static TNetworkContext client;
		protected static TNetworkContext shared;

		protected static void CommenceAttributeSearch<TAsset>(Type behaviourType, Action<TAssociationAttribute, MethodInfo, TAsset> onAssetFound)
		{
			List<TAssociationAttribute> attributeList = new List<TAssociationAttribute>();
			SearchableAttribute.GetInstances(attributeList);

			foreach (TAssociationAttribute attribute in attributeList)
			{
				if (attribute.target is not MethodInfo methodInfo)
				{
					Debug.LogError($"{nameof(TAssociationAttribute)} cannot be applied to object of type '{attribute.target?.GetType().Name}'");
					continue;
				}
				string cannotBeAppliedToMethod = $"{nameof(TAssociationAttribute)} cannot be applied to method {methodInfo.DeclaringType.FullName}.{methodInfo.Name}: ";
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

		protected static ref TNetworkContext GetCurrentNetworkContext()
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