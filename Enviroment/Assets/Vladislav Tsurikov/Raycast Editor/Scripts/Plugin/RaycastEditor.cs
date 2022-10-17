#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.RaycastEditorSystem
{
    [InitializeOnLoad]
    public static class RaycastEditor 
    {
        private static bool _needsDelayedInit;
        private static SceneObjectTree _objectTree = new SceneObjectTree();

        public delegate bool HandleUtility_IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out UnityEngine.RaycastHit raycastHit);
        public static HandleUtility_IntersectRayMesh IntersectRayMesh = null;
        public static bool HandleTransformChanges = false;

        static RaycastEditor()
        {
            MethodInfo methodIntersectRayMesh = typeof(HandleUtility).GetMethod("IntersectRayMesh", BindingFlags.Static | BindingFlags.NonPublic);

            if (methodIntersectRayMesh != null)
            {
                IntersectRayMesh = delegate (Ray ray, Mesh mesh, Matrix4x4 matrix, out UnityEngine.RaycastHit raycastHit)
                {
                    object[] parameters = new object[] { ray, mesh, matrix, null };
                    bool result = (bool)methodIntersectRayMesh.Invoke(null, parameters);
                    raycastHit = (UnityEngine.RaycastHit)parameters[3];
                    return result;
                };
            }

            EditorApplication.hierarchyChanged -= OnSceneObjectsChanged;
            EditorApplication.hierarchyChanged += OnSceneObjectsChanged;

            SceneManagement.ActiveSceneChangedInEditMode -= ChangedActiveScene;
            SceneManagement.ActiveSceneChangedInEditMode += ChangedActiveScene;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void ChangedActiveScene(Scene current, Scene next)
        {
            RefreshObjectTree();
        }

        private static void Update()
		{
            if (_needsDelayedInit) DoDelayedInit();
            _objectTree.HandleTransformChangesForAllRegisteredObjects();
		}

        private static void DoDelayedInit()
        {
            RefreshObjectTree();
            _needsDelayedInit = false;
        }

        public static List<GameObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter overlapFilter)
        {
            return _objectTree.OverlapBox(boxCenter, boxSize, boxRotation, overlapFilter);
        }

        public static List<GameObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter overlapFilter)
        {
            return _objectTree.OverlapSphere(sphereCenter, sphereRadius, overlapFilter);
        }

        public static RayHit Raycast(Ray ray, LayerMask layerMask)
        {
            ObjectFilter raycastFilter = new ObjectFilter();
            raycastFilter.LayerMask = layerMask;

            return Raycast(ray, raycastFilter);
        }

        public static RayHit Raycast(Ray ray, ObjectFilter raycastFilter)
        {            
            List<RayHit> allObjectHits = _objectTree.RaycastAll(ray, raycastFilter);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;

            return closestObjectHit;
        }

        public static List<RayHit> RaycastAll(Ray ray, ObjectFilter raycastFilter)
        {
            return _objectTree.RaycastAll(ray, raycastFilter);
        }

        public static GameObject[] GetSceneObjects()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                var roots = new List<GameObject>(Mathf.Max(1, activeScene.rootCount));
                activeScene.GetRootGameObjects(roots);
                List<GameObject> sceneObjects = new List<GameObject>(Mathf.Max(1, activeScene.rootCount * 5));

                foreach (var root in roots)
                {
                    var allChildrenAndSelf = root.GetAllChildrenAndSelf();
                    sceneObjects.AddRange(allChildrenAndSelf);
                }

                return sceneObjects.ToArray();
            }

            return new GameObject[0];
        }    

        public static void RefreshObjectTree()
        {
            _objectTree.Clear();
            _objectTree.RemoveNullObjectNodes();
            _objectTree.RegisterGameObjects(GetSceneObjects());
        }

        public static void RegisterGameObject(GameObject gameObject)
        {
            if(gameObject == null) return;
            
            List<GameObject> allChildrenIncludingSelf = gameObject.GetAllChildrenAndSelf();
            _objectTree.RegisterGameObjects(allChildrenIncludingSelf);
        }

        private static void OnSceneObjectsChanged()
        {
            _objectTree.RemoveNullObjectNodes();
        }

        #region Gizmos
        public static void DrawGizmosRaycastAll(Color nodeColor, Color containObjectNodeColor)
        {
            _objectTree.DrawGizmosRaycastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), null, nodeColor, containObjectNodeColor);
        }

        public static void DrawGizmosAllCells(Color nodeColor)
        {
            _objectTree.DrawGizmosAllCells(nodeColor);
        }
        #endregion
    }
}
#endif