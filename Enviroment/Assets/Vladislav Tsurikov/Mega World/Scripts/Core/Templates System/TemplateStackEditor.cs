#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
	public class TemplateStackEditor
    {
		public List<Template> TemplateList;

        public TemplateStackEditor()
        {
			TemplateList = new List<Template>();

            for(int i = 0; i < AllTemplateTypes.TypeList.Count; ++i) 
            {
                System.Type type = AllTemplateTypes.TypeList[i];

                Create(type);
            }
        }

        public void ShowManu(GenericMenu menu, Type toolType, SelectedVariables selectedVariables)
        {
            menu.AddSeparator("");

            for(int i = 0; i < AllTemplateTypes.TypeList.Count; i++)
            {
                TemplateAttribute templateAttribute = AllTemplateTypes.TypeList[i].GetAttribute<TemplateAttribute>();

                string name = templateAttribute.Name;
				ResourceType[] supportedResourceTypes = templateAttribute.SupportedResourceTypes;

				Template template = TemplateList[i];

                if(templateAttribute.ToolTypes.Contains(toolType))
                {
                    menu.AddItem(new GUIContent("Apply Templates/" + name), false, () => template.Apply(supportedResourceTypes, selectedVariables));
                }
            }
        }

		private void Create(Type type)
        {
            var editor = (Template)Activator.CreateInstance(type);
            TemplateList.Add(editor);
        }
	}
}
#endif