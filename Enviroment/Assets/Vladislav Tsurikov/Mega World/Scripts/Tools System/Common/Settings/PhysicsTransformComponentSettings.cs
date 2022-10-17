using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class PhysicsTransformComponentSettings : BaseSettings
    {
        public TransformComponentStack Stack = new TransformComponentStack();

		public override void Init(ScriptableObject asset)
		{
			if(!Stack.Init)
            {
                Stack.AddSettings(typeof(PositionOffset), asset);
                Stack.AddSettings(typeof(Rotation), asset);
                Stack.AddSettings(typeof(Scale), asset);

                Stack.Init = true;
            }
		}

#if UNITY_EDITOR
		private TransformComponentStackEditor _transformComponentEditor = null;

		public override void OnGUI() 
		{
 
		}

		public void OnGUI(Prototype proto) 
		{
			if(_transformComponentEditor == null || _transformComponentEditor.Stack == null )
			{
				List<System.Type> types = new List<System.Type>() {typeof(Align)};
			    _transformComponentEditor = new TransformComponentStackEditor(proto, new GUIContent("Transform Components Settings"), Stack, types, true);
			}

			_transformComponentEditor.OnGUI(proto);
		}
#endif
    }
}