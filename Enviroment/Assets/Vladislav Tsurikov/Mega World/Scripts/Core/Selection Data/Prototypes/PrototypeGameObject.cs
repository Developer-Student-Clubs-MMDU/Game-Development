using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class PrototypeGameObject : Prototype
    {
		public PrototypeGameObject()
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

		public static PrototypeGameObject Create(Group group, GameObject gameObject, Vector3 extents)
        {
			PrototypeGameObject proto = (PrototypeGameObject)Prototype.Create(gameObject.name, group, typeof(PrototypeGameObject));
			proto.Init(gameObject, extents);
			return proto;
        }

        private void Init(GameObject gameObject, Vector3 extents)
        {
            Prefab = gameObject;
            PastTransform = new PastTransform(gameObject.transform);
            Extents = extents;
			ID = gameObject.GetInstanceID();

#if UNITY_EDITOR
            CreateMegaWorldWindowSettings.CreatePrototypeSettings(this);
#endif
        }
    }
}