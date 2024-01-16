using RoR2;
using System.Runtime.CompilerServices;

namespace IvyLibrary
{
    /// <summary>
    /// Static extensions for manipulating a <see cref="NetworkSoundEventDef"/> at runtime. 
    /// </summary>
    /// <remarks>
    /// Allows method chaining syntax.
    /// </remarks>
    public static class NetworkSoundEventExtensions
    {
        /// <summary>
        /// Set the event name to be played by this network sound event.
        /// </summary>
        /// <returns><paramref name="networkSoundEventDef"/>, to continue a method chain.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNetworkSoundEventDef SetEventName<TNetworkSoundEventDef>(this TNetworkSoundEventDef networkSoundEventDef, string eventName) where TNetworkSoundEventDef : NetworkSoundEventDef
        {
            networkSoundEventDef.eventName = eventName;
            return networkSoundEventDef;
        }
    }
}