using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class CommonDataPackage : ScriptableObject
    {
		[SerializeField]
		private ToolStack toolStack = new ToolStack();

		public ToolStack ToolStack 
		{
			get
			{
				toolStack.RemoveNullTool();
				return toolStack;
			}
		}
		
        public LayerSettings layerSettings = new LayerSettings();
        public TransformSpace TransformSpace = TransformSpace.Global;
        public ToolComponent SelectedTool;
        
#if UNITY_EDITOR
        private ToolStackEditor _toolComponentsEditor = null;
        public ResourcesControllerEditor ResourcesControllerEditor = new ResourcesControllerEditor();

		public ToolStackEditor ToolComponentsEditor
		{
			get
			{
				if(_toolComponentsEditor == null || _toolComponentsEditor.Stack == null)
				{
					_toolComponentsEditor = new ToolStackEditor(ToolStack);
				}

				return _toolComponentsEditor;
			}
		}

        public void Save()
		{
            EditorUtility.SetDirty(this);
		}
#endif
    }
}