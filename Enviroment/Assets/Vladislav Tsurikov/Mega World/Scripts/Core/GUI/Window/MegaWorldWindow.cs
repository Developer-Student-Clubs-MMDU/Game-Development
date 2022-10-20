#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public partial class MegaWorldWindow : EditorWindow
    {
        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.modifierKeysChanged += Repaint;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.modifierKeysChanged -= Repaint;

            MegaWorldPath.CommonDataPackage.ToolComponentsEditor.DisableAllTools();

            MegaWorldPath.DataPackage.SaveAllData();
            MegaWorldPath.CommonDataPackage.Save();
        }

        private void OnSceneGUI(SceneView sceneView)
        { 
            MegaWorldPath.DataPackage.SelectedVariables.DeleteNullValueIfNecessary(MegaWorldPath.DataPackage.BasicData.GroupList);
            MegaWorldPath.DataPackage.SelectedVariables.SetAllSelectedParameters(MegaWorldPath.DataPackage.BasicData.GroupList);
            UpdateSceneViewEvent();

            MegaWorldPath.CommonDataPackage.ToolStack.DoSelectedTool();
        }
    }
} 
#endif