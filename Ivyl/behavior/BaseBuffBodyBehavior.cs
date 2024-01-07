using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using System.Collections.Generic;
using HG;
using HG.Reflection;
using JetBrains.Annotations;
using System.Reflection;
using BepInEx.Logging;
using System.Linq;

namespace IvyLibrary
{
	public abstract class BaseBuffBodyBehavior : BaseAssetAssociatedBehavior<BaseBuffBodyBehavior.BuffDefAssociationAttribute, BaseBuffBodyBehavior.NetworkContext>
	{
		private static CharacterBody earlyAssignmentBody = null;
		private static Dictionary<UnityObjectWrapperKey<CharacterBody>, BaseBuffBodyBehavior[]> bodyToBuffBehaviors;

		public CharacterBody body { get; private set; }

		public int stack;

		protected void Awake()
		{
			body = earlyAssignmentBody;
			earlyAssignmentBody = null;
		}

		[SystemInitializer(typeof(BuffCatalog))]
		private static void Init()
		{
			var server = new List<BuffTypePair>();
			var client = new List<BuffTypePair>();
			var shared = new List<BuffTypePair>();

			void RegisterBehaviour(BuffDefAssociationAttribute attribute, MethodInfo methodInfo, BuffDef asset)
            {
				if (asset.buffIndex < 0)
				{
					Debug.LogError($"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returned a BuffDef that's not registered in the BuffCatalog. result={asset}");
				}
				else
				{
					BuffTypePair buffTypePair = new BuffTypePair
					{
						buffIndex = asset.buffIndex,
						behaviorType = methodInfo.DeclaringType,
					};
					if (attribute.useOnServer)
					{
						server.Add(buffTypePair with { index = server.Count });
					}
					if (attribute.useOnClient)
					{
						client.Add(buffTypePair with { index = client.Count });
					}
					if (attribute.useOnServer || attribute.useOnClient)
					{
						shared.Add(buffTypePair with { index = shared.Count });
					}
				}
			}

			CommenceAttributeSearch<BuffDef>(typeof(BaseBuffBodyBehavior), RegisterBehaviour);

			if (shared.Count <= 0)
			{
				return;
			}

			BaseBuffBodyBehavior.server.SetBuffTypePairs(server);
			BaseBuffBodyBehavior.client.SetBuffTypePairs(client);
			BaseBuffBodyBehavior.shared.SetBuffTypePairs(shared);
			bodyToBuffBehaviors = new Dictionary<UnityObjectWrapperKey<CharacterBody>, BaseBuffBodyBehavior[]>();

			CharacterBody.onBodyAwakeGlobal += OnBodyAwakeGlobal;
			CharacterBody.onBodyDestroyGlobal += OnBodyDestroyGlobal;
			On.RoR2.CharacterBody.SetBuffCount += CharacterBody_SetBuffCount;
		}
		private static void OnBodyAwakeGlobal(CharacterBody body)
		{
			BaseBuffBodyBehavior[] value = GetCurrentNetworkContext().behaviorArraysPool.Request();
			bodyToBuffBehaviors.Add(body, value);
		}

		private static void OnBodyDestroyGlobal(CharacterBody body)
		{
			BaseBuffBodyBehavior[] array = bodyToBuffBehaviors[body];
			for (int i = 0; i < array.Length; i++)
			{
				Destroy(array[i]);
			}
			bodyToBuffBehaviors.Remove(body);
			if (NetworkServer.active || NetworkClient.active)
			{
				GetCurrentNetworkContext().behaviorArraysPool.Return(array);
			}
		}

		private static void CharacterBody_SetBuffCount(On.RoR2.CharacterBody.orig_SetBuffCount orig, CharacterBody self, BuffIndex buffType, int newCount)
        {
			Lookup<BuffIndex, BuffTypePair> buffTypePairLookup = GetCurrentNetworkContext().buffTypePairsLookup;
			if (!buffTypePairLookup.Contains(buffType))
            {
				orig(self, buffType, newCount);
				return;
			}
			int stack = self.GetBuffCount(buffType);
			orig(self, buffType, newCount);
			if (stack != (stack = self.GetBuffCount(buffType)))
            {
				BaseBuffBodyBehavior[] array = bodyToBuffBehaviors[self];
				foreach (BuffTypePair buffTypePair in buffTypePairLookup[buffType])
				{
					SetBuffStack(self, ref array[buffTypePair.index], buffTypePair.behaviorType, stack);
				}
			}
		}

		private static void SetBuffStack(CharacterBody body, ref BaseBuffBodyBehavior behavior, Type behaviorType, int stack)
		{
			if (behavior == null != stack <= 0)
			{
				if (stack <= 0)
				{
					behavior.stack = 0;
					Destroy(behavior);
					behavior = null;
				}
				else
				{
					earlyAssignmentBody = body;
					behavior = (BaseBuffBodyBehavior)body.gameObject.AddComponent(behaviorType);
					earlyAssignmentBody = null;
				}
			}
			if (behavior != null)
			{
				behavior.stack = stack;
			}
		}

		public struct BuffTypePair
		{
			public BuffIndex buffIndex;
			public Type behaviorType;
			public int index;
		}

		public struct NetworkContext
		{
			public Lookup<BuffIndex, BuffTypePair> buffTypePairsLookup;
			public FixedSizeArrayPool<BaseBuffBodyBehavior> behaviorArraysPool;

			public void SetBuffTypePairs(List<BuffTypePair> buffTypePairs)
			{
				buffTypePairsLookup = (Lookup<BuffIndex, BuffTypePair>)buffTypePairs.ToLookup(x => x.buffIndex);
				behaviorArraysPool = new FixedSizeArrayPool<BaseBuffBodyBehavior>(buffTypePairs.Count);
			}
		}

		[MeansImplicitUse]
		[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
		public class BuffDefAssociationAttribute : HG.Reflection.SearchableAttribute
		{
			public bool useOnServer = true;

			public bool useOnClient = true;
		}
	}
}