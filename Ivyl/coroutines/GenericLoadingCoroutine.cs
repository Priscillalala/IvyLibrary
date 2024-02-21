using System.Collections;
using System.Runtime.CompilerServices;
using System;
using UnityEngine;
using System.Collections.Generic;
using HG;

namespace IvyLibrary
{
    /// <summary>
    /// Combine delegates and coroutines to create a generic mod loading sequence.
    /// </summary>
    public class GenericLoadingCoroutine : IEnumerator<float>, IEnumerable
    {
        private List<BaseOperationWrapper> operations = new List<BaseOperationWrapper>();
        private readonly IEnumerator internalCoroutine;
        private float totalWeight;
        private float completedWeight;
        public float Progress { get; private set; }

        public GenericLoadingCoroutine()
        {
            internalCoroutine = InternalCoroutine();
        }

        private IEnumerator InternalCoroutine()
        {
            for (int i = 0; i < operations.Count; i++)
            {
                BaseOperationWrapper operation = operations[i];
                IEnumerator coroutine = operation.Execute();
                if (coroutine != null)
                {
                    while (coroutine.MoveNext())
                    {
                        Progress = (completedWeight + (operation.weight * Mathf.Clamp01(operation.Progress))) / totalWeight;
                        yield return coroutine.Current;
                    }
                }
                Progress = (completedWeight += operation.weight) / totalWeight;
            }
            Progress = 1f;
            operations = null;
        }

        public bool MoveNext() => internalCoroutine.MoveNext();

        public void Add(Func<IEnumerator<float>> coroutineWithProgressMethod, float weight = 1f) => Add(new DelayedCoroutineWithProgressWrapper
        {
            coroutineMethod = coroutineWithProgressMethod,
            weight = weight,
        });

        public void Add(IEnumerator<float> coroutineWithProgress, float weight = 1f) => Add(new CoroutineWithProgressWrapper
        {
            coroutine = coroutineWithProgress,
            weight = weight,
        });

        public void Add(Func<IEnumerator> coroutineMethod, float weight = 1f) => Add(new DelayedCoroutineWrapper
        {
            coroutineMethod = coroutineMethod,
            weight = weight,
        });

        public void Add(IEnumerator coroutine, float weight = 1f) => Add(new CoroutineWrapper
        {
            coroutine = coroutine,
            weight = weight,
        });

        public void Add(Func<IEnumerator> coroutineMethod, ReadableProgress<float> coroutineProgressReceiver, float weight = 1f) => Add(new DelayedCoroutineWithProgressRecieverWrapper
        {
            coroutineMethod = coroutineMethod,
            weight = weight,
            coroutineProgressReciever = coroutineProgressReceiver,
        });

        public void Add(IEnumerator coroutine, ReadableProgress<float> coroutineProgressReceiver, float weight = 1f) => Add(new CoroutineWithProgressRecieverWrapper
        {
            coroutine = coroutine,
            weight = weight,
            coroutineProgressReciever = coroutineProgressReceiver,
        });

        public void Add(Action action, float weight = 0.05f) => Add(new ActionWrapper
        {
            action = action,
            weight = weight,
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(BaseOperationWrapper operation)
        {
            if (operations == null)
            {
                throw new InvalidOperationException();
            }
            operations.Add(operation);
            totalWeight += operation.weight;
        }

        public abstract class BaseOperationWrapper
        {
            public abstract float Progress { get; }

            public float weight;

            public abstract IEnumerator Execute();
        }

        public class DelayedCoroutineWithProgressWrapper : CoroutineWithProgressWrapper
        {
            public Func<IEnumerator<float>> coroutineMethod;

            public override IEnumerator Execute() => coroutine = coroutineMethod();
        }

        public class CoroutineWithProgressWrapper : BaseOperationWrapper
        {
            public override float Progress => coroutine.Current;

            public IEnumerator<float> coroutine;

            public override IEnumerator Execute() => coroutine;
        }

        public class DelayedCoroutineWrapper : CoroutineWrapper
        {
            public Func<IEnumerator> coroutineMethod;

            public override IEnumerator Execute() => coroutine = coroutineMethod();
        }

        public class CoroutineWrapper : BaseOperationWrapper
        {
            public override float Progress => 0f;

            public IEnumerator coroutine;

            public override IEnumerator Execute() => coroutine;
        }

        public class DelayedCoroutineWithProgressRecieverWrapper : DelayedCoroutineWrapper
        {
            public override float Progress => coroutineProgressReciever.value;

            public ReadableProgress<float> coroutineProgressReciever;
        }

        public class CoroutineWithProgressRecieverWrapper : CoroutineWrapper
        {
            public override float Progress => coroutineProgressReciever.value;

            public ReadableProgress<float> coroutineProgressReciever;
        }

        public class ActionWrapper : BaseOperationWrapper
        {
            public override float Progress => 0f;

            public Action action;

            public override IEnumerator Execute()
            {
                action();
                return null;
            }
        }

        object IEnumerator.Current => internalCoroutine.Current;

        float IEnumerator<float>.Current => Progress;

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose() { }

        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}