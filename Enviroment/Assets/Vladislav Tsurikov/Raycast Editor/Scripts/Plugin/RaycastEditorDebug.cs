#if UNITY_EDITOR
//#define OVERLAP_SHAPE_ON_GRID
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.RaycastEditorSystem
{
    /// <summary>
    /// An enum which allows us to switch between different demos. Essentially,
    /// it will be used to control the type of test that we will perform in the
    /// scene view (raycast, overlap etc).
    /// </summary>
    public enum DemoMode
    {
        Raycast = 1,
        BoxOverlap,
        SphereOverlap
    }

    public enum DebugMode
    {
        ShowAllCells,
        ShowHitRaycastCells,
        ShowHitRaycast,
    }

    /// <summary>
    /// A simple demo class which allows the user to perform different tests using the
    /// CFE API and show the results inside the scene view.
    /// </summary>
    [ExecuteInEditMode]
    public class RaycastEditorDebug : MonoBehaviour
    {
        public LayerMask LayerMask = new LayerMask();
        public DebugMode DebugMode = DebugMode.ShowAllCells;

        [SerializeField]
        private DemoMode _demoMode = DemoMode.Raycast;

        private Vector3 _overlapBoxCenter;
        [SerializeField]
        private Vector3 _overlapBoxSize = Vector3.one;
        [SerializeField]
        private Vector3 _overlapBoxEuler = Vector3.zero;

        private Vector3 _overlapSphereCenter;
        [SerializeField]
        private float _overlapSphereRadius = 1.0f;

        [SerializeField]
        private Color _overlapShapeColor = Color.blue.WithAlpha(0.5f);
        [SerializeField]
        private Color _overlappedSolidColor = Color.green.WithAlpha(0.5f);
        [SerializeField]
        private Color _overlappedWireColor = Color.black;
        

        [SerializeField]
        private float _hitNormalLength = 1.0f;
        [SerializeField]
        private float _hitPointSize = 0.08f;
        [SerializeField]
        private Color _hitNormalColor = Color.green;
        [SerializeField]
        private Color _hitPointColor = Color.green;

        private RayHit _objectRayHit;
        private List<GameObject> _overlappedObjects = new List<GameObject>();

        public Color NodeColor = Color.white;
        public Color ContainObjectNodeColor = Color.blue.WithAlpha(0.2f);
        public Vector3 OverlapBoxSize { get { return _overlapBoxSize; } set { _overlapBoxSize = Vector3.Max(value, Vector3.one * 1e-5f); } }
        public float OverlapSphereRadius { get { return _overlapSphereRadius; } set { _overlapSphereRadius = Mathf.Max(value, 1e-5f); } }
        public float HitNormalLength { get { return _hitNormalLength; } set { _hitNormalLength = Mathf.Max(1e-5f, value); } }
        public float HitPointSize { get { return _hitPointSize; } set { _hitPointSize = Mathf.Max(1e-5f, value); } }

        /// <summary>
        /// Called when the script is enabled.
        /// </summary>
        private void OnEnable()
        {
            // Register the OnSceneGUI handler
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// Called when the script is disabled.
        /// </summary>
        private void OnDisable()
        {
            // Unregister the OnSceneGUI handler
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnDrawGizmosSelected() 
        {
            switch (DebugMode)
            {
                case DebugMode.ShowAllCells:
                {
                    RaycastEditor.DrawGizmosAllCells(NodeColor);
                    break;
                }
                case DebugMode.ShowHitRaycastCells:
                {
                    RaycastEditor.DrawGizmosRaycastAll(NodeColor, ContainObjectNodeColor);
                    ShowHitRaycast();
                    break;
                }
            }
        }

        public void ShowHitRaycast()
        {
            // If we are raycasting and something was actually hit, we will draw a label
            // that contains some information about the hit.
            if (_demoMode == DemoMode.Raycast && _objectRayHit != null)
            {               
                // Use a yellow color. Seems to work really well at least with the dev's workspace.
                GUIStyle style = new GUIStyle("label");
                style.normal.textColor = Color.yellow;

                // Build the label text. We will show the coordinates of the hit point and the hit point normal.
                var labelText = "Hit Point: " + _objectRayHit.Point.ToString() + "; \r\nHit Normal: " + _objectRayHit.Normal.ToString();
                Handles.Label(_objectRayHit.Point, new GUIContent(labelText), style);
            }

            if (_demoMode == DemoMode.Raycast && _objectRayHit != null)
            {
                // Draw a sphere centered on the position of the hit point and a normal emenating from that point
                GizmosEx.PushColor(_hitPointColor);
                Gizmos.DrawSphere(_objectRayHit.Point, _hitPointSize * HandleUtility.GetHandleSize(_objectRayHit.Point));
                GizmosEx.PopColor();
                GizmosEx.PushColor(_hitNormalColor);
                Gizmos.DrawLine(_objectRayHit.Point, _objectRayHit.Point + _objectRayHit.Normal * _hitNormalLength);
                GizmosEx.PopColor();
            }
            else
            if (_demoMode == DemoMode.BoxOverlap)
            {
                // Draw the box shape
                GizmosEx.PushColor(_overlapShapeColor);
                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapBoxCenter, Quaternion.Euler(_overlapBoxEuler), _overlapBoxSize));
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                GizmosEx.PopMatrix();
                GizmosEx.PopColor();

                // Draw the overlapped volumes
                DrawOverlappedVolumesGizmos();
            }
            else
            if (_demoMode == DemoMode.SphereOverlap)
            {
                // Draw the sphere shape
                GizmosEx.PushColor(_overlapShapeColor);
                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapSphereCenter, Quaternion.identity, Vector3.one * _overlapSphereRadius));
                Gizmos.DrawSphere(Vector3.zero, 1.0f);
                GizmosEx.PopMatrix();
                GizmosEx.PopColor();

                // Draw the overlapped volumes
                DrawOverlappedVolumesGizmos();
            }
        }

        /// <summary>
        /// Event handler for the 'SceneView.onSceneGUIDelegate' event.
        /// </summary>
        /// <remarks>
        /// You could also just have a custom editor (e.g. class MyCustomEditor : Editor {}) 
        /// for your own MonoBehaviour and implement the logic there inside the OnSceneGUI
        /// function. The advantage of using an event handler is that the object does not have
        /// to be selected for the logic to execute.
        /// </remarks>
        private void OnSceneGUI(SceneView sceneView)
        {
            // We only do anything if the current event is a mouse move event
            Event e = Event.current;
            if (e.type == EventType.MouseMove)
            {
                // Reset data
                _objectRayHit = null;
                _overlappedObjects.Clear();

                // Are we raycasting?
                if (_demoMode == DemoMode.Raycast)
                {
                    // Raycast
                    _objectRayHit = RaycastEditor.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), null);
                }
                else
                // Are we overlapping with a box?
                if (_demoMode == DemoMode.BoxOverlap)
                {
                    #if OVERLAP_SHAPE_ON_GRID
                    // Place the box on the scene grid
                    var gridCellHit = ColliderEditor.RaycastSceneGrid(HandleUtility.GUIPointToWorldRay(e.mousePosition));
                    if (gridCellHit != null)
                    {
                        // Perform the overlap test
                        _overlapBoxCenter = gridCellHit.HitPoint;
                        ObjectFilter overlapFilter = new ObjectFilter();
                        overlapFilter.LayerMask = layerMask;
                        _overlappedObjects = ColliderEditor.OverlapBox(_overlapBoxCenter, _overlapBoxSize, Quaternion.Euler(_overlapBoxEuler), overlapFilter);
                    }
                    #else
                    // Place the box on the hovered object
                    RayHit objectHit = RaycastEditor.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), null);
                    if (objectHit != null)
                    {
                        // Perform the overlap test
                        _overlapBoxCenter = objectHit.Point;
                        ObjectFilter overlapFilter = new ObjectFilter();
                        overlapFilter.LayerMask = LayerMask;
                        _overlappedObjects = RaycastEditor.OverlapBox(_overlapBoxCenter, _overlapBoxSize, Quaternion.Euler(_overlapBoxEuler), overlapFilter);
                    }
                    #endif
                }
                else
                // Are we overlapping with a sphere?
                if (_demoMode == DemoMode.SphereOverlap)
                {
                    #if OVERLAP_SHAPE_ON_GRID
                    // Place the sphere on the scene grid
                    var gridCellHit = ColliderEditor.RaycastSceneGrid(HandleUtility.GUIPointToWorldRay(e.mousePosition));
                    if (gridCellHit != null)
                    {
                        // Perform the overlap test
                        _overlapSphereCenter = gridCellHit.HitPoint;
                        ObjectFilter overlapFilter = new ObjectFilter();
                        overlapFilter.LayerMask = layerMask;
                        _overlappedObjects = ColliderEditor.OverlapSphere(_overlapSphereCenter, _overlapSphereRadius, overlapFilter);
                    }
                    #else
                    // Place the sphere on the hovered object
                    var objectHit = RaycastEditor.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), null);
                    if (objectHit != null)
                    {
                        // Perform the overlap test
                        _overlapSphereCenter = objectHit.Point;
                        ObjectFilter overlapFilter = new ObjectFilter();
                        overlapFilter.LayerMask = LayerMask;
                        _overlappedObjects = RaycastEditor.OverlapSphere(_overlapSphereCenter, _overlapSphereRadius, overlapFilter);
                    }
                    #endif
                }

                // Repaint the scene
                sceneView.Repaint();
            }
        }

        /// <summary>
        /// Draws the volumes of the objects which are overlapped by the box
        /// or sphere (depedning on the active demo mode);
        /// </summary>
        private void DrawOverlappedVolumesGizmos()
        {
            // Set the color and then loop through each overlapped object and draw it
            GizmosEx.PushColor(_overlappedSolidColor);
            foreach (var gameObj in _overlappedObjects)
            {
                // Calculate the object's world OBB. If the OBB is valid, draw it.
                OBB worldOBB = ObjectBounds.CalcWorldOBB(gameObj);
                if (worldOBB.IsValid)
                {
                    // Inflate the OBB a bit to avoid any Z wars (e.g. cubes)
                    worldOBB.Inflate(1e-3f);

                    // Activate the trasnform matrix and then draw.
                    // Note: We use the OBB's transform information to build the matrix
                    GizmosEx.PushMatrix(Matrix4x4.TRS(worldOBB.Center, worldOBB.Rotation, worldOBB.Size));
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    GizmosEx.PushColor(_overlappedWireColor);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopColor();
                    GizmosEx.PopMatrix();
                }
            }

            // Restore color
            GizmosEx.PopColor();
        }

        /// <summary>
        /// Called when we modify properties in the Inspector. It allows us to
        /// apply restrictions on some of the properties.
        /// </summary>
        private void OnValidate()
        {
            OverlapBoxSize = _overlapBoxSize;
            OverlapSphereRadius = _overlapSphereRadius;
            HitNormalLength = _hitNormalLength;
            HitPointSize = _hitPointSize;
        }
    }
}
#endif