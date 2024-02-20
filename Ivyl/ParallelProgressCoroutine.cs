using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace IvyLibrary
{
    public class ParallelProgressCoroutine : IEnumerator<float>
    {
        private List<IEnumerator<float>> coroutinesList = new List<IEnumerator<float>>();
        private readonly IEnumerator internalCoroutine;
        private float maxProgress;
        private int completedSubCoroutinesCount;
        public float Progress { get; private set; }

        public ParallelProgressCoroutine()
        {
            internalCoroutine = InternalCoroutine();
        }

        private IEnumerator InternalCoroutine()
        {
            Debug.Log("InternalCoroutine on the move");
            while (coroutinesList.Count > 0) 
            {
                float currentProgress = 0;
                for (int i = coroutinesList.Count - 1; i >= 0; i--)
                {
                    IEnumerator<float> coroutine = coroutinesList[i];
                    Debug.Log("InternalCoroutine sub routine on the move");
                    if (coroutine.MoveNext())
                    {
                        currentProgress += Mathf.Clamp01(coroutine.Current);
                        yield return ((IEnumerator)coroutine).Current;
                    }
                    else
                    {
                        completedSubCoroutinesCount++;
                        coroutinesList.RemoveAt(i);
                    }
                }
                Progress = (completedSubCoroutinesCount + currentProgress) / maxProgress;
            }
            Progress = 1f;
            coroutinesList = null;
        }

        public bool MoveNext() => internalCoroutine.MoveNext();

        public void Add(IEnumerator<float> coroutineWithProgress)
        {
            if (coroutinesList == null)
            {
                throw new InvalidOperationException();
            }
            maxProgress += 1f;
            coroutinesList.Add(coroutineWithProgress);
        }

        object IEnumerator.Current => internalCoroutine.Current;

        float IEnumerator<float>.Current => Progress;

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose() { }
    }
}