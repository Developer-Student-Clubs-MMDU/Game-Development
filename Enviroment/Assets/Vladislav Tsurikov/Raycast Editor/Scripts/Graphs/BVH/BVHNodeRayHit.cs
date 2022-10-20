#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public class BVHNodeRayHit<TNodeData>
        where TNodeData : class
    {
        private BVHNode<TNodeData> _hitNode;
        private float _distance;
        private Vector3 _hitPoint;

        public BVHNode<TNodeData> HitNode { get { return _hitNode; } }
        public float Distance { get { return _distance; } }
        public Vector3 HitPoint { get { return _hitPoint; } }

        public BVHNodeRayHit(Ray ray, BVHNode<TNodeData> hitNode, float distance)
        {
            _hitNode = hitNode;
            _distance = distance;
            _hitPoint = ray.GetPoint(_distance);
        }
    }
}
#endif