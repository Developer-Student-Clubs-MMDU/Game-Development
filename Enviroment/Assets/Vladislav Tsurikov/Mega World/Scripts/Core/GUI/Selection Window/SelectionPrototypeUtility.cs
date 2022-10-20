#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class SelectionPrototypeUtility
    {
        public static void DeleteSelectedProtoInstantItem(Group group)
        {
            foreach (PrototypeInstantItem proto in group.ProtoInstantItemList.FindAll(proto => proto.Selected))
            {
                string pathToDelete = AssetDatabase.GetAssetPath(proto);      
                AssetDatabase.DeleteAsset(pathToDelete);
            }

            group.ProtoInstantItemList.RemoveAll((proto) => proto == null);

            AssetDatabase.SaveAssets();
        }

        public static void DeleteSelectedProtoGameObject(Group group)
        {
            foreach (PrototypeGameObject proto in group.ProtoGameObjectList.FindAll(proto => proto.Selected))
            {
                string pathToDelete = AssetDatabase.GetAssetPath(proto);      
                AssetDatabase.DeleteAsset(pathToDelete);
            }

            group.ProtoGameObjectList.RemoveAll((proto) => proto == null);

            AssetDatabase.SaveAssets();
        }

        public static void DeleteSelectedProtoTerrainDetail(Group group)
        {
            foreach (PrototypeTerrainDetail proto in group.ProtoTerrainDetailList.FindAll(proto => proto.Selected))
            {
                string pathToDelete = AssetDatabase.GetAssetPath(proto);      
                AssetDatabase.DeleteAsset(pathToDelete);
            }

            group.ProtoTerrainDetailList.RemoveAll((proto) => proto == null);

            AssetDatabase.SaveAssets();
        }

        public static void DeleteSelectedProtoTerrainTexture(Group group)
        {
            foreach (PrototypeTerrainTexture proto in group.ProtoTerrainTextureList.FindAll(proto => proto.Selected))
            {
                string pathToDelete = AssetDatabase.GetAssetPath(proto);      
                AssetDatabase.DeleteAsset(pathToDelete);
            }

            group.ProtoTerrainTextureList.RemoveAll((proto) => proto == null);

            AssetDatabase.SaveAssets();
        }

        public static void SelectPrototype(Group group, int prototypeIndex)
        {
            List<Prototype> protoList = group.GetPrototypes();

            SetSelectedAllPrototypes(group, false);

            if(prototypeIndex < 0 && prototypeIndex >= protoList.Count)
            {
                return;
            }

            protoList[prototypeIndex].Selected = true;
        }

        public static void SelectPrototypeAdditive(Group group, int prototypeIndex)
        {
            List<Prototype> protoList = group.GetPrototypes();

            if(prototypeIndex < 0 && prototypeIndex >= protoList.Count)
            {
                return;
            }

            protoList[prototypeIndex].Selected = !protoList[prototypeIndex].Selected;
        }

        public static void SelectPrototypeRange(Group group, int prototypeIndex)
        {
            List<Prototype> protoList = group.GetPrototypes();

            if(prototypeIndex < 0 && prototypeIndex >= group.ProtoGameObjectList.Count)
            {
                return;
            }

            int rangeMin = prototypeIndex;
            int rangeMax = prototypeIndex;

            for (int i = 0; i < protoList.Count; i++)
            {
                if (protoList[i].Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (protoList[i].Selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                protoList[i].Selected = true;
            }
        }

        public static void SetSelectedAllPrototypes(Group Group, bool select)
        {
            Group.ProtoInstantItemList.ForEach(proto => proto.Selected = select);
            Group.ProtoGameObjectList.ForEach(proto => proto.Selected = select);
            Group.ProtoTerrainDetailList.ForEach(proto => proto.Selected = select);
            Group.ProtoTerrainTextureList.ForEach(proto => proto.Selected = select);
        }

        public static void InsertSelectedProto(Group group, int index, bool after)
        {
            List<Prototype> selectedProto = new List<Prototype>();
            List<Prototype> currentPrototypes = group.GetPrototypes();

            currentPrototypes.ForEach ((Action<Prototype>)((proto) => { if(proto.Selected) selectedProto.Add(proto); }));

            if(selectedProto.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, currentPrototypes.Count);

                currentPrototypes.Insert(index, null); 
                currentPrototypes.RemoveAll (b => b != null && b.Selected); 
                currentPrototypes.InsertRange(currentPrototypes.IndexOf(null), selectedProto); 
                currentPrototypes.RemoveAll ((b) => b == null); 

                group.SetPrototypes(currentPrototypes);
            }
        }
    }
}
#endif