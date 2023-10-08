using System;
using RoR2;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using HG;
using UnityEngine;

namespace Ivyl
{
	public struct RoR2AssetGroup<TObject> : IEquatable<RoR2AssetGroup<TObject>>, IDisposable
	{
		private class LoadAssetsOp
		{
			private static Dictionary<object, object> _opsFromKey;
			private static Dictionary<object, object> _opsFromLocation;

			public static LoadAssetsOp Get(string[] keys, Addressables.MergeMode mergeMode)
            {
				if (_opsFromKey == null)
                {
					_opsFromKey = new Dictionary<object, object>();
                }
				else if (_opsFromKey.TryGetValue((keys, mergeMode), out object value) && value is LoadAssetsOp op)
                {
					op._referenceCount++;
					return op;
                }
				LoadAssetsOp newOp = new LoadAssetsOp
				{
					loadAssetLocationsOp = Addressables.LoadResourceLocationsAsync((IEnumerable)keys, mergeMode, typeof(TObject)),
				};
				newOp.loadAssetLocationsOp.Completed += newOp.OnLoadAssetLocations;
				_opsFromKey.Add((keys, mergeMode), newOp);
				_opsFromKey.Add(newOp, (keys, mergeMode));
				return newOp;
            }

			public static LoadAssetsOp Get(IList<IResourceLocation> assetLocations)
			{
				if (_opsFromLocation == null)
				{
					_opsFromLocation = new Dictionary<object, object>();
				}
				else if (_opsFromLocation.TryGetValue(assetLocations, out object value) && value is LoadAssetsOp op)
				{
					op._referenceCount++;
					return op;
				}
				LoadAssetsOp newOp = new LoadAssetsOp
				{
					assetLocations = assetLocations,
					loadAssetValuesOp = Addressables.LoadAssetsAsync<TObject>(assetLocations, null, false),
				};
				newOp.loadAssetValuesOp.Completed += newOp.OnLoadAssetValues;
				_opsFromLocation.Add(assetLocations, newOp);
				_opsFromLocation.Add(newOp, assetLocations);
				return newOp;
			}

			public static void Return(ref LoadAssetsOp op)
            {
				if (op == null)
                {
					return;
                }
				op._referenceCount--;
				if (op._referenceCount <= 0)
                {
					if (_opsFromKey.TryGetValue(op, out object key))
                    {
						_opsFromKey.Remove(op);
						_opsFromKey.Remove(key);
						if (_opsFromKey.Count <= 0)
                        {
							_opsFromKey = null;
						}
					}
					if (_opsFromLocation.TryGetValue(op, out object location))
					{
						_opsFromLocation.Remove(op);
						_opsFromLocation.Remove(location);
						if (_opsFromLocation.Count <= 0)
						{
							_opsFromLocation = null;
						}
					}
					op.Release();
				}
				op = null;
            }

			private int _referenceCount = 1;
			private bool _loadAssetLocationsCompleted;
			private bool _loadAssetValuesCompleted;
			private ReadOnlyDictionary<string, TObject> _assets;
			private TaskCompletionSource<ReadOnlyDictionary<string, TObject>> _assetsTaskCompletionSource;

			public AsyncOperationHandle<IList<IResourceLocation>> loadAssetLocationsOp;
			public IList<IResourceLocation> assetLocations;
			public AsyncOperationHandle<IList<TObject>> loadAssetValuesOp;

			public Task<ReadOnlyDictionary<string, TObject>> Task
            {
				get
                {
					if (_assetsTaskCompletionSource == null)
					{
						_assetsTaskCompletionSource = new TaskCompletionSource<ReadOnlyDictionary<string, TObject>>();
						if (_assets != null)
						{
							_assetsTaskCompletionSource.SetResult(_assets);
						}
					}
					return _assetsTaskCompletionSource.Task;
				}
            }

