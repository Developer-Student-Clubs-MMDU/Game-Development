using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class GetRandomPrototype 
    {
        public static PrototypeGameObject GetMaxSuccessProtoGameObject(List<PrototypeGameObject> protoList)
        {
            List<Prototype> localProtoList = new List<Prototype>();
            localProtoList.AddRange(protoList);
            return (PrototypeGameObject)GetMaxSuccessProto(localProtoList);
        }

        public static PrototypeInstantItem GetMaxSuccessProtoInstantItem(List<PrototypeInstantItem> protoList)
        {
            List<Prototype> localProtoList = new List<Prototype>();
            localProtoList.AddRange(protoList);
            return (PrototypeInstantItem)GetMaxSuccessProto(localProtoList);
        }

        public static Prototype GetMaxSuccessProto(List<Prototype> protoList)
        {
            if(protoList.Count == 0)
            {
                Debug.Log("You have not selected more than one prototype.");
                return null;
            }

            if(protoList.Count == 1)
            {
                return protoList[0];
            }

            List<float> successProto = new List<float>();

            foreach (Prototype proto in protoList)
            {
                SuccessSettings successSettings = (SuccessSettings)proto.GetSettings(typeof(SuccessSettings));

                if(proto.Active == false)
                {
                    successProto.Add(-1);
                    continue;
                }

                float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

                if(randomSuccess > successSettings.SuccessValue / 100)
                {
                    randomSuccess = successSettings.SuccessValue / 100;
                }

                successProto.Add(randomSuccess);
            }

            float maxSuccessProto = successProto.Max();
            int maxIndexProto = 0;

            maxIndexProto = successProto.IndexOf(maxSuccessProto);
            return protoList[maxIndexProto];
        }

        public static Prototype GetRandomSelectedPrototype(Group group)
        {
            if(group.ResourceType == ResourceType.GameObject)
            {
                List<PrototypeGameObject> protoList = group.GetAllSelectedGameObject();

                return protoList[UnityEngine.Random.Range(0, protoList.Count)];
            }
            else if(group.ResourceType == ResourceType.InstantItem)
            {
                List<PrototypeInstantItem> protoList = group.GetAllSelectedInstantItem();

                return protoList[UnityEngine.Random.Range(0, protoList.Count)];
            }
            
            return null;
        }
    }
}