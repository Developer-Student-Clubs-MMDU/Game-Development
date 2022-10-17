using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class PrototypeInstantItem : Prototype
    {
		public PrototypeInstantItem()
		{
			
		}

#if UNITY_EDITOR
		public override void SetIconInfo(out Texture2D preview, out string name)
		{
            if (Prefab != null)
            {
                preview = MegaWorldGUIUtility.GetPrefabPreviewTexture(Prefab);      
				name = Prefab.name;
            }
			else
			{
				preview = null;
				name = "Missing Prefab";
			}
		}
#endif

		public static PrototypeInstantItem Create(Group type, GameObject gameObject, Vector3 extents)
        {
			PrototypeInstantItem proto = (PrototypeInstantItem)Prototype.Create(gameObject.name, type, typeof(PrototypeInstantItem));
			proto.Init(gameObject, extents);
			return proto;
        }

        private void Init(GameObject go, Vector3 extents)
        {
            Prefab = go;
			Extents = extents;
			ID = go.GetInstanceID();

#if UNITY_EDITOR
			CreateMegaWorldWindowSettings.CreatePrototypeSettings(this);
#endif
			
        }
    }
}