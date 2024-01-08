using RoR2;

namespace IvyLibrary
{
    public static class ItemRelationshipProviderExtensions
    {
        public static TItemRelationshipProvider SetRelationshipType<TItemRelationshipProvider>(this TItemRelationshipProvider itemRelationshipProvider, ItemRelationshipType relationshipType) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            itemRelationshipProvider.relationshipType = relationshipType;
            return itemRelationshipProvider;
        }

        public static TItemRelationshipProvider SetRelationships<TItemRelationshipProvider>(this TItemRelationshipProvider itemRelationshipProvider, params ItemDef.Pair[] relationships) where TItemRelationshipProvider : ItemRelationshipProvider
        {
            itemRelationshipProvider.relationships = relationships;
            return itemRelationshipProvider;
        }
    }
}