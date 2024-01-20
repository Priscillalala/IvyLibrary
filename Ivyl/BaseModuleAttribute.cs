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
    /// <item>Implement <see cref="Initialize(object[], List{HG.Reflection.SearchableAttribute})"/> to define modularity</item>
    /// <item>Add this attribute to behaviour classes (e.g., <see cref="Behaviour"/>, <see cref="MonoBehaviour"/>, <see cref="ModularBehaviour"/>, or derived) in your codebase</item>
    /// <item>Initialize all modules with <see cref="InitializeModules(Type, object[])"/> or an overload (usually during plugin Awake)</item>
    /// </list>
    /// <para><see cref="BaseModuleAttribute"/> is a <see cref="HG.Reflection.SearchableAttribute"/>. <see cref="HG.Reflection.SearchableAttribute.OptInAttribute"/> must be present.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseModuleAttribute : HG.Reflection.SearchableAttribute
    {
        /// <summary>
        /// Initialize all modules of type <typeparamref name="TModuleAttribute"/>.
        /// </summary>
        /// <typeparam name="TModuleAttribute">A module attribute type inheriting from <see cref="BaseModuleAttribute"/>.</typeparam>
        /// <inheritdoc cref="InitializeModules(Type, object[])"/>
        public static GameObject InitializeModules<TModuleAttribute>(params object[] args) where TModuleAttribute : BaseModuleAttribute
        {
            return InitializeModules(typeof(TModuleAttribute), args);
        }

        /// <summary>
        /// Initialize all modules of type <typeparamref name="TModuleAttribute"/> and attach them to <paramref name="managerObject"/>.
        /// </summary>
        /// <typeparam name="TModuleAttribute">A module attribute type inheriting from <see cref="BaseModuleAttribute"/>.</typeparam>
        /// <inheritdoc cref="InitializeModules(Type, GameObject, object[])"/>
        public static void InitializeModules<TModuleAttribute>(GameObject managerObject, params object[] args) where TModuleAttribute : BaseModuleAttribute
        {
            InitializeModules(typeof(TModuleAttribute), managerObject, args);
        }

        /// <summary>
        /// Initialize all modules of <paramref name="attributeType"/>.
        /// </summary>
        /// <remarks>
        /// A new module manager object will be parented to the <see cref="Chainloader.ManagerObject"/>, if present.
        /// </remarks>
        /// <inheritdoc cref="InitializeModules(Type, GameObject, object[])"/>
        /// <returns>A manager <see cref="GameObject"/> with all modular behaviours attached.</returns>
        public static GameObject InitializeModules(Type attributeType, params object[] args)
        {
            GameObject managerObject = new GameObject($"{attributeType.Name}_Manager");
            UnityEngine.Object.DontDestroyOnLoad(managerObject);
            managerObject.transform.SetParent(Chainloader.ManagerObject?.transform);
            InitializeModules(attributeType, managerObject, args);
            return managerObject;
        }

        /// <summary>
        /// Initialize all modules of <paramref name="attributeType"/> and attach them to <paramref name="managerObject"/>.
        /// </summary>
        /// <param name="attributeType">A module attribute type inheriting from <see cref="BaseModuleAttribute"/>.</param>
        /// <param name="managerObject">A <see cref="GameObject"/> that module instances will be attached to.</param>
        /// <param name="args">Optional arguments passed to <see cref="Initialize(object[], List{HG.Reflection.SearchableAttribute})"/>.</param>
        public static void InitializeModules(Type attributeType, GameObject managerObject, params object[] args) 
        {
            if (managerObject == null)
            {
                throw new ArgumentNullException(nameof(managerObject));
            }
            if (!attributeType.IsSubclassOf(typeof(BaseModuleAttribute)))
            {
                throw new ArgumentException(nameof(attributeType));
            }
            if (!instancesListsByType.TryGetValue(attributeType, out var attributesList) || attributesList.Count <= 0)
            {
                Debug.LogWarning($"{nameof(BaseModuleAttribute)}: Attempted to {nameof(InitializeModules)} of type {attributeType.Name} but no instances were found.");
                return;
            }

            HashSet<object> targetBlacklist = new HashSet<object>();
            foreach (BaseModuleAttribute attribute in attributesList)
            {
                if (attribute.target is not Type moduleType || !moduleType.IsSubclassOf(typeof(Behaviour)))
                {
                    Debug.LogWarning($"{nameof(BaseModuleAttribute)}: Module of type {attributeType.Name} has an invalid target ({attribute.target ?? "null"}). Target must be a class inheriting from {nameof(Behaviour)}.");
                    continue;
                }
                if (targetBlacklist.Add(attribute.target) && attribute.Initialize(args, attributesList))
                {
                    earlyAssignmentMetadata = attribute;
                    managerObject.RequireComponent(moduleType);
                    earlyAssignmentMetadata = null;
                }
            }
        }

        internal static BaseModuleAttribute earlyAssignmentMetadata;

        /// <summary>
        /// Implement this method to determine when this modular behaviour can be instantiated.
        /// </summary>
        /// <param name="args">Additional arguments optionally provided at module initialization time.</param>
        /// <param name="peers">All module attributes of this type; includes this attribute and may include duplicates.</param>
        /// <returns>true if this module will be instantiated; otherwise, false.</returns>
        protected abstract bool Initialize(object[] args, List<HG.Reflection.SearchableAttribute> peers);
    }
}