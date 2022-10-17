#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using VladislavTsurikov.Extensions;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public class BVHTree<TNode, TNodeData>
        where TNode : BVHNode<TNodeData>, new()
        where TNodeData : class
    {
        private const int _numChildren = 2;
        private TNode _root = new TNode();

        public void Clear()
        {
            _root = new TNode();
        }

        public void InsertContainObjectNode(TNode node)
        {
            InsertContainObjectNodeRecurse(_root, node);
            node.SetContainObject();
        }
        
        public void RemoveContainObjectNode(TNode node)
        {
            if (!node.ContainObject) return;

            BVHNode<TNodeData> currentParent = node.Parent;
            node.SetParent(null);

            while (currentParent != null && currentParent != _root && currentParent.NumChildren == 0)
            {
                BVHNode<TNodeData> newParent = currentParent.Parent;
                currentParent.SetParent(null);
                currentParent = newParent;
            }
            
            if(currentParent != null)
            {
                currentParent.EncapsulateChildrenBottomUp();
            }
        }

        public List<BVHNodeRayHit<TNodeData>> RaycastAll(Ray ray, bool sort)
        {
            var nodeHits = new List<BVHNodeRayHit<TNodeData>>(10);
            RaycastAllRecurse(ray, _root, nodeHits);

            if (sort)
            {
                nodeHits.Sort(delegate(BVHNodeRayHit<TNodeData> h0, BVHNodeRayHit<TNodeData> h1)
                {
                    return h0.Distance.CompareTo(h1.Distance);
                });
            }

            return nodeHits;
        }

        public List<BVHNode<TNodeData>> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation)
        {
            var overlappedNodes = new List<BVHNode<TNodeData>>();
            OverlapBoxRecurse(_root, boxCenter, boxSize, boxRotation, overlappedNodes);
            return overlappedNodes;
        }


        public List<BVHNode<TNodeData>> OverlapSphere(Vector3 sphereCenter, float sphereRadius)
        {
            var overlappedNodes = new List<BVHNode<TNodeData>>();
            OverlapSphereRecurse(_root, sphereCenter, sphereRadius, overlappedNodes);
            return overlappedNodes;
        }

        private void InsertContainObjectNodeRecurse(BVHNode<TNodeData> parent, BVHNode<TNodeData> node)
        {
            if (!parent.ContainObject)
            {
                if (parent.NumChildren < _numChildren)
                {
                    node.SetParent(parent);
                    parent.EncapsulateChildrenBottomUp();
                }
                else
                {
                    BVHNode<TNodeData> closestChild = parent.FindClosestChild(node);
                    InsertContainObjectNodeRecurse(closestChild, node);
                }
            }
            else
            {
                TNode newParent = new TNode();
                BVHNode<TNodeData> oldParent = parent.Parent;

                node.SetParent(newParent);
                parent.SetParent(newParent);
                newParent.SetParent(oldParent);
                newParent.EncapsulateChildrenBottomUp();
            }
        }

        private void RaycastAllRecurse(Ray ray, BVHNode<TNodeData> node, List<BVHNodeRayHit<TNodeData>> outputNodes)
        {
            float t = 0.0f;
            if (node.Raycast(ray, out t))
            {
                if (node.ContainObject)
                {
                    outputNodes.Add(new BVHNodeRayHit<TNodeData>(ray, node, t));
                }
                
                for (int childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                {
                    RaycastAllRecurse(ray, node.GetChild(childIndex), outputNodes);
                }
            }
        }

        private void OverlapBoxRecurse(BVHNode<TNodeData> node, Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, List<BVHNode<TNodeData>> outputNodes)
        {
            if (node.IntersectsBox(boxCenter, boxSize, boxRotation))
            {
                if (node.ContainObject) outputNodes.Add(node);
                else
                {
                    for (int childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                        OverlapBoxRecurse(node.GetChild(childIndex), boxCenter, boxSize, boxRotation, outputNodes);
                }
            }
        }

        private void OverlapSphereRecurse(BVHNode<TNodeData> node, Vector3 sphereCenter, float sphereRadius, List<BVHNode<TNodeData>> outputNodes)
        {
            if (node.IntersectsSphere(sphereCenter, sphereRadius))
            {
                if (node.ContainObject) outputNodes.Add(node);
                else
                {
                    for (int childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                        OverlapSphereRecurse(node.GetChild(childIndex), sphereCenter, sphereRadius, outputNodes);
                }
            }
        }

        #region Gizmos
        private void DrawRaycastGizmosRecurse(Ray ray, BVHNode<TNodeData> node, Matrix4x4 transformMtx, Color nodeColor, Color containObjectNodeColor)
        {
            float t = 0.0f;
            if (node.Raycast(ray, out t))
            {
                if (node.ContainObject)
                {
                    Matrix4x4 nodeMatrix = Matrix4x4.TRS(node.Position, Quaternion.identity, node.Size);
                    GizmosEx.PushColor(containObjectNodeColor);
                    GizmosEx.PushMatrix(transformMtx * nodeMatrix);
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopMatrix();
                    GizmosEx.PopColor();
                }
                else
                {
                    GizmosEx.PushColor(nodeColor);
                    Matrix4x4 nodeMatrix = Matrix4x4.TRS(node.Position, Quaternion.identity, node.Size);
                    GizmosEx.PushMatrix(transformMtx * nodeMatrix);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopMatrix();
                    GizmosEx.PopColor();
                }

                for (int childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                {
                    DrawRaycastGizmosRecurse(ray, node.GetChild(childIndex), transformMtx, nodeColor, containObjectNodeColor);
                }
            }
        }

        public void DrawGizmosRaycastAll(Ray ray, Matrix4x4 transformMtx, Color nodeColor, Color containObjectNodeColor) 
        {
            DrawRaycastGizmosRecurse(ray, _root, transformMtx, nodeColor, containObjectNodeColor);
        }

        public void DrawGizmosAllCells(Matrix4x4 transformMtx, Color lineColor)
        {
            GizmosEx.PushColor(lineColor);
            DrawGizmosRecurse(_root, transformMtx, lineColor);
            GizmosEx.PopColor();
        }

        private void DrawGizmosRecurse(BVHNode<TNodeData> node, Matrix4x4 transformMtx, Color lineColor)
        {
            Matrix4x4 nodeMatrix = Matrix4x4.TRS(node.Position, Quaternion.identity, node.Size);
            GizmosEx.PushMatrix(transformMtx * nodeMatrix);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            GizmosEx.PopMatrix();
       
            for (int childIndex = 0; childIndex < node.NumChildren; ++childIndex)
                DrawGizmosRecurse(node.GetChild(childIndex), transformMtx, lineColor);
        }
        #endregion
    }
}
#endif