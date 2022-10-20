using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class MaskFilterSettings : BaseSettings
    {
        public MaskFilterStack Stack = new MaskFilterStack();
        public MaskFilterContext FilterContext;
		public Texture2D FilterMaskTexture2D;

#if UNITY_EDITOR
		private MaskFilterStackEditor _maskFilterStackEditor = null;

		public override void OnGUI() 
		{

		}

        public void OnGUI(ScriptableObject asset, string text) 
		{
			if(_maskFilterStackEditor == null || _maskFilterStackEditor.Stack == null)
			{
				_maskFilterStackEditor = new MaskFilterStackEditor(new GUIContent("Mask Filters Settings"), Stack);
			}

			_maskFilterStackEditor.OnGUI(asset);
		}
#endif
    }
}