using RoR2;

namespace IvyLibrary
{
    public static class NetworkSoundEventExtensions
    {
        public static TNetworkSoundEventDef SetEventName<TNetworkSoundEventDef>(this TNetworkSoundEventDef networkSoundEventDef, string eventName) where TNetworkSoundEventDef : NetworkSoundEventDef
        {
            networkSoundEventDef.eventName = eventName;
            return networkSoundEventDef;
        }
    }
}