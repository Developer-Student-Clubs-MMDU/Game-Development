#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace VladislavTsurikov.MegaWorldSystem
{
	public class Template 
    {
		public void Apply(ResourceType[] supportedResourceTypes, SelectedVariables selectedVariables)
		{
			if(supportedResourceTypes.Contains(selectedVariables.SelectedGroup.ResourceType))
			{
				Apply(selectedVariables.SelectedGroup);

				foreach (Prototype proto in selectedVariables.GetAllSelectedPrototypes())
				{
					Apply(proto);
				}
			}
		}

		public virtual void Apply(Group group){}
		public virtual void Apply(Prototype proto){}
	}
}
#endif