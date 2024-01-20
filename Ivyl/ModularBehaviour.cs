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
            Metadata = BaseModuleAttribute.earlyAssignmentMetadata;
            BaseModuleAttribute.earlyAssignmentMetadata = null;
        }
    }
}