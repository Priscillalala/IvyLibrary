using System;
using RoR2;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Ivyl
{
	public struct RoR2Asset<TObject> : IEquatable<RoR2Asset<TObject>>, IDisposable
	{
		private bool _disposed;
		private AsyncOperationHandle<TObject> _op;

		public string Key { get; }

		public TObject Value 
		{
			get 
			{
				if (_disposed)
                {
					throw new ObjectDisposedException(ToString());
                }
				if (!_op.IsValid())
                {
					return default;
                }
				if (!_op.IsDone)
                {
					_op.WaitForCompletion();
                }
				return _op.Result;
			}
		}

		public RoR2Asset(string key)
		{
			if (LegacyResourcesAPI.oldResourcesPathToGuid.TryGetValue(key, out string guid))
			{
				key = guid;
			}
			Key = key;
			_disposed = false;
			_op = Addressables.LoadAssetAsync<TObject>(Key);
		}

		public RoR2Asset(IResourceLocation assetLocation)
		{
			Key = assetLocation.PrimaryKey;
			_disposed = false;
			_op = Addressables.LoadAssetAsync<TObject>(assetLocation);
		}

		public bool Equals(RoR2Asset<TObject> other) => Key == other.Key;

		public override bool Equals(object other)
		{
			return other is RoR2Asset<TObject> asset && Equals(asset);
		}

		public override int GetHashCode() => Key.GetHashCode();

		public override string ToString() => Key.ToString();

		public bool IsValid() => _op.IsValid();

		public TaskAwaiter<TObject> GetAwaiter()
        {
			if (_disposed)
            {
				throw new ObjectDisposedException(ToString());
            }
			return _op.Task.GetAwaiter();
        }

        public void Dispose()
        {
			if (!_disposed)
			{
				if (_op.IsValid())
				{
					_op.Release();
					_op = default;
				}
				_disposed = true;
			}
        }

		public static implicit operator RoR2Asset<TObject>(string value)
		{
			return new RoR2Asset<TObject>(value);
		}
		
		public static explicit operator TObject(RoR2Asset<TObject> value)
		{
			return value.Value;
		}
	}
}
