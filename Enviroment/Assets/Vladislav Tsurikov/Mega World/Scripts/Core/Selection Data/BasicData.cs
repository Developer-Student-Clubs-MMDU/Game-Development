using System.Collections.Generic;
using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class BasicData
    {
        public List<Group> GroupList = new List<Group>();
        public readonly SelectedVariables SelectedVariables = new SelectedVariables();

#if UNITY_EDITOR
        public Vector2 TypeWindowsScroll = Vector2.zero;
        public BasicDataEditor BasicDataEditor = new BasicDataEditor();

        public void OnGUI(DrawBasicData drawBasicData, Type toolType, ClipboardBase clipboard, TemplateStackEditor templateStackEditor)
        {
            BasicDataEditor.OnGUI(this, drawBasicData, toolType, clipboard, templateStackEditor);
        }

        public void SaveAllData()
		{
			foreach (Group group in GroupList)
			{
                group.Save();

                List<Prototype> allPrototypes = new List<Prototype>(); 

                allPrototypes.AddRange(group.ProtoGameObjectList);
                allPrototypes.AddRange(group.ProtoInstantItemList);
                allPrototypes.AddRange(group.ProtoTerrainDetailList);
                allPrototypes.AddRange(group.ProtoTerrainTextureList);

                allPrototypes.ForEach(proto => proto.Save());
			}
		}
#endif
    }
}