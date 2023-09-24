using System;

namespace BepInEx
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BepInConfig : Attribute
    {
        public string ConfigName { get; protected set; }

        public ComponentGroupingFlags ComponentEntriesGrouping { get; protected set; }

        public BepInConfig(string ConfigName, ComponentGroupingFlags ComponentEntriesGrouping = ComponentGroupingFlags.ByComponent)
        {
            this.ConfigName = ConfigName;
            this.ComponentEntriesGrouping = ComponentEntriesGrouping;
        }

        [Flags]
        public enum ComponentGroupingFlags
        {
            None,
            ByComponent = 1 << 0,
            ByNamespace = 1 << 1,
        }
    }
}