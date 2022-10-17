#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.CustomGUI;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class DrawBasicData 
    {
		public virtual void OnGUI(BasicData basicData, Type toolType, ClipboardBase clipboard, TemplateStackEditor templateStackEditor){}
	}

    public class DrawGeneralBasicData : DrawBasicData
    {
		public SelectionTypeWindow SelectionTypeWindow = new SelectionTypeWindow();
		public SelectionPrototypeWindow SelectionPrototypeWindow = new SelectionPrototypeWindow();

		public override void OnGUI(BasicData basicData, Type toolType, ClipboardBase clipboard, TemplateStackEditor templateStackEditor)
		{
			SelectionTypeWindow.OnGUI(basicData, basicData.SelectedVariables, clipboard);
			SelectionPrototypeWindow.OnGUI(basicData.SelectedVariables, toolType, clipboard, templateStackEditor);
		}
	}
}
#endif