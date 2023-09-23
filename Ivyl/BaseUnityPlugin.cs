using System;

namespace BepInEx
{
    public abstract class BaseUnityPlugin<TSelf> : BaseUnityPlugin where TSelf : BaseUnityPlugin<TSelf>
    {
        public static TSelf Instance { get; private set; }

        protected BaseUnityPlugin() : base()
        {
            Instance ??= this as TSelf;
        }
    }
}