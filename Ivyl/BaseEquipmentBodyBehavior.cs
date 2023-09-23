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
using System.Linq;

namespace Ivyl
{
	public abstract class BaseEquipmentBodyBehavior : MonoBehaviour
	{
		private static CharacterBody earlyAssignmentBody = null;
		private static Dictionary<UnityObjectWrapperKey<CharacterBody>, EquipmentBehaviorsState> bodyToEquipmentBehaviors;

		public CharacterBody body { get; private set; }

		public EquipmentState state;

		protected void Awake()
		{
			body = earlyAssignmentBody;
			earlyAssignmentBody = null;
		}

		[SystemInitializer(typeof(EquipmentCatalog))]
		private static void Init()
		{
			List<EquipmentTypePair> server = new List<EquipmentTypePair>();
			List<EquipmentTypePair> client = new List<EquipmentTypePair>();
			List<EquipmentTypePair> shared = new List<EquipmentTypePair>();

			void RegisterBehaviour(EquipmentDefDefAssociationAttribute attribute, MethodInfo methodInfo, EquipmentDef asset)
            {
				if (asset.equipmentIndex < 0)
				{
					Debug.LogError($"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returned an EquipmentDef that's not registered in the EquipmentCatalog. result={asset}");
				}
				else
				{
					EquipmentTypePair itemTypePair = new EquipmentTypePair
					{
						equipmentIndex = asset.equipmentIndex,
						behaviorType = methodInfo.DeclaringType,
					};
					if (attribute.useOnServer)
					{
						server.Add(itemTypePair);
					}
					if (attribute.useOnClient)
					{
						client.Add(itemTypePair);
					}
					if (attribute.useOnServer || attribute.useOnClient)
					{
						shared.Add(itemTypePair);
					}
				}
			}

			AssetAssociatedBehaviorUtil.CommenceAttributeSearch<EquipmentDefDefAssociationAttribute, EquipmentDef>(typeof(BaseEquipmentBodyBehavior), RegisterBehaviour);

			if (shared.Count <= 0)
            {
				return;
            }

			NetworkContext.server.SetEquipmentTypePairs(server);
			NetworkContext.client.SetEquipmentTypePairs(client);
			NetworkContext.shared.SetEquipmentTypePairs(shared);
			bodyToEquipmentBehaviors = new Dictionary<UnityObjectWrapperKey<CharacterBody>, EquipmentBehaviorsState>();

            CharacterBody.onBodyDestroyGlobal += CharacterBody_onBodyDestroyGlobal;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
		}

        private static void CharacterBody_onBodyDestroyGlobal(CharacterBody body)
        {
			if (bodyToEquipmentBehaviors.TryGetValue(body, out EquipmentBehaviorsState state))
			{
				for (int i = 0; i < state.activeBehaviorsArray.Length; i++)
				{
					Destroy(state.activeBehaviorsArray[i]);
				}
				bodyToEquipmentBehaviors.Remove(body);
			}
		}

		private static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
		{
			UpdateBodyEquipmentBehavior(body);
		}

		private static void UpdateBodyEquipmentBehavior(CharacterBody body)
		{
			Inventory inventory = body.inventory;
			if (bodyToEquipmentBehaviors.TryGetValue(body, out EquipmentBehaviorsState state))
			{
				if (state.currentEquipmentIndex != inventory.currentEquipmentIndex)
				{
					if (state.activeBehaviorsArray != null)
					{
						for (int i = 0; i < state.activeBehaviorsArray.Length; i++)
						{
							state.activeBehaviorsArray[i].state = EquipmentState.empty;
							Destroy(state.activeBehaviorsArray[i]);
						}
					}
					if (NetworkContext.Current.equipmentTypePairsDict.TryGetValue(inventory.currentEquipmentIndex, out EquipmentTypePair[] equipmentTypePairs))
                    {
						SetActiveBehaviors(body, ref state.activeBehaviorsArray, equipmentTypePairs);
					} 
					else
                    {
						state.activeBehaviorsArray = null;
                    }
				}
			} 
			else
            {
				if (!inventory || !NetworkContext.Current.equipmentTypePairsDict.TryGetValue(inventory.currentEquipmentIndex, out EquipmentTypePair[] equipmentTypePairs))
				{
					return;
				}
				SetActiveBehaviors(body, ref state.activeBehaviorsArray, equipmentTypePairs);
			}
			state.currentEquipmentIndex = inventory.currentEquipmentIndex;
			bodyToEquipmentBehaviors[body] = state;
			if (state.activeBehaviorsArray != null)
            {
				for (int i = 0; i < state.activeBehaviorsArray.Length; i++)
                {
					state.activeBehaviorsArray[i].state = inventory.currentEquipmentState;
                }
            }
		}

		private static void SetActiveBehaviors(CharacterBody body, ref BaseEquipmentBodyBehavior[] activeBehaviorsArray, EquipmentTypePair[] equipmentTypePairs)
		{
			if (activeBehaviorsArray == null || activeBehaviorsArray.Length != equipmentTypePairs.Length)
			{
				activeBehaviorsArray = new BaseEquipmentBodyBehavior[equipmentTypePairs.Length];
			}
			for (int i = 0; i < equipmentTypePairs.Length; i++)
			{
				earlyAssignmentBody = body;
				activeBehaviorsArray[i] = (BaseEquipmentBodyBehavior)body.gameObject.AddComponent(equipmentTypePairs[i].behaviorType);
				earlyAssignmentBody = null;
			}
		}

		/*private static void SetEquipmentActive(CharacterBody body, ref BaseEquipmentBodyBehavior behavior, Type behaviorType, bool active)
		{
			if (behavior != active)
			{
				if (active)
                {
					earlyAssignmentBody = body;
					behavior = (BaseEquipmentBodyBehavior)body.gameObject.AddComponent(behaviorType);
					earlyAssignmentBody = null;
				} 
				else
                {
					behavior.state = EquipmentState.empty;
					Destroy(behavior);
					behavior = null;
				}
			}
			if (behavior != null)
			{
				behavior.state = body.inventory.currentEquipmentState;
			}
		}*/

		public struct EquipmentTypePair
        {
			public EquipmentIndex equipmentIndex;
			public Type behaviorType;
			public bool requiresActiveSlot;
		}

		public struct EquipmentBehaviorsState
        {
			public BaseEquipmentBodyBehavior[] activeBehaviorsArray;
			public EquipmentIndex currentEquipmentIndex;
		}

		private struct NetworkContext
		{
			public static ref NetworkContext Current => ref AssetAssociatedBehaviorUtil.GetNetworkContext(ref server, ref client, ref shared);

			public static NetworkContext server;
			public static NetworkContext client;
			public static NetworkContext shared;

			public Dictionary<EquipmentIndex, EquipmentTypePair[]> equipmentTypePairsDict;
			//public EquipmentTypePair[] equipmentTypePairs;
			//public FixedSizeArrayPool<BaseEquipmentBodyBehavior> behaviorArraysPool;

			public void SetEquipmentTypePairs(List<EquipmentTypePair> equipmentTypePairs)
			{
				equipmentTypePairsDict = equipmentTypePairs
					.GroupBy(x => x.equipmentIndex)
					.ToDictionary(x => x.Key, x => x.ToArray());
				//behaviorArraysPool = new FixedSizeArrayPool<BaseEquipmentBodyBehavior>(this.equipmentTypePairs.Length);
			}
		}

		[MeansImplicitUse]
		[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
		public class EquipmentDefDefAssociationAttribute : HG.Reflection.SearchableAttribute
		{
			public bool useOnServer = true;
			public bool useOnClient = true;
		}
	}
}