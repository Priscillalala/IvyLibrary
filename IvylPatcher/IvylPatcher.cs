using BepInEx.Bootstrap;
using BepInEx.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BepInEx;
using Ivyl;
using HG.Reflection;
using System.Linq;
using BepInEx.Configuration;
using System.IO;

namespace Ivyl
{
    public static class IvylPatcher
    {
        public static IEnumerable<string> TargetDLLs { get; } = Array.Empty<string>();

        private static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("IvylPatcher");
        private static Lookup<Type, PluginComponentAttribute> pluginComponentAttributesLookup;
        private static MethodInfo method_ConfigFile_Bind;
        private static Dictionary<Type, MethodInfo> genericBindCache;
        private static ILHook il_Chainloader_Initialize;
        private static ILHook il_Chainloader_Start;
        private static ILHook il_BaseUnityPlugin_ctor;

        public static void Finish()
        {
            //logger.LogInfo("Init!");

            il_Chainloader_Initialize = new ILHook(
                    typeof(Chainloader).GetMethod(nameof(Chainloader.Initialize)),
                    Chainloader_Initialize
                );
        }

        private static void Chainloader_Initialize(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.EmitDelegate<Action>(() => 
            {
                //logger.LogInfo("Chainloader Init!");
                //il_Chainloader_Initialize.Free();
                //il_Chainloader_Initialize = null;
                il_Chainloader_Start = new ILHook(
                    typeof(Chainloader).GetMethod(nameof(Chainloader.Start)),
                    Chainloader_Start
                );
                il_BaseUnityPlugin_ctor = new ILHook(
                    typeof(BaseUnityPlugin).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
                    BaseUnityPlugin_ctor
                );
            });
        }

