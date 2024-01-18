using System;
using UnityEngine;
using System.Collections.Generic;
using RoR2;
using BepInEx.Bootstrap;

namespace IvyLibrary
{
    /// <summary>
    /// Inherit from this attribute to manage a modular codebase.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Define an attribute inheriting from <see cref="BaseModuleAttribute"/></item>
    /// <item>Implement <see cref="EnableInstance(MonoBehaviour, object[])"/> to define module behavior</item>
    /// <item>Add this attribute to component classes in your codebase (minimum <see cref="MonoBehaviour"/>)</item>
    /// <item>Initialize all modules with <see cref="InitializeModules(Type, string, object[])"/> or an overload; usually in plugin Awake</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseModuleAttribute : HG.Reflection.SearchableAttribute
    {
        /// <inheritdoc cref="InitializeModules{TModuleAttribute}(string, object[])"/>
        public static GameObject InitializeModules<TModuleAttribute>(params object[] args) where TModuleAttribute : BaseModuleAttribute
        {
            return InitializeModules(typeof(TModuleAttribute), args);
        }

        /// <summary>
        /// Initialize all modules of type <typeparamref name="TModuleAttribute"/>.
        /// </summary>
        /// <typeparam name="TModuleAttribute">A module attribute type inheriting from <see cref="BaseModuleAttribute"/>.</typeparam>
        /// <inheritdoc cref="InitializeModules(Type, string, object[])"/>
        public static GameObject InitializeModules<TModuleAttribute>(string managerObjectName, params object[] args) where TModuleAttribute : BaseModuleAttribute
        {
            return InitializeModules(typeof(TModuleAttribute), managerObjectName, args);
        }

        /// <inheritdoc cref="InitializeModules(Type, string, object[])"/>
        public static GameObject InitializeModules(Type attributeType, params object[] args)
        {
            return InitializeModules(attributeType, $"{attributeType.Name}_Manager", args);
        }

        /// <summary>
        /// Initialize all modules of <paramref name="attributeType"/>.
        /// </summary>
        /// <remarks>
        /// The module manager object will be parented to the <see cref="Chainloader.ManagerObject"/>, if present.
        /// </remarks>
        /// <param name="attributeType">A module attribute type inheriting from <see cref="BaseModuleAttribute"/>.</param>
        /// <param name="managerObjectName">The name of the manager <see cref="GameObject"/> that module instances will be attached to.</param>
        /// <param name="args">Optional arguments to be passed to <see cref="EnableInstance(MonoBehaviour, object[])"/>.</param>
        /// <returns>A manager <see cref="GameObject"/> with all modular components attached.</returns>
        public static GameObject InitializeModules(Type attributeType, string managerObjectName, params object[] args) 
        {
            if (!attributeType.IsSubclassOf(typeof(BaseModuleAttribute)))
            {
                throw new ArgumentException(nameof(attributeType));
            }
            GameObject managerObject = new GameObject(managerObjectName);
            UnityEngine.Object.DontDestroyOnLoad(managerObject);
            managerObject.transform.SetParent(Chainloader.ManagerObject?.transform);

            if (!instancesListsByType.TryGetValue(attributeType, out var attributesList) || attributesList.Count <= 0)
            {
                Debug.LogWarning($"{nameof(BaseModuleAttribute)}: Attempted to {nameof(InitializeModules)} of type {attributeType.Name} but no instances were found.");
                return managerObject;
            }

            HashSet<Type> targetBlacklist = new HashSet<Type>();
            managerObject.SetActive(false);
            foreach (BaseModuleAttribute attribute in attributesList)
            {
                if (attribute.target is not Type moduleType || !moduleType.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    Debug.LogWarning($"{nameof(BaseModuleAttribute)}: Module of type {attributeType.Name} has an invalid target ({attribute.target ?? "null"}). Target must be a class inheriting from {nameof(MonoBehaviour)}.");
                    continue;
                }
                if (targetBlacklist.Add(moduleType))
                {
                    MonoBehaviour moduleInstance = (MonoBehaviour)managerObject.AddComponent(moduleType);
                    moduleInstance.enabled = attribute.EnableInstance(moduleInstance, args);
                }
            }
            managerObject.SetActive(true);

            return managerObject;
        }

        /// <summary>
        /// Called during module initialization to handle module setup.
        /// </summary>
        /// <remarks>
        /// This method is called before any Unity messages on <paramref name="moduleInstance"/> (e.g., Awake, OnEnable).
        /// </remarks>
        /// <param name="moduleInstance">An instance of the <see cref="MonoBehaviour"/> this attribute was applied to.</param>
        /// <param name="args">Additional arguments optionally provided at module initialization time.</param>
        /// <returns>true if this module will be enabled; otherwise, false.</returns>
        public abstract bool EnableInstance(MonoBehaviour moduleInstance, object[] args);

    }
}