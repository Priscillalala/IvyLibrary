using BepInEx;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System.Reflection;
using System;
using System.ComponentModel;

namespace Ivyl
{
    public abstract class LoggerInPlace<TSelf> : BaseLoggerInPlace<TSelf> where TSelf : LoggerInPlace<TSelf>, new()
    {
        public abstract string SourceName { get; }

        protected sealed override bool Init()
        {
            return (ManualLogSource = Logger.CreateLogSource(SourceName)) != null;
        }
    }

    public class PluginLoggerInPlace<TPlugin> : BaseLoggerInPlace<PluginLoggerInPlace<TPlugin>> where TPlugin : BaseUnityPlugin, new()
    {
        protected sealed override bool Init()
        {
            if (!Chainloader.ManagerObject)
            {
                return false;
            }
            PropertyInfo logger = typeof(BaseUnityPlugin).GetProperty("Logger", BindingFlags.Instance | BindingFlags.NonPublic);
            return (ManualLogSource = (ManualLogSource)logger.GetMethod.Invoke(Chainloader.ManagerObject.GetComponent<TPlugin>(), null)) != null;
        }
    }

    public abstract class BaseLoggerInPlace<TSelf> : BaseInstanceInPlace<TSelf> where TSelf : BaseLoggerInPlace<TSelf>, new()
    {
        public static ManualLogSource ManualLogSource { get; protected set; }

        public static void Log(LogLevel level, object data) 
        {
            TryInit();
            ManualLogSource.Log(level, data);
        }

        public static void LogFatal(object data) 
        {
            TryInit();
            ManualLogSource.LogFatal(data);
        }
        public static void LogError(object data)
        {
            TryInit();
            ManualLogSource.LogError(data);
        }
        public static void LogWarning(object data)
        {
            TryInit();
            ManualLogSource.LogWarning(data);
        }
        public static void LogMessage(object data)
        {
            TryInit();
            ManualLogSource.LogMessage(data);
        }
        public static void LogInfo(object data)
        {
            TryInit();
            ManualLogSource.LogInfo(data);
        }
        public static void LogDebug(object data)
        {
            TryInit();
            ManualLogSource.LogDebug(data);
        }
    }
}