        private static void Chainloader_Start(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.EmitDelegate<Action>(() =>
            {
                //logger.LogInfo("Chainloader Start!");

                static IEnumerable<PluginComponentAttribute> AssemblyPluginComponents(Assembly assembly)
                {
                    foreach (Type type in assembly.GetTypes().Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && !t.IsAbstract)) 
                    {
                        IEnumerable<RequireComponent> requireComponents = null;
                        HashSet<Type> pluginTypes = new HashSet<Type>();
                        foreach (PluginComponent attribute in type.GetCustomAttributes<PluginComponent>(false))
                        {
                            pluginTypes.Add(attribute.PluginType);
                            yield return new PluginComponentAttribute
                            {
                                attribute = attribute,
                                requireComponents = (requireComponents ??= type.GetCustomAttributes<RequireComponent>()),
                                componentType = type,
                            };
                        }
                        Type currentType = type.BaseType;
                        while (currentType != null && currentType != typeof(MonoBehaviour))
                        {
                            foreach (PluginComponent attribute in currentType.GetCustomAttributes<PluginComponent>(false))
                            {
                                if (!pluginTypes.Add(attribute.PluginType))
                                {
                                    continue;
                                }
                                yield return new PluginComponentAttribute
                                {
                                    attribute = attribute,
                                    requireComponents = (requireComponents ??= type.GetCustomAttributes<RequireComponent>()),
                                    componentType = type,
                                };
                            }
                            currentType = currentType.BaseType;
                        }
                    }
                }

                string assemblyName = typeof(PluginComponent).Assembly.GetName().FullName;
                pluginComponentAttributesLookup = (Lookup<Type, PluginComponentAttribute>)AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(x => x.GetReferencedAssemblies().Any(x => x.FullName == assemblyName))
                    .SelectMany(AssemblyPluginComponents)
                    .ToLookup(x => x.attribute.PluginType);
                method_ConfigFile_Bind = typeof(ConfigFile).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where((MethodInfo info) => info.Name == nameof(ConfigFile.Bind)).FirstOrDefault();
                genericBindCache = new Dictionary<Type, MethodInfo>();
            });

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<PluginInfo>("set_Instance")
                ))
            {
                c.Emit(OpCodes.Dup);
                c.EmitDelegate<Action<BaseUnityPlugin>>(OnPluginInstantiated);
            }
            else logger.LogError($"{nameof(Chainloader_Start)} IL hook failed!");

            c.Index = c.Instrs.Count - 1;
            c.EmitDelegate<Action>(() =>
            {
                //logger.LogWarning("Chainloader Start end!");
                pluginComponentAttributesLookup = null;
                method_ConfigFile_Bind = null;
                genericBindCache = null;
            });
        }

        private static void BaseUnityPlugin_ctor(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdstr(".cfg"),
                x => x.MatchCallOrCallvirt<string>(nameof(String.Concat))
                ))
            {
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate<Func<string, BaseUnityPlugin, string>>((guid, plugin) =>
                {
                    BepInConfig configName = plugin.GetType().GetCustomAttribute<BepInConfig>();
                    return configName != null ? configName.ConfigName : guid;
                });
            }
            else logger.LogError($"{nameof(BaseUnityPlugin_ctor)} IL hook 1 failed!");

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchNewobj<ConfigFile>()
                ))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate<Func<bool, BaseUnityPlugin, bool>>((saveOnInit, plugin) => Attribute.IsDefined(plugin.GetType(), typeof(BepInConfig)) || saveOnInit);
            }
            else logger.LogError($"{nameof(BaseUnityPlugin_ctor)} IL hook 2 failed!");

            /*c.Index = c.Instrs.Count - 1;
            c.EmitDelegate<Action>(() =>
            {
                il_Chainloader_Start.Free();
                il_Chainloader_Start = null;
            });*/
        }

        public static void OnPluginInstantiated(BaseUnityPlugin plugin)
        {
            Type pluginType = plugin.GetType();
            if (!pluginComponentAttributesLookup.Contains(pluginType))
            {
                return;
            }
            BepInConfig bepInConfig = pluginType.GetCustomAttribute<BepInConfig>();
            GameObject pluginObject = new GameObject(plugin.Info.Metadata.GUID);
            pluginObject.transform.SetParent(Chainloader.ManagerObject.transform);
            //UnityEngine.Object.DontDestroyOnLoad(pluginObject);
            pluginObject.SetActive(false);

            List<PluginComponentAttribute> evalAttributesList = pluginComponentAttributesLookup[pluginType].ToList();

            bool IsRequireComponentSatisfied(RequireComponent requireComponent)
            {
                return (requireComponent.m_Type0 == null || pluginObject.GetComponent(requireComponent.m_Type0))
                && (requireComponent.m_Type1 == null || pluginObject.GetComponent(requireComponent.m_Type1))
                && (requireComponent.m_Type2 == null || pluginObject.GetComponent(requireComponent.m_Type2));
            }

            void EvalAttribute(PluginComponentAttribute pluginComponentAttribute, bool addIfValid)
            {
                PluginComponent attribute = pluginComponentAttribute.attribute;
                Type componentType = pluginComponentAttribute.componentType;

                bool shouldAdd = (attribute.PluginComponentFlags & PluginComponent.Flags.Disabled) <= PluginComponent.Flags.None;
                
                if ((attribute.PluginComponentFlags & PluginComponent.Flags.ConfigAll) <= PluginComponent.Flags.None)
                {
                    if (shouldAdd && addIfValid)
                    {
                        pluginObject.AddComponent(componentType);
                        return;
                    }
                }

                string section = null;
                BepInConfig.ComponentGroupingFlags componentEntriesGrouping = bepInConfig?.ComponentEntriesGrouping ?? BepInConfig.ComponentGroupingFlags.ByComponent;
                if (componentEntriesGrouping > BepInConfig.ComponentGroupingFlags.None)
                {
                    if ((componentEntriesGrouping & BepInConfig.ComponentGroupingFlags.ByNamespace) > BepInConfig.ComponentGroupingFlags.None)
                    {
                        section = IvyLibrary.Nicify(componentType.Namespace.Substring(componentType.Namespace.LastIndexOf('.') + 1));
                    }
                    if ((componentEntriesGrouping & BepInConfig.ComponentGroupingFlags.ByComponent) > BepInConfig.ComponentGroupingFlags.None)
                    {
                        section = section != null ? section + ": " + IvyLibrary.Nicify(componentType.Name) : IvyLibrary.Nicify(componentType.Name);
                    }
                }
                if (section == null)
                {
                    section = Path.GetFileNameWithoutExtension(plugin.Config.ConfigFilePath); ;
                }

                if ((attribute.PluginComponentFlags & PluginComponent.Flags.ConfigComponent) > PluginComponent.Flags.None)
                {
                    string name = IvyLibrary.Nicify(componentType.Name);
                    shouldAdd = plugin.Config.Bind(new ConfigDefinition(section, $"Enable {name}"), shouldAdd).Value;
                }

                static bool ShouldSerializeField(FieldInfo field)
                {
                    bool serialize = Attribute.IsDefined(field, typeof(SerializeField));
                    bool nonSerialized = Attribute.IsDefined(field, typeof(NonSerializedAttribute));
                    return field.IsPublic ? (serialize || !nonSerialized) : (serialize && !nonSerialized);
                }

                void BindFieldConfig(FieldInfo field, object instance)
                {
                    Type fieldType = field.FieldType;
                    string name = IvyLibrary.Nicify(field.Name);
                    string description = field.GetCustomAttribute<TooltipAttribute>()?.tooltip;
                    AcceptableValueBase acceptableValues = null;

                    if (fieldType.IsPrimitive && typeof(IComparable).IsAssignableFrom(fieldType))
                    {
                        RangeAttribute rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
                        MinAttribute minAttribute = field.GetCustomAttribute<MinAttribute>();
                        float? min = null;
                        float? max = null;
                        if (rangeAttribute != null)
                        {
                            min = rangeAttribute.min;
                            max = rangeAttribute.max;
                        }
                        if (minAttribute != null && (min == null || minAttribute.min > min))
                        {
                            min = minAttribute.min;
                        }
                        if (min != null && max != null)
                        {
                            acceptableValues = new AcceptableValueRange((IComparable)Convert.ChangeType((float)min, fieldType), (IComparable)Convert.ChangeType((float)max, fieldType), fieldType);
                        }
                        else if (min != null)
                        {
                            acceptableValues = new AcceptableValueFloor((IComparable)Convert.ChangeType((float)min, fieldType), fieldType);
                        }
                    }

                    if (!genericBindCache.TryGetValue(fieldType, out MethodInfo genericBind))
                    {
                        genericBind = method_ConfigFile_Bind.MakeGenericMethod(fieldType);
                        genericBindCache.Add(fieldType, genericBind);
                    }

                    field.SetValue(instance, ((ConfigEntryBase)genericBind.Invoke(plugin.Config, new object[]
                    {
                            new ConfigDefinition(section, name),
                            field.GetValue(instance),
                            (description != null || acceptableValues != null) ? new ConfigDescription(description ?? string.Empty, acceptableValues) : null,
                    })).BoxedValue);
                }

                if ((attribute.PluginComponentFlags & PluginComponent.Flags.ConfigStaticFields) > PluginComponent.Flags.None)
                {
                    foreach (FieldInfo staticField in componentType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(ShouldSerializeField))
                    {
                        BindFieldConfig(staticField, null);
                    }
                }

                if (shouldAdd && addIfValid)
                {
                    Component instance = pluginObject.AddComponent(componentType);
                    if ((attribute.PluginComponentFlags & PluginComponent.Flags.ConfigInstanceFields) > PluginComponent.Flags.None)
                    {
                        foreach (FieldInfo instanceField in componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(ShouldSerializeField))
                        {
                            BindFieldConfig(instanceField, instance);
                        }
                    }
                }
            }

            int i = 0;
            while (i < evalAttributesList.Count)
            {
                if (evalAttributesList[i].requireComponents.All(IsRequireComponentSatisfied))
                {
                    EvalAttribute(evalAttributesList[i], true);
                    evalAttributesList.RemoveAt(i);
                    i = 0;
                    continue;
                }
                i++;
            }

            foreach (var pluginComponentAttribute in evalAttributesList)
            {
                EvalAttribute(pluginComponentAttribute, false);
            }

            pluginObject.SetActive(true);
        }

        public static void Patch(AssemblyDefinition _) { }

        public struct PluginComponentAttribute
        {
            public PluginComponent attribute;
            public IEnumerable<RequireComponent> requireComponents;
            public Type componentType;
        }
    }
}
