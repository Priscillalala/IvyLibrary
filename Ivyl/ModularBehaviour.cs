using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace IvyLibrary
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that stores <see cref="BaseModuleAttribute"/> metadata.
    /// </summary>
    /// <remarks>
    /// This class is can be optionally implemented alongside a <see cref="BaseModuleAttribute"/> to avoid reflection.
    /// </remarks>
    /// <typeparam name="TModuleAttribute">The <see cref="BaseModuleAttribute"/> type associated with this behaviour.</typeparam>
    public abstract class ModularBehaviour<TModuleAttribute> : MonoBehaviour where TModuleAttribute : BaseModuleAttribute
    {
        /// <summary>
        /// The <typeparamref name="TModuleAttribute"/> instance applied to this class.
        /// </summary>
        public TModuleAttribute Metadata { get; }

        public ModularBehaviour()
        {
            if (BaseModuleAttribute.earlyAssignmentMetadata is TModuleAttribute earlyAssignmentMetadata)
            {
                Metadata = earlyAssignmentMetadata;
                BaseModuleAttribute.earlyAssignmentMetadata = null;
            }
            else
            {
                List<HG.Reflection.SearchableAttribute> attributes = HG.Reflection.SearchableAttribute.GetInstances<TModuleAttribute>();
                if (attributes != null)
                {
                    Type type = GetType();
                    Metadata = (TModuleAttribute)attributes.FirstOrDefault(x => x.target is Type moduleType && moduleType == type);
                }
                if (Metadata == null)
                {
                    Debug.LogWarning($"Could not locate metadata for {nameof(ModularBehaviour<TModuleAttribute>)} instance of type {GetType().Name}!");
                }
            }
        }
    }
}