			private void OnLoadAssetLocations(AsyncOperationHandle<IList<IResourceLocation>> handle)
			{
				if (_loadAssetLocationsCompleted)
				{
					return;
				}
				_loadAssetLocationsCompleted = true;
				assetLocations = handle.Result;
				_opsFromLocation ??= new Dictionary<object, object>();
				_opsFromLocation.Add(assetLocations, this);
				_opsFromLocation.Add(this, assetLocations);
				loadAssetValuesOp = Addressables.LoadAssetsAsync<TObject>(assetLocations, null, false);
				loadAssetValuesOp.Completed += OnLoadAssetValues;
			}

			private void OnLoadAssetValues(AsyncOperationHandle<IList<TObject>> handle)
			{
				if (_loadAssetValuesCompleted)
				{
					return;
				}
				_loadAssetValuesCompleted = true;
				IList<TObject> assetValues = handle.Result;
				Dictionary<string, TObject> assets = new Dictionary<string, TObject>();
				for (int i = 0; i < assetValues.Count; i++)
				{
					assets[System.IO.Path.GetFileNameWithoutExtension(assetLocations[i].PrimaryKey)] = assetValues[i];
				}
				for (int i = 0; i < assetValues.Count; i++)
				{
					assets[assetLocations[i].PrimaryKey] = assetValues[i];
				}
				_assets = new ReadOnlyDictionary<string, TObject>(assets);
				if (_assetsTaskCompletionSource != null)
				{
					_assetsTaskCompletionSource.SetResult(_assets);
				}
			}

			public ReadOnlyDictionary<string, TObject> WaitForAssets()
            {
				if (loadAssetLocationsOp.IsValid() && !_loadAssetLocationsCompleted)
				{
					loadAssetLocationsOp.WaitForCompletion();
				}
				if (loadAssetValuesOp.IsValid() && !_loadAssetValuesCompleted)
				{
					loadAssetValuesOp.WaitForCompletion();
				}
				return _assets;
			}

			private void Release()
            {
				if (loadAssetLocationsOp.IsValid())
				{
					loadAssetLocationsOp.Completed -= OnLoadAssetLocations;
					loadAssetLocationsOp.Release();
					loadAssetLocationsOp = default;
				}
				assetLocations = null;
				if (loadAssetValuesOp.IsValid())
				{
					loadAssetValuesOp.Completed -= OnLoadAssetValues;
					loadAssetValuesOp.Release();
					loadAssetValuesOp = default;
				}
				_assetsTaskCompletionSource = null;
				_assets = null;
			}
		}

		private bool _disposed;
		private LoadAssetsOp _internalOp;

		public ReadOnlyDictionary<string, TObject> Assets
		{
			get 
			{
				if (_disposed)
				{
					throw new ObjectDisposedException(ToString());
				}
				return _internalOp?.WaitForAssets();
			}
		}

		public TObject this[string key] => Assets[key];

		public RoR2AssetGroup(params string[] keys) : this(Addressables.MergeMode.Union, keys) 
		{

		}

		public RoR2AssetGroup(Addressables.MergeMode mergeMode, params string[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (LegacyResourcesAPI.oldResourcesPathToGuid.TryGetValue(keys[i], out string guid))
				{
					keys[i] = guid;
				}
			}
			_disposed = false;
			_internalOp = LoadAssetsOp.Get(keys, mergeMode);
		}

		public RoR2AssetGroup(IList<IResourceLocation> assetLocations)
		{
			_disposed = false;
			_internalOp = LoadAssetsOp.Get(assetLocations);
		}

		public bool Equals(RoR2AssetGroup<TObject> other) => _internalOp == other._internalOp;

		public override bool Equals(object other)
		{
			return other is RoR2AssetGroup<TObject> asset && Equals(asset);
		}

		public override int GetHashCode() => _internalOp.GetHashCode();

		public bool IsValid() => _internalOp != null;

		public TaskAwaiter<ReadOnlyDictionary<string, TObject>> GetAwaiter()
        {
			if (_disposed)
            {
				throw new ObjectDisposedException(ToString());
            }
			return _internalOp != null ? _internalOp.Task.GetAwaiter() : default;
        }

        public void Dispose()
        {
			if (!_disposed)
			{
				LoadAssetsOp.Return(ref _internalOp);
				_disposed = true;
			}
        }
	}
}