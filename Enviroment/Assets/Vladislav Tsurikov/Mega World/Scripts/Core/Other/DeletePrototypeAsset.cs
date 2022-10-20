#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class DeletePrototypeAsset 
    {   
        static DeletePrototypeAsset()
        {
            EditorApplication.update -= Check;
            EditorApplication.update += Check;
        }

        public static void Check()
        {
            bool find = false;

            foreach (Prototype proto in AllAvailablePrototypes.ProtoList)
            {
                if(proto.IsNullType())
                {
                    string pathToDelete = AssetDatabase.GetAssetPath(proto);      
                    AssetDatabase.DeleteAsset(pathToDelete);
                    find = true;
                }
            }

            if(find)
            {
                AllAvailablePrototypes.ProtoList.RemoveAll((proto) => proto == null);
                AssetDatabase.SaveAssets();
            }
        }
    }   
}
#endif