using RoR2.ContentManagement;
using System.Collections;
using BepInEx;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    public abstract class BaseContentPlugin : BaseUnityPlugin, IContentPackProvider
    {
        public ContentPack Content { get; }

        public string identifier => Info.Metadata.GUID;

        public delegate IEnumerator LoadStaticContentAsyncDelegate(LoadStaticContentAsyncArgs args);
        public delegate IEnumerator GenerateContentPackAsyncDelegate(GetContentPackAsyncArgs args);
        public delegate IEnumerator FinalizeAsyncDelegate(FinalizeAsyncArgs args);

        public event LoadStaticContentAsyncDelegate loadStaticContentAsync;
        public event GenerateContentPackAsyncDelegate generateContentPackAsync;
        public event FinalizeAsyncDelegate finalizeAsync;

        protected BaseContentPlugin() : base()
        {
            Content = new ContentPack
            {
                identifier = identifier
            };
            ContentManager.collectContentPackProviders += add => add(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerator LoadStaticContentAsyncImpl(LoadStaticContentAsyncArgs args)
        {
            if (loadStaticContentAsync != null)
            {
                foreach (LoadStaticContentAsyncDelegate func in loadStaticContentAsync.GetInvocationList())
                {
                    yield return func(args);
                }
                loadStaticContentAsync = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerator GenerateContentPackAsyncImpl(GetContentPackAsyncArgs args)
        {
            if (generateContentPackAsync != null)
            {
                foreach (GenerateContentPackAsyncDelegate func in generateContentPackAsync.GetInvocationList())
                {
                    yield return func(args);
                }
                generateContentPackAsync = null;
            }
            ContentPack.Copy(Content, args.output);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual IEnumerator FinalizeAsyncImpl(FinalizeAsyncArgs args)
        {
            if (finalizeAsync != null)
            {
                foreach (FinalizeAsyncDelegate func in finalizeAsync.GetInvocationList())
                {
                    yield return func(args);
                }
                finalizeAsync = null;
            }
            Content.PopulateNetworkedObjectAssetIds();
        }

        IEnumerator IContentPackProvider.LoadStaticContentAsync(LoadStaticContentAsyncArgs args) => LoadStaticContentAsyncImpl(args);

        IEnumerator IContentPackProvider.GenerateContentPackAsync(GetContentPackAsyncArgs args) => GenerateContentPackAsyncImpl(args);

        IEnumerator IContentPackProvider.FinalizeAsync(FinalizeAsyncArgs args) => FinalizeAsyncImpl(args);
    }

    public abstract class BaseContentPlugin<TInstance> : BaseContentPlugin where TInstance : BaseContentPlugin<TInstance>
    {
        public static TInstance Instance { get; private set; }

        public BaseContentPlugin() : base()
        {
            Instance = (TInstance)this;
        }

    }
}