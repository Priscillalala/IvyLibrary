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
	public abstract class BaseItemMasterBehavior : MonoBehaviour
	{
		private static CharacterMaster earlyAssignmentMaster = null;
		private static Dictionary<UnityObjectWrapperKey<CharacterMaster>, BaseItemMasterBehavior[]> masterToItemBehaviors;

		public CharacterMaster master { get; private set; }

		public int stack;

		protected void Awake()
		{
			master = earlyAssignmentMaster;
			earlyAssignmentMaster = null;
		}

		[SystemInitializer(typeof(ItemCatalog))]
		private static void Init()
		{
			List<BaseItemBodyBehavior.ItemTypePair> server = new List<BaseItemBodyBehavior.ItemTypePair>();
			List<BaseItemBodyBehavior.ItemTypePair> client = new List<BaseItemBodyBehavior.ItemTypePair>();
			List<BaseItemBodyBehavior.ItemTypePair> shared = new List<BaseItemBodyBehavior.ItemTypePair>();

			void RegisterBehaviour(ItemDefAssociationAttribute attribute, MethodInfo methodInfo, ItemDef asset)
            {
				if (asset.itemIndex < 0)
				{
					Debug.LogError($"{methodInfo.DeclaringType.FullName}.{methodInfo.Name} returned an ItemDef that's not registered in the ItemCatalog. result={asset}");
				}
				else
				{
					BaseItemBodyBehavior.ItemTypePair itemTypePair = new BaseItemBodyBehavior.ItemTypePair
					{
						itemIndex = asset.itemIndex,
						behaviorType = methodInfo.DeclaringType
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

			AssetAssociatedBehaviorUtil.CommenceAttributeSearch<ItemDefAssociationAttribute, ItemDef>(typeof(BaseItemMasterBehavior), RegisterBehaviour);

			if (shared.Count <= 0)
            {
				return;
            }

			NetworkContext.server.SetItemTypePairs(server);
			NetworkContext.client.SetItemTypePairs(client);
			NetworkContext.shared.SetItemTypePairs(shared);
			masterToItemBehaviors = new Dictionary<UnityObjectWrapperKey<CharacterMaster>, BaseItemMasterBehavior[]>();

			//On.RoR2.CharacterMaster.Awake += CharacterMaster_Awake;
			On.RoR2.CharacterMaster.OnDestroy += CharacterMaster_OnDestroy;
			On.RoR2.CharacterMaster.OnInventoryChanged += CharacterMaster_OnInventoryChanged;
		}

		/*private static void CharacterMaster_Awake(On.RoR2.CharacterMaster.orig_Awake orig, CharacterMaster self)
		{
			BaseItemMasterBehavior[] value = NetworkContext.Current.behaviorArraysPool.Request();
			masterToItemBehaviors.Add(self, value);
			orig(self);
		}*/

		private static void CharacterMaster_OnDestroy(On.RoR2.CharacterMaster.orig_OnDestroy orig, CharacterMaster self)
		{
			orig(self);
			if (masterToItemBehaviors.TryGetValue(self, out BaseItemMasterBehavior[] array)) 
			{
				for (int i = 0; i < array.Length; i++)
				{
					Destroy(array[i]);
				}
				masterToItemBehaviors.Remove(self);
				if (NetworkServer.active || NetworkClient.active)
				{
					NetworkContext.Current.behaviorArraysPool.Return(array);
				}
			}
		}

		private static void CharacterMaster_OnInventoryChanged(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
		{
			orig(self);
			UpdateMasterItemBehaviorStacks(self);
		}

		private static void UpdateMasterItemBehaviorStacks(CharacterMaster master)
		{
			ref NetworkContext currentNetworkContext = ref NetworkContext.Current;
			Inventory inventory = master.inventory;
			if (!masterToItemBehaviors.TryGetValue(master, out BaseItemMasterBehavior[] array) && inventory && inventory.itemAcquisitionOrder.Count > 0)
            {
				array = currentNetworkContext.behaviorArraysPool.Request();
				masterToItemBehaviors.Add(master, array);
			}
			if (array == null)
            {
				return;
            }
			BaseItemBodyBehavior.ItemTypePair[] itemTypePairs = currentNetworkContext.itemTypePairs;
			if (inventory)
			{
				for (int i = 0; i < itemTypePairs.Length; i++)
				{
					BaseItemBodyBehavior.ItemTypePair itemTypePair = itemTypePairs[i];
					ref BaseItemMasterBehavior behavior = ref array[i];
					SetItemStack(master, ref behavior, itemTypePair.behaviorType, inventory.GetItemCount(itemTypePair.itemIndex));
				}
				return;
			}
			for (int j = 0; j < itemTypePairs.Length; j++)
			{
				ref BaseItemMasterBehavior ptr = ref array[j];
				if (ptr != null)
				{
					Destroy(ptr);
					ptr = null;
				}
			}
		}

		private static void SetItemStack(CharacterMaster master, ref BaseItemMasterBehavior behavior, Type behaviorType, int stack)
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
					earlyAssignmentMaster = master;
					behavior = (BaseItemMasterBehavior)master.gameObject.AddComponent(behaviorType);
					earlyAssignmentMaster = null;
				}
			}
			if (behavior != null)
			{
				behavior.stack = stack;
			}
		}

		private struct NetworkContext
		{
			public static ref NetworkContext Current => ref AssetAssociatedBehaviorUtil.GetNetworkContext(ref server, ref client, ref shared);

			public static NetworkContext server;
			public static NetworkContext client;
			public static NetworkContext shared;

			public BaseItemBodyBehavior.ItemTypePair[] itemTypePairs;
			public FixedSizeArrayPool<BaseItemMasterBehavior> behaviorArraysPool;

			public void SetItemTypePairs(List<BaseItemBodyBehavior.ItemTypePair> itemTypePairs)
			{
				this.itemTypePairs = itemTypePairs.ToArray();
				behaviorArraysPool = new FixedSizeArrayPool<BaseItemMasterBehavior>(this.itemTypePairs.Length);
			}
		}

		[MeansImplicitUse]
		[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
		public class ItemDefAssociationAttribute : HG.Reflection.SearchableAttribute
		{
			public bool useOnServer = true;

			public bool useOnClient = true;
		}
	}
}