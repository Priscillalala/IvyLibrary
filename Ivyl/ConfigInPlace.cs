using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System.Reflection;
using BepInEx.Configuration;
using System.IO;

namespace Ivyl
{
    public abstract class ConfigInPlace<TSelf> : BaseConfigInPlace<TSelf> where TSelf : ConfigInPlace<TSelf>, new()
    {
        public abstract string ConfigName { get; }

        public virtual BaseUnityPlugin Owner { get; } = null;

        protected sealed override bool Init()
        {
            return (ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ConfigName + ".cfg"), true, Owner?.Info?.Metadata)) != null;
        }
    }

    public class PluginConfigInPlace<TPlugin> : BaseConfigInPlace<PluginConfigInPlace<TPlugin>> where TPlugin : BaseUnityPlugin, new()
    {
        protected sealed override bool Init()
        {
            if (!Chainloader.ManagerObject)
            {
                return false;
            }
            return (ConfigFile = Chainloader.ManagerObject.GetComponent<TPlugin>().Config) != null;
        }
    }

    public abstract class BaseConfigInPlace<TSelf> : BaseInstanceInPlace<TSelf> where TSelf : BaseConfigInPlace<TSelf>, new()
    {
        public static ConfigFile ConfigFile { get; protected set; }

        public static ConfigEntry<T> Bind<T>(ConfigDefinition configDefinition, T defaultValue, ConfigDescription configDescription = null)
        {
            TryInit();
            return ConfigFile.Bind(configDefinition, defaultValue, configDescription);
        }

        public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, ConfigDescription configDescription = null)
        {
            TryInit();
            return ConfigFile.Bind(section, key, defaultValue, configDescription);
        }

        public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
        {
            TryInit();
            return ConfigFile.Bind(section, key, defaultValue, description);
        }
    }
}