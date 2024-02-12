﻿using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using BepInEx;
using System.Runtime.CompilerServices;
using System;
using HG.Coroutines;
using HG;

namespace IvyLibrary
{
    /// <summary>
    /// A specialized <see cref="BaseUnityPlugin"/> that generates a <see cref="ContentPack"/> and implements <see cref="IContentPackProvider"/>; comparable to <see cref="RoR2Content"/>.
    /// </summary>
    public abstract class BaseContentPlugin : BaseUnityPlugin, IContentPackProvider
    {
        /// <summary>
        /// A <see cref="ContentPack"/> generated by this plugin.
        /// </summary>
        public ContentPack Content { get; }

        string IContentPackProvider.identifier => Info.Metadata.GUID;

        public delegate IEnumerator LoadStaticContentAsyncDelegate(ContentPack content, LoadStaticContentAsyncArgs args);
        public delegate IEnumerator GenerateContentPackAsyncDelegate(ContentPack content, GetContentPackAsyncArgs args);
        public delegate IEnumerator FinalizeAsyncDelegate(ContentPack content, FinalizeAsyncArgs args);

        /// <summary>
        /// Subscribers are yielded in parallel during <see cref="IContentPackProvider.LoadStaticContentAsync(LoadStaticContentAsyncArgs)"/>.
        /// </summary>
        public event LoadStaticContentAsyncDelegate loadStaticContentAsync;
        /// <summary>
        /// Subscribers are yielded in parallel during <see cref="IContentPackProvider.GenerateContentPackAsync(GetContentPackAsyncArgs)"/>.
        /// </summary>
        public event GenerateContentPackAsyncDelegate generateContentPackAsync;
        /// <summary>
        /// Subscribers are yielded in parallel during <see cref="IContentPackProvider.FinalizeAsync(FinalizeAsyncArgs)"/>.
        /// </summary>
        public event FinalizeAsyncDelegate finalizeAsync;

        public BaseContentPlugin() : base()
        {
            Content = new ContentPack
            {
                identifier = ((IContentPackProvider)this).identifier
            };
            ContentManager.collectContentPackProviders += add => add(this);
        }

        /// <summary>
        /// Implementation of <see cref="IContentPackProvider.LoadStaticContentAsync(LoadStaticContentAsyncArgs)"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation invokes <see cref="loadStaticContentAsync"/> and tracks the results as a <see cref="ParallelProgressCoroutine"/>. This behavior can be overridden.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            if (loadStaticContentAsync != null)
            {
                ParallelProgressCoroutine parallelProgressCoroutine = new ParallelProgressCoroutine(args.progressReceiver);
                foreach (LoadStaticContentAsyncDelegate func in loadStaticContentAsync.GetInvocationList())
                {
                    if (func != null)
                    {
                        ReadableProgress<float> readableProgress = new ReadableProgress<float>();
                        parallelProgressCoroutine.Add(
                            func(Content, new LoadStaticContentAsyncArgs(readableProgress, args.peerLoadInfos)),
                            readableProgress);
                    }
                }
                while (parallelProgressCoroutine.MoveNext())
                {
                    yield return parallelProgressCoroutine.Current;
                }
                loadStaticContentAsync = null;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IContentPackProvider.GenerateContentPackAsync(GetContentPackAsyncArgs)"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation invokes <see cref="generateContentPackAsync"/> and tracks the results as a <see cref="ParallelProgressCoroutine"/>, then outputs <see cref="Content"/>. This behavior can be overridden.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            if (generateContentPackAsync != null)
            {
                ParallelProgressCoroutine parallelProgressCoroutine = new ParallelProgressCoroutine(args.progressReceiver);
                foreach (GenerateContentPackAsyncDelegate func in generateContentPackAsync.GetInvocationList())
                {
                    if (func != null)
                    {
                        ReadableProgress<float> readableProgress = new ReadableProgress<float>();
                        parallelProgressCoroutine.Add(
                            func(Content, new GetContentPackAsyncArgs(readableProgress, args.output, args.peerLoadInfos, args.retriesRemaining)),
                            readableProgress);
                    }
                }
                while (parallelProgressCoroutine.MoveNext())
                {
                    yield return parallelProgressCoroutine.Current;
                }
            }
            ContentPack.Copy(Content, args.output);
        }

        /// <summary>
        /// Implementation of <see cref="IContentPackProvider.FinalizeAsync(FinalizeAsyncArgs)"/>.
        /// </summary>
        /// <remarks>
        /// The default implementation invokes <see cref="finalizeAsync"/> and tracks the results as a <see cref="ParallelProgressCoroutine"/>, then populates the asset ids of networked objects in <see cref="Content"/>. This behavior can be overridden.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            generateContentPackAsync = null;
            if (finalizeAsync != null)
            {
                ParallelProgressCoroutine parallelProgressCoroutine = new ParallelProgressCoroutine(args.progressReceiver);
                foreach (FinalizeAsyncDelegate func in finalizeAsync.GetInvocationList())
                {
                    if (func != null)
                    {
                        ReadableProgress<float> readableProgress = new ReadableProgress<float>();
                        parallelProgressCoroutine.Add(
                            func(Content, new FinalizeAsyncArgs(readableProgress, args.peerLoadInfos, args.finalContentPack)),
                            readableProgress);
                    }
                }
                while (parallelProgressCoroutine.MoveNext())
                {
                    yield return parallelProgressCoroutine.Current;
                }
                finalizeAsync = null;
            }
            Content.PopulateNetworkedObjectAssetIds();
        }

        IEnumerator IContentPackProvider.LoadStaticContentAsync(LoadStaticContentAsyncArgs args) => LoadStaticContentAsync(args);

        IEnumerator IContentPackProvider.GenerateContentPackAsync(GetContentPackAsyncArgs args) => GenerateContentPackAsync(args);

        IEnumerator IContentPackProvider.FinalizeAsync(FinalizeAsyncArgs args) => FinalizeAsync(args);
    }
}