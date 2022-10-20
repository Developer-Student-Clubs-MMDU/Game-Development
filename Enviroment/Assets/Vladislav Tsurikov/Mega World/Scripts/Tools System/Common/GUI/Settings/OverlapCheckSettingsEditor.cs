#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.CustomGUI;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class OverlapCheckSettingsEditor 
    {
        public bool overlapСheckFoldout = true;

        public void OnGUI(OverlapCheckSettings settings)
        {
            overlapСheckFoldout = CustomEditorGUILayout.Foldout(overlapСheckFoldout, "Overlap Check Settings");

			if(overlapСheckFoldout)
			{
				EditorGUI.indentLevel++;

				CustomEditorGUILayout.BeginChangeCheck();
				
				settings.OverlapShape = (OverlapShape)CustomEditorGUILayout.EnumPopup(overlapShape, settings.OverlapShape);

				EditorGUI.indentLevel++;

				switch (settings.OverlapShape)
				{
					case OverlapShape.Bounds:
					{
						BoundsCheck boundsCheck = settings.BoundsCheck;

						boundsCheck.BoundsType = (BoundsCheckType)CustomEditorGUILayout.EnumPopup(boundsType, boundsCheck.BoundsType);

						if(boundsCheck.BoundsType == BoundsCheckType.Custom)
						{
							boundsCheck.UniformBoundsSize = CustomEditorGUILayout.Toggle(uniformBoundsSize, boundsCheck.UniformBoundsSize);

							if(boundsCheck.UniformBoundsSize)
							{
								boundsCheck.BoundsSize.x = CustomEditorGUILayout.FloatField(boundsSize, boundsCheck.BoundsSize.x);

								boundsCheck.BoundsSize.z = boundsCheck.BoundsSize.x;
								boundsCheck.BoundsSize.y = boundsCheck.BoundsSize.x;
							}
							else
							{
								boundsCheck.BoundsSize = CustomEditorGUILayout.Vector3Field(boundsSize, boundsCheck.BoundsSize);
							}

							boundsCheck.MultiplyBoundsSize = CustomEditorGUILayout.Slider(multiplyBoundsSize, boundsCheck.MultiplyBoundsSize, 0, 5);
						}
						else if(boundsCheck.BoundsType == BoundsCheckType.BoundsPrefab)
						{
							boundsCheck.MultiplyBoundsSize = CustomEditorGUILayout.Slider(multiplyBoundsSize, boundsCheck.MultiplyBoundsSize, 0, 5);
						}
						break;
					}
					case OverlapShape.Sphere:
					{
						SphereCheck sphereCheck = settings.SphereCheck;

						sphereCheck.VegetationMode = CustomEditorGUILayout.Toggle(vegetationMode, sphereCheck.VegetationMode);

						if(sphereCheck.VegetationMode)
						{
							sphereCheck.Priority = CustomEditorGUILayout.IntField(priority, sphereCheck.Priority);
							sphereCheck.TrunkSize = CustomEditorGUILayout.Slider(trunkSize, sphereCheck.TrunkSize, 0, 10);
							sphereCheck.ViabilitySize = CustomEditorGUILayout.FloatField(viabilitySize, sphereCheck.ViabilitySize);

							if(sphereCheck.ViabilitySize < settings.SphereCheck.TrunkSize)
							{
								sphereCheck.ViabilitySize = settings.SphereCheck.TrunkSize;
							}
						}
						else
						{
							sphereCheck.Size = CustomEditorGUILayout.FloatField(size, sphereCheck.Size);
						}
						break;
					}
				}

				EditorGUI.indentLevel--;

				CollisionCheck collisionCheck = settings.CollisionCheck;

				collisionCheck.collisionCheckType =  CustomEditorGUILayout.Toggle(new GUIContent("Collision Check"), collisionCheck.collisionCheckType);
				
				if(collisionCheck.collisionCheckType)
				{
					EditorGUI.indentLevel++;

					collisionCheck.multiplyBoundsSize = CustomEditorGUILayout.Slider(multiplyBoundsSize, collisionCheck.multiplyBoundsSize, 0, 10);
					collisionCheck.checkCollisionLayers = CustomEditorGUILayout.LayerField(new GUIContent("Check Collision Layers"), collisionCheck.checkCollisionLayers);

					EditorGUI.indentLevel--;
				}

				if(CustomEditorGUILayout.EndChangeCheck())
				{
					EditorUtility.SetDirty(settings);
				}

				EditorGUI.indentLevel--;
			}
        }

		[NonSerialized]
		public GUIContent overlapShape = new GUIContent("Overlap Shape", "What shape will be checked for intersection with other prototypes. Overlap Shape only works with added prototypes in MegaWorld. Overlap Chap can be Bounds and Sphere.");

		#region Bounds Check
		[NonSerialized]
		public GUIContent boundsType = new GUIContent("Bounds Type", "Which Bounds will be used.");
		[NonSerialized]
		public GUIContent uniformBoundsSize = new GUIContent("Uniform Bounds Size", "Each side of the Bounds has the same size value.");
		[NonSerialized]
		public GUIContent boundsSize = new GUIContent("Bounds Size", "Lets you choose the size of the vector for bounds size.");
		[NonSerialized]
		public GUIContent multiplyBoundsSize = new GUIContent("Multiply Bounds Size", "Allows you to resize the bounds.");
		#endregion

		#region Sphere Variables
		[NonSerialized]
		public GUIContent vegetationMode = new GUIContent("Vegetation Mode", "Allows you to use the priority system, which allows for example small trees to spawn under a large tree.");
		[NonSerialized]
		public GUIContent priority = new GUIContent("Priority", "Sets the ability of the object so that the object can spawn around the Viability Size of another object whose this value is less.");
		[NonSerialized]
		public GUIContent trunkSize = new GUIContent("Trunk Size", "Sets the size of the trunk. Other objects will never be spawn in this size.");
		[NonSerialized]
		public GUIContent viabilitySize = new GUIContent("Viability Size", " This is size in which other objects will not be spawned if Priority is less.");
		[NonSerialized]
		public GUIContent size = new GUIContent("Size", "The size of the sphere that will not spawn.");
		#endregion

		#region Collision Check
		[NonSerialized]
		public GUIContent collisionCheckType = new GUIContent("Collision Check", "Used to prevent the object from spawning inside the GameObject. This is a useful feature to prevent an object from spawning inside a building, for example. Overlap Shape only works with added prototypes in MegaWorld, and this function allows you to check for overlaps for all GameObjects.");
		[NonSerialized]
		public GUIContent checkCollisionLayers = new GUIContent("Check Collision Layers", "Layer to be checked for overlap.");
		#endregion
    }
}
#endif
