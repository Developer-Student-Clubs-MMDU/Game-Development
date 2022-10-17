using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class GetPrototype 
    {
        public static PrototypeGameObject GetCurrentPrototypeGameObject(GameObject go)
        {
            foreach (Group group in AllAvailableTypes.GetAllTypes())
            {                
                foreach (PrototypeGameObject proto in group.ProtoGameObjectList)  
                {
                    if(CommonUtility.IsSameGameObject(proto.Prefab, go))
                    {
                        return proto;
                    }
                }
            }

            return null;
        }

        public static PrototypeGameObject GetCurrentPrototypeGameObject(Group group, GameObject go)
        {
            foreach (PrototypeGameObject proto in group.ProtoGameObjectList)  
            {
                if(CommonUtility.IsSameGameObject(proto.Prefab, go))
                {
                    return proto;
                }
            }

            return null;
        }

        public static PrototypeGameObject GetCurrentPrototypeGameObject(int ID, bool getSelected)
        {
            foreach (Group group in AllAvailableTypes.GetAllTypes())
            {
                foreach (PrototypeGameObject proto in group.ProtoGameObjectList)
                {
                    if(proto.ID == ID)
                    {
                        return proto;
                    }
                }
            }
            
            return null;
        }

        public static PrototypeInstantItem GetCurrentInstantItem(int ID, bool getSelected)
        {
            foreach (Group group in AllAvailableTypes.GetAllTypes())
            {
                if(getSelected)
                {
                    if(!group.Selected)
                    {
                        continue;
                    }
                }

                foreach (PrototypeInstantItem proto in group.ProtoInstantItemList)
                {
                    if(proto.ID == ID)
                    {
                        return proto;
                    }
                }
            }
            
            return null;
        }
    }
}