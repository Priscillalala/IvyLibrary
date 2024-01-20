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
    public class ModularBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="BaseModuleAttribute"/> instance applied to this class.
        /// </summary>
        public BaseModuleAttribute Metadata { get; }

        public ModularBehaviour()
        {
            if (BaseModuleAttribute.earlyAssignmentMetadata != null)
            {
                Metadata = BaseModuleAttribute.earlyAssignmentMetadata;
                BaseModuleAttribute.earlyAssignmentMetadata = null;
            }
            else
            {
                List<HG.Reflection.SearchableAttribute> attributes = HG.Reflection.SearchableAttribute.GetInstances<BaseModuleAttribute>();
                if (attributes != null)
                {
                    Type type = GetType();
                    Metadata = (BaseModuleAttribute)attributes.FirstOrDefault(x => x.target is Type moduleType && moduleType == type);
                }
                if (Metadata == null)
                {
                    Debug.LogWarning($"Could not locate metadata for {nameof(ModularBehaviour)} instance of type {GetType().Name}!");
                }
            }
        }
    }
}