using RoR2.ContentManagement;
using System;
using System.Collections;

namespace BepInEx
{
    public abstract class BaseContentPlugin : BaseUnityPlugin, IContentPackProvider
    {
        public static BaseContentPlugin Instance { get; private set; }

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
            Instance = this;
            Content = new ContentPack
            {
                identifier = identifier
            };
            ContentManager.collectContentPackProviders += add => add(this);
        }

        public virtual IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
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

        public virtual IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
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

        public virtual IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            if (finalizeAsync != null)
            {
                foreach (FinalizeAsyncDelegate func in finalizeAsync.GetInvocationList())
                {
                    yield return func(args);
                }
                finalizeAsync = null;
            }
        }
    }
}