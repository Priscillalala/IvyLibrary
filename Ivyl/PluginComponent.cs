using System;
using HG.Reflection;
using System.ComponentModel;

namespace Ivyl
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PluginComponent : Attribute
    {
        public enum Flags 
        {
            None,
            Disabled = 1 << 0,
            ConfigComponent = 1 << 1,
            ConfigInstanceFields = 1 << 2,
            ConfigStaticFields = 1 << 3,
            ConfigAll = ConfigComponent | ConfigInstanceFields | ConfigStaticFields,
        }

        public PluginComponent(Type pluginType, Flags flags = Flags.None)
        {
            PluginType = pluginType;
            PluginComponentFlags = flags;
        }

        public Type PluginType { get; }

        public Flags PluginComponentFlags { get; }

        /*[Obsolete(".target is revealed by publicized assemblies but should not be used.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new object target 
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException(); 
        }

        public Type GetTargetType()
        {
            return base.target as Type;
        }*/
    }
}