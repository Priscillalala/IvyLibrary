using RoR2.ContentManagement;
using System.Collections;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using System.Collections.Generic;

namespace IvyLibrary
{
    public class ModLoadHelper : IEnumerator
    {
        private readonly List<BaseLoadOperation> loadOperations = new List<BaseLoadOperation>();
        private readonly List<GenericOperation> genericOperations = new List<GenericOperation>();
        public readonly IEnumerator coroutine;
        private IProgress<float> progressReceiver;
        private float totalWeight;
        private float completedWeight;

        public float progress { get; private set; } 

        public ModLoadHelper()
        {
            coroutine = Coroutine();
        }

        public ModLoadHelper(IProgress<float> progressReceiver) : this()
        {
            this.progressReceiver = progressReceiver;
        }

        public ModLoadHelper(LoadStaticContentAsyncArgs args) : this(args.progressReceiver) { }

        public ModLoadHelper(GetContentPackAsyncArgs args) : this(args.progressReceiver) { }

        public ModLoadHelper(FinalizeAsyncArgs args) : this(args.progressReceiver) { }

        private IEnumerator Coroutine()
        {
            while (loadOperations.Count > 0)
            {
                float currentProgressWeight = 0f;
                for (int i = loadOperations.Count - 1; i >= 0; i--)
                {
                    BaseLoadOperation loadOperation = loadOperations[i];
                    if (loadOperation.IsDone)
                    {
                        completedWeight += loadOperation.weight;
                        loadOperations.RemoveAt(i);
                    }
                    else
                    {
                        currentProgressWeight += loadOperation.weight * loadOperation.Progress;
                        yield return null;
                    }
                }
                SetProgress((completedWeight + currentProgressWeight) / totalWeight);
            }
            foreach (GenericOperation genericOperation in genericOperations)
            {
                IEnumerator operation = genericOperation.getOperation();
                while (operation.MoveNext())
                {
                    yield return operation.Current;
                }
                SetProgress((completedWeight += genericOperation.weight) / totalWeight);
            }

            void SetProgress(float newProgress)
            {
                if (progress != newProgress)
                {
                    progress = newProgress;
                    progressReceiver?.Report(newProgress);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLoadOperation(IEnumerator loadOperation, float weight = 1f) => AddLoadOperation(new CoroutineWrapper
        {
            coroutine = loadOperation,
            weight = weight,
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLoadOperation(AsyncOperation loadOperation, float weight = 1f) => AddLoadOperation(new UnityAsyncOperationWrapper
        {
            asyncOperation = loadOperation,
            weight = weight,
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLoadOperation(AsyncOperationHandle loadOperation, float weight = 1f) => AddLoadOperation(new ResourcesAsyncOperationWrapper
        {
            asyncOperation = loadOperation.InternalOp,
            weight = weight,
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLoadOperation(BaseLoadOperation loadOperation)
        {
            loadOperations.Add(loadOperation);
            totalWeight += loadOperation.weight;
        }

        #region IEnumerator
        public void AddLoadOperations(ValueTuple<IEnumerator, IEnumerator> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
        }

        public void AddLoadOperations(ValueTuple<IEnumerator, IEnumerator, IEnumerator> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
        }

        public void AddLoadOperations(ValueTuple<IEnumerator, IEnumerator, IEnumerator, IEnumerator> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
        }

        public void AddLoadOperations(ValueTuple<IEnumerator, IEnumerator, IEnumerator, IEnumerator, IEnumerator> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
        }

        public void AddLoadOperations(ValueTuple<IEnumerator, IEnumerator, IEnumerator, IEnumerator, IEnumerator, IEnumerator> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
        }

        public void AddLoadOperations(ValueTuple<IEnumerator, IEnumerator, IEnumerator, IEnumerator, IEnumerator, IEnumerator, IEnumerator> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
        }

        public void AddLoadOperations(float weightPer, params IEnumerator[] loadOperations)
        {
            for (int i = 0; i < loadOperations.Length; i++)
            {
                AddLoadOperation(loadOperations[i], weightPer);
            }
        }

        public void AddLoadOperations(params IEnumerator[] loadOperations) => AddLoadOperations(1f, loadOperations);
        #endregion

        #region AsyncOperationHandle
        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
            AddLoadOperation(loadOperations.Item9, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
            AddLoadOperation(loadOperations.Item9, weightPer);
            AddLoadOperation(loadOperations.Item10, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
            AddLoadOperation(loadOperations.Item9, weightPer);
            AddLoadOperation(loadOperations.Item10, weightPer);
            AddLoadOperation(loadOperations.Item11, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
            AddLoadOperation(loadOperations.Item9, weightPer);
            AddLoadOperation(loadOperations.Item10, weightPer);
            AddLoadOperation(loadOperations.Item11, weightPer);
            AddLoadOperation(loadOperations.Item12, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
            AddLoadOperation(loadOperations.Item9, weightPer);
            AddLoadOperation(loadOperations.Item10, weightPer);
            AddLoadOperation(loadOperations.Item11, weightPer);
            AddLoadOperation(loadOperations.Item12, weightPer);
            AddLoadOperation(loadOperations.Item13, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, 
            ValueTuple<AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle, AsyncOperationHandle>> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
            AddLoadOperation(loadOperations.Item8, weightPer);
            AddLoadOperation(loadOperations.Item9, weightPer);
            AddLoadOperation(loadOperations.Item10, weightPer);
            AddLoadOperation(loadOperations.Item11, weightPer);
            AddLoadOperation(loadOperations.Item12, weightPer);
            AddLoadOperation(loadOperations.Item13, weightPer);
            AddLoadOperation(loadOperations.Item14, weightPer);
        }

        public void AddLoadOperations(float weightPer, params AsyncOperationHandle[] loadOperations)
        {
            for (int i = 0; i < loadOperations.Length; i++)
            {
                AddLoadOperation(loadOperations[i], weightPer);
            }
        }

        public void AddLoadOperations(params AsyncOperationHandle[] loadOperations) => AddLoadOperations(1f, loadOperations);
        #endregion

        #region AsyncOperation
        public void AddLoadOperations(ValueTuple<AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
        }
        public void AddLoadOperations(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
        }

        public void AddLoadOperations(ValueTuple<AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation, AsyncOperation> loadOperations, float weightPer = 1f)
        {
            AddLoadOperation(loadOperations.Item1, weightPer);
            AddLoadOperation(loadOperations.Item2, weightPer);
            AddLoadOperation(loadOperations.Item3, weightPer);
            AddLoadOperation(loadOperations.Item4, weightPer);
            AddLoadOperation(loadOperations.Item5, weightPer);
            AddLoadOperation(loadOperations.Item6, weightPer);
            AddLoadOperation(loadOperations.Item7, weightPer);
        }

        public void AddLoadOperations(float weightPer, params AsyncOperation[] loadOperations)
        {
            for (int i = 0; i < loadOperations.Length; i++)
            {
                AddLoadOperation(loadOperations[i], weightPer);
            }
        }

        public void AddLoadOperations(params AsyncOperation[] loadOperations) => AddLoadOperations(1f, loadOperations);
        #endregion

        public void AddGenericOperation(Func<IEnumerator> operation, float weight = 0.1f)
        {
            genericOperations.Add(new GenericOperation
            {
                getOperation = operation,
                weight = weight,
            });
            totalWeight += weight;
        }

        public void AddGenericOperation(Action operation, float weight = 0.05f)
        {
            IEnumerator Coroutine()
            {
                operation();
                yield break;
            }
            AddGenericOperation(Coroutine, weight);
        }

        public abstract class BaseLoadOperation
        {
            public abstract float Progress { get; }
            public abstract bool IsDone { get; }

            public float weight;
        }

        public class CoroutineWrapper : BaseLoadOperation
        {
            public override float Progress => 0;
            public override bool IsDone => !coroutine.MoveNext();

            public IEnumerator coroutine;
        }

        public class UnityAsyncOperationWrapper : BaseLoadOperation
        {
            public override float Progress => asyncOperation.progress;
            public override bool IsDone => asyncOperation.isDone;

            public AsyncOperation asyncOperation;
        }

        public class ResourcesAsyncOperationWrapper : BaseLoadOperation
        {
            public override float Progress => asyncOperation.PercentComplete;
            public override bool IsDone => asyncOperation.IsDone;

            public IAsyncOperation asyncOperation;
        }

        public struct GenericOperation
        {
            public Func<IEnumerator> getOperation;
            public float weight;
        }

        object IEnumerator.Current => coroutine.Current;

        bool IEnumerator.MoveNext() => coroutine.MoveNext();

        void IEnumerator.Reset() => throw new NotSupportedException();
    }
}