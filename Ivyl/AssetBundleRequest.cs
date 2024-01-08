using System;
using System.Collections;
using UnityEngine;

namespace IvyLibrary
{
    public class AssetBundleRequest<T> : IEnumerator where T : UnityEngine.Object
    {
        private AssetBundleRequest _internalRequest;
        private T _asset;
        private T[] _allAssets;

        public AssetBundleRequest(AssetBundleRequest request)
        {
            _internalRequest = request;
        }

        public static implicit operator AssetBundleRequest(AssetBundleRequest<T> obj) => obj._internalRequest;

        public T asset => _asset ??= (T)_internalRequest.asset;
        public T[] allAssets => _allAssets ??= Array.ConvertAll(_internalRequest.allAssets, x => (T)x);
        public bool isDone => _internalRequest.isDone;
        public float progress => _internalRequest.progress;
        public int priority 
        {
            get => _internalRequest.priority;
            set => _internalRequest.priority = value;
        }
        public bool allowSceneActivation
        {
            get => _internalRequest.allowSceneActivation;
            set => _internalRequest.allowSceneActivation = value;
        }
        
        public event Action<AssetBundleRequest<T>> completed
        {
            add => _internalRequest.completed += _ => value(this);
            remove => _internalRequest.completed -= _ => value(this);
        }

        public event Action<AssetBundleRequest> completedTypeless
        {
            add => _internalRequest.completed += _ => value(_internalRequest);
            remove => _internalRequest.completed -= _ => value(_internalRequest);
        }

        public override bool Equals(object obj) => (obj is AssetBundleRequest request && _internalRequest == request) || (obj is AssetBundleRequest<T> other && _internalRequest == other._internalRequest);

        public override int GetHashCode() => _internalRequest.GetHashCode();

        object IEnumerator.Current => asset;

        bool IEnumerator.MoveNext() => !_internalRequest.isDone;

        void IEnumerator.Reset() { }
    }
}