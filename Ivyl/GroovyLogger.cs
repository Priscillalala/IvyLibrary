using BepInEx;
using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using MonoMod.Cil;
using BepInEx.Logging;

namespace Ivyl
{
    internal static class GroovyLogger
    {
        internal static ManualLogSource Logger => _logger ??= BepInEx.Logging.Logger.CreateLogSource("Ivyl");

        private static ManualLogSource _logger;

        internal static void Log(LogLevel level, object data) => Logger.Log(level, data);
    }
}