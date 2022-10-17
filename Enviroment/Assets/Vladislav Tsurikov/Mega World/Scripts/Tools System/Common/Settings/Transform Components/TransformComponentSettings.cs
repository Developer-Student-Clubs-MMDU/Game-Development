using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class TransformComponentSettings : BaseSettings
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

		public void OnGUI(Prototype proto, string text) 
		{
			if(_transformComponentEditor == null || _transformComponentEditor.Stack == null )
			{
				_transformComponentEditor = new TransformComponentStackEditor(proto, new GUIContent(text), Stack);
			}

			_transformComponentEditor.OnGUI(proto);
		}

		public void OnGUI(Prototype proto, string text, bool useSimpleComponent) 
		{
			if(_transformComponentEditor == null || _transformComponentEditor.Stack == null )
			{
				_transformComponentEditor = new TransformComponentStackEditor(proto, new GUIContent(text), Stack, useSimpleComponent);
			}

			_transformComponentEditor.OnGUI(proto);
		}
#endif
    }
}