#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Events;

namespace VladislavTsurikov
{
    //This class was created because the EditorSceneManager.activeSceneChanged API was not working at the time of development
    public static class SceneManagement
    {
        private static Scene _pastScene; 
        private static int _pastSceneCount;

        public static event UnityAction<Scene, Scene> ActiveSceneChangedInEditMode;

        static SceneManagement()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void Update()
		{
            if(_pastScene != null)
            {
                if(_pastScene != EditorSceneManager.GetActiveScene())
                {
                    ActiveSceneChangedInEditMode(_pastScene, EditorSceneManager.GetActiveScene());
                }
            }
            else if(_pastSceneCount != EditorSceneManager.sceneCount)
            {
                ActiveSceneChangedInEditMode(_pastScene, EditorSceneManager.GetActiveScene());
            }

            _pastScene = EditorSceneManager.GetActiveScene();
            _pastSceneCount = EditorSceneManager.sceneCount;
		}
    }
}
#endif