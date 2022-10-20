using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
	[CustomEditor(typeof(Group))]
    public class TypeEditor : Editor
    {
        private Group group;

		public SelectionPrototypeWindow SelectionPrototypeWindow = new SelectionPrototypeWindow();
		public readonly SelectedVariables SelectedVariables = new SelectedVariables();

        private void OnEnable()
        {
            group = (Group)target;
        }

        public override void OnInspectorGUI()
        {
            CustomEditorGUILayout.IsInspector = true;
            OnGUI();
        }

		public void OnGUI()
		{
			List<Group> groups = new List<Group>() {group};

			SelectedVariables.SetAllSelectedParameters(groups);
            SelectedVariables.SelectedGroupList.Add(group);
            SelectedVariables.SelectedGroup = group;

			SelectionPrototypeWindow.OnGUI(SelectedVariables, null, null, null);
		}
	}
}
