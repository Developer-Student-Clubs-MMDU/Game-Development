#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public class SceneObjectTree
    {
        private BVHTree<BVHNodeAABB<RaycastObject>, RaycastObject> _tree = new BVHTree<BVHNodeAABB<RaycastObject>, RaycastObject>();
        private Dictionary<RaycastObject, BVHNodeAABB<RaycastObject>> _objectToNode = new Dictionary<RaycastObject, BVHNodeAABB<RaycastObject>>();

        public List<GameObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter)
        {
            var overlappedNodes = _tree.OverlapBox(boxCenter, boxSize, boxRotation);
            if (overlappedNodes.Count == 0) return new List<GameObject>();

            var overlappedObjects = new List<GameObject>();
            foreach(var node in overlappedNodes)
            {
                if (objectFilter != null)
                {
                    if(!objectFilter.Filter(node.Data.GameObject))
                    {
                        continue;
                    }
                }

                GameObject gameObject = node.Data.GameObject;
                if (gameObject == null || !gameObject.activeInHierarchy) continue;

                Mesh mesh = gameObject.GetMesh();
                if (mesh != null && !gameObject.IsRendererEnabled()) continue;

                OBB worldOBB = ObjectBounds.CalcWorldOBB(gameObject);
                if (worldOBB.IsValid)
                {
                    if (BoxMath.BoxIntersectsBox(worldOBB.Center, worldOBB.Size, worldOBB.Rotation,
                        boxCenter, boxSize, boxRotation)) overlappedObjects.Add(gameObject);
                }
            }

            return overlappedObjects;
        }

        public List<GameObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter)
        {
            var overlappedNodes = _tree.OverlapSphere(sphereCenter, sphereRadius);
            if (overlappedNodes.Count == 0) return new List<GameObject>();

            float radiusSqr = sphereRadius * sphereRadius;
            var overlappedObjects = new List<GameObject>();
            foreach (var node in overlappedNodes)
            {
                if (objectFilter != null)
                {
                    if(!objectFilter.Filter(node.Data.GameObject))
                    {
                        continue;
                    }
                }

                GameObject gameObject = node.Data.GameObject;
                if (gameObject == null || !gameObject.activeInHierarchy) continue;

                Mesh mesh = gameObject.GetMesh();
                if (mesh != null && !gameObject.IsRendererEnabled()) continue;

                OBB worldOBB = ObjectBounds.CalcWorldOBB(gameObject);
                if (worldOBB.IsValid)
                {
                    Vector3 closestPt = BoxMath.CalcBoxPtClosestToPt(sphereCenter, worldOBB.Center, worldOBB.Size, worldOBB.Rotation);
                    if ((closestPt - sphereCenter).sqrMagnitude <= radiusSqr) overlappedObjects.Add(gameObject);
                }
            }

            return overlappedObjects;

        }

        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter)
        {
            var nodeHits = _tree.RaycastAll(ray, false);
            if (nodeHits.Count == 0) return new List<RayHit>();

            var sortedObjectHits = new List<RayHit>(20);
            foreach (var hit in nodeHits)
            {
                if (objectFilter != null)
                {
                    if(!objectFilter.Filter(hit.HitNode.Data.GameObject))
                    {
                        continue;
                    }
                }

                RaycastObject raycastObject = hit.HitNode.Data;
                if (raycastObject == null || raycastObject.GameObject == null || !raycastObject.GameObject.activeInHierarchy) continue;

                if (raycastObject.Mesh != null)
                {
                    if (raycastObject.IsRendererEnabled())
                    {
                        UnityEngine.RaycastHit unityRaycastHit = default(UnityEngine.RaycastHit);

                        if (RaycastEditor.IntersectRayMesh != null &&
                        RaycastEditor.IntersectRayMesh(ray, raycastObject.Mesh, raycastObject.GameObject.transform.localToWorldMatrix, out unityRaycastHit))
                        {
                            sortedObjectHits.Add(new RayHit(ray, raycastObject.GameObject, unityRaycastHit.normal, unityRaycastHit.point, unityRaycastHit.distance));
                        }
                    }
                    continue;
                }
                if (raycastObject.TerrainCollider != null)
                {
                    RaycastHit raycastHit;
                    if (raycastObject.TerrainCollider.Raycast(ray, out raycastHit, float.MaxValue))
                        sortedObjectHits.Add(new RayHit(ray, raycastObject.GameObject, raycastHit.normal, raycastHit.point, raycastHit.distance));
                    continue;
                }
            }

            RayHit.SortByHitDistance(sortedObjectHits);

            return sortedObjectHits;
        }
        
        public void Clear()
        {
            RemoveNullObjectNodes();
            foreach(var pair in _objectToNode)
            {
                _tree.RemoveContainObjectNode(pair.Value);
            }
            _objectToNode.Clear();
            _tree.Clear();
        }

        public void RemoveNullObjectNodes()
        {
            bool foundNull = false;
            foreach(var pair in _objectToNode)
            {
                if (pair.Value.Data == null || pair.Value.Data.GameObject == null)
                {
                    foundNull = true;
                    _tree.RemoveContainObjectNode(pair.Value);
                }
            }
            
            if (foundNull)
            {
                var newDictionary = new Dictionary<RaycastObject, BVHNodeAABB<RaycastObject>>();
                foreach(var pair in _objectToNode)
                {
                    if (pair.Key != null)
                        newDictionary.Add(pair.Key, pair.Value);
                }

                _objectToNode.Clear();
                _objectToNode = newDictionary;
            }
            
        }

        public void RegisterGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach(var gameObject in gameObjects)
            {
                RegisterGameObject(gameObject);
            }
        }

        private void RegisterGameObject(GameObject gameObject)
        {
            AABB objectAABB = ObjectBounds.CalcWorldAABB(gameObject);
            if (objectAABB.IsValid)
            {
                RaycastObject raycastObject = RaycastObject.MakeRaycastObject(gameObject);
                
                if(raycastObject != null)
                {
                    raycastObject.GameObject.transform.hasChanged = false;

                    var treeNode = new BVHNodeAABB<RaycastObject>(raycastObject);
                    treeNode.Position = objectAABB.Center;
                    treeNode.Size = objectAABB.Size;
                    _tree.InsertContainObjectNode(treeNode);

                    _objectToNode.Add(raycastObject, treeNode); 
                }
            }
        }

        public void HandleTransformChangesForAllRegisteredObjects()
        {
            if(Selection.gameObjects.Length != 0 || RaycastEditor.HandleTransformChanges)
            {
                Dictionary<RaycastObject, BVHNodeAABB<RaycastObject>> changedTransform = new Dictionary<RaycastObject, BVHNodeAABB<RaycastObject>>();

                // Loop through all object-to-nodes pairs
                foreach (var pair in _objectToNode)
                {
                    // Can be null if the object was destroyed in the meantime
                    if (pair.Key.GameObject == null) continue;

                    RaycastObject gameObjectTransform = pair.Key;
                    if (gameObjectTransform.GameObject.transform.hasChanged)
                    {
                        changedTransform.Add(pair.Key, pair.Value);
                        gameObjectTransform.GameObject.transform.hasChanged = false;
                    }
                }

                foreach (var pair in changedTransform)
                {
                    _tree.RemoveContainObjectNode(pair.Value);
                    _objectToNode.Remove(pair.Key);
                    RegisterGameObject(pair.Key.GameObject);
                }

                RaycastEditor.HandleTransformChanges = false;
            }
        }

        #region Gizmos
        public void DrawGizmosRaycastAll(Ray ray, ObjectFilter raycastFilter, Color nodeColor, Color containObjectNodeColor)
        {
            _tree.DrawGizmosRaycastAll(ray, Matrix4x4.identity, nodeColor, containObjectNodeColor);
        }

        public void DrawGizmosAllCells(Color nodeColor)
        {
            _tree.DrawGizmosAllCells(Matrix4x4.identity, nodeColor);
        }
        #endregion
    }
}
#endif