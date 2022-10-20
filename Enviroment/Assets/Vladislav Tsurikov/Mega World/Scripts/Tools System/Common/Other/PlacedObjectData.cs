#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class PlacedObjectData
    {
        public ResourceType ResourceType;
        public Prototype Proto;
        public PlacedObject PlacedObject;

        public PlacedObjectData(Prototype proto, PlacedObject placedObject)
        {
            Proto = proto; 
            PlacedObject = placedObject;

            if(proto is PrototypeGameObject)
            {
                ResourceType = ResourceType.GameObject;
            }
            else if(proto is PrototypeInstantItem)
            {
                ResourceType = ResourceType.InstantItem;
            }
        }
    }
}
#endif