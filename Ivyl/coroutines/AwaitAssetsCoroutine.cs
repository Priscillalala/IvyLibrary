using RoR2.ContentManagement;
using System.Collections;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;
using HG;

namespace IvyLibrary
{
    public class AwaitAssetsCoroutine : IEnumerator<float>, IEnumerable
    {
        private List<BaseLoadOperationWrapper> loadOperations = new List<BaseLoadOperationWrapper>();
        private readonly IEnumerator internalCoroutine;
        private float totalWeight;
        private float completedWeight;
        public float Progress { get; private set; }

        public AwaitAssetsCoroutine()
        {
            internalCoroutine = InternalCoroutine();
        }

        private IEnumerator InternalCoroutine()
        {
            while (loadOperations.Count > 0)
            {
                float currentProgressWeight = 0f;
                for (int i = loadOperations.Count - 1; i >= 0; i--)
                {
                    BaseLoadOperationWrapper loadOperation = loadOperations[i];
                    if (loadOperation.IsDone)
                    {
                        completedWeight += loadOperation.weight;
                        loadOperations.RemoveAt(i);
                    }
                    else
                    {
                        currentProgressWeight += loadOperation.weight * Mathf.Clamp01(loadOperation.Progress);
                        yield return null;
                    }
                }
                Progress = (completedWeight + currentProgressWeight) / totalWeight;
            }
            Progress = 1f;
            loadOperations = null;
        }

        public bool MoveNext() => internalCoroutine.MoveNext();

        public void Add(AsyncOperation loadOperation, float weight = 1f) => Add(new UnityAsyncOperationWrapper
        {
            asyncOperation = loadOperation,
            weight = weight,
        });

        public void Add(AsyncOperationHandle loadOperation, float weight = 1f) => Add(new ResourcesAsyncOperationWrapper
        {
            asyncOperation = loadOperation.InternalOp,
            weight = weight,
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(BaseLoadOperationWrapper loadOperation)
        {
            if (loadOperations == null)
            {
                throw new InvalidOperationException();
            }
            loadOperations.Add(loadOperation);
            totalWeight += loadOperation.weight;
        }

        #region AsyncOperationHandle
        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
        }
        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
        }
        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
        }
        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
            Add(loadOperations.Item9, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
            Add(loadOperations.Item9, weightPer);
            Add(loadOperations.Item10, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
            Add(loadOperations.Item9, weightPer);
            Add(loadOperations.Item10, weightPer);
            Add(loadOperations.Item11, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
            Add(loadOperations.Item9, weightPer);
            Add(loadOperations.Item10, weightPer);
            Add(loadOperations.Item11, weightPer);
            Add(loadOperations.Item12, weightPer);
        }

        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
            Add(loadOperations.Item9, weightPer);
            Add(loadOperations.Item10, weightPer);
            Add(loadOperations.Item11, weightPer);
            Add(loadOperations.Item12, weightPer);
            Add(loadOperations.Item13, weightPer);
        }
        public void Add(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle,
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
            Add(loadOperations.Item8, weightPer);
            Add(loadOperations.Item9, weightPer);
            Add(loadOperations.Item10, weightPer);
            Add(loadOperations.Item11, weightPer);
            Add(loadOperations.Item12, weightPer);
            Add(loadOperations.Item13, weightPer);
            Add(loadOperations.Item14, weightPer);
        }

        public void Add(float weightPer, params AsyncOperationHandle[] loadOperations)
        {
            for (int i = 0; i < loadOperations.Length; i++)
            {
                Add(loadOperations[i], weightPer);
            }
        }

        public void Add(params AsyncOperationHandle[] loadOperations) => Add(1f, loadOperations);
        #endregion

        #region AsyncOperation
        public void Add(ValueTuple<AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
        }

        public void Add(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
        }
        public void Add(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
        }
        public void Add(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
        }
        public void Add(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
        }

        public void Add(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            Add(loadOperations.Item1, weightPer);
            Add(loadOperations.Item2, weightPer);
            Add(loadOperations.Item3, weightPer);
            Add(loadOperations.Item4, weightPer);
            Add(loadOperations.Item5, weightPer);
            Add(loadOperations.Item6, weightPer);
            Add(loadOperations.Item7, weightPer);
        }

        public void Add(float weightPer, params AsyncOperation[] loadOperations)
        {
            for (int i = 0; i < loadOperations.Length; i++)
            {
                Add(loadOperations[i], weightPer);
            }
        }

        public void Add(params AsyncOperation[] loadOperations) => Add(1f, loadOperations);
        #endregion

        public abstract class BaseLoadOperationWrapper
        {
            public abstract float Progress { get; }
            public abstract bool IsDone { get; }

            public float weight;
        }

        public class UnityAsyncOperationWrapper : BaseLoadOperationWrapper
        {
            public override float Progress => asyncOperation.progress;
            public override bool IsDone => asyncOperation.isDone;

            public AsyncOperation asyncOperation;
        }

        public class ResourcesAsyncOperationWrapper : BaseLoadOperationWrapper
        {
            public override float Progress => asyncOperation.PercentComplete;
            public override bool IsDone => asyncOperation.IsDone;

            public IAsyncOperation asyncOperation;
        }

        object IEnumerator.Current => internalCoroutine.Current;

        float IEnumerator<float>.Current => Progress;

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose() { }

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}