using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace IvyLibrary
{
    public class AssetBundleRequest<T> : IEnumerator, IAsyncOperation where T : UnityEngine.Object
    {
        private AssetBundleRequest _internalRequest;
        private T _asset;
        private T[] _allAssets;

        public AssetBundleRequest(AssetBundleRequest request)
        {
            _internalRequest = request;
        }

        public static explicit operator AssetBundleRequest(AssetBundleRequest<T> obj) => obj._internalRequest;
        public static implicit operator AsyncOperationHandle(AssetBundleRequest<T> obj) => new AsyncOperationHandle(obj);

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

        void IEnumerator.Reset() => throw new NotSupportedException();
        #region IAsyncOperation

        event Action<AsyncOperationHandle> IAsyncOperation.CompletedTypeless
        {
            add => completedTypeless += _ => value(new AsyncOperationHandle(this));
            remove => completedTypeless -= _ => value(new AsyncOperationHandle(this));
        }

        event Action<AsyncOperationHandle> IAsyncOperation.Destroyed
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        Type IAsyncOperation.ResultType => typeof(T);

        int IAsyncOperation.Version => 0;

        string IAsyncOperation.DebugName => throw new NotImplementedException();

        int IAsyncOperation.ReferenceCount => throw new NotSupportedException();

        float IAsyncOperation.PercentComplete => progress;

        AsyncOperationStatus IAsyncOperation.Status => isDone ? (asset != null ? AsyncOperationStatus.Succeeded : AsyncOperationStatus.Failed) : AsyncOperationStatus.None;

        Exception IAsyncOperation.OperationException => throw new NotSupportedException();

        bool IAsyncOperation.IsDone => isDone;

        Action<IAsyncOperation> IAsyncOperation.OnDestroy { set => throw new NotSupportedException(); }

        bool IAsyncOperation.IsRunning => !isDone;

        Task<object> IAsyncOperation.Task => throw new NotSupportedException();

        AsyncOperationHandle IAsyncOperation.Handle => new AsyncOperationHandle(this);

        object IAsyncOperation.GetResultAsObject() => asset;

        void IAsyncOperation.DecrementReferenceCount() => throw new NotSupportedException();

        void IAsyncOperation.IncrementReferenceCount() => throw new NotSupportedException();

        DownloadStatus IAsyncOperation.GetDownloadStatus(HashSet<object> visited) => throw new NotSupportedException();

        void IAsyncOperation.GetDependencies(List<AsyncOperationHandle> deps) => throw new NotSupportedException();

        void IAsyncOperation.InvokeCompletionEvent() => throw new NotSupportedException();

        void IAsyncOperation.Start(ResourceManager rm, AsyncOperationHandle dependency, DelegateList<float> updateCallbacks) => throw new NotSupportedException();

        void IAsyncOperation.WaitForCompletion() => throw new NotSupportedException();
        #endregion
    }
}