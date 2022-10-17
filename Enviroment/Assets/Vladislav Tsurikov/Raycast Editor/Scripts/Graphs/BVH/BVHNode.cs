#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public abstract class BVHNode<TData>
        where TData : class
    {
        private TData _data;
        private bool _containObject;
        private BVHNode<TData> _parent;
        private List<BVHNode<TData>> _children = new List<BVHNode<TData>>();

        public TData Data { get { return _data; } }
        public bool ContainObject { get { return _containObject; } }
        public abstract Vector3 Position { get; set; }
        public abstract Vector3 Size { get; set; }
        public BVHNode<TData> Parent { get { return _parent; } }
        public int NumChildren { get { return _children.Count; } }

        public BVHNode()
        {

        }

        public BVHNode(TData data)
        {
            _data = data;
        }

        public BVHNode<TData> GetChild(int index)
        {
            return _children[index];
        }

        public void SetContainObject()
        {
            _containObject = true;
        }

        public void SetParent(BVHNode<TData> parent)
        {
            if (_parent != null) _parent._children.Remove(this);

            _parent = parent;
            if (_parent != null) _parent._children.Add(this);
        }

        public BVHNode<TData> FindClosestChild(BVHNode<TData> node)
        {
            BVHNode<TData> closestChild = null;
            float minDistSqr = float.MaxValue;
            foreach (var child in _children)
            {
                float distSqr = (node.Position - child.Position).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    closestChild = child;
                }
            }

            return closestChild;
        }

        public void EncapsulateChildrenBottomUp()
        {
            if (ContainObject || NumChildren == 0) return;

            BVHNode<TData> currentParent = this;
            while (currentParent != null)
            {
                currentParent.Position = currentParent._children[0].Position;
                currentParent.Size = currentParent._children[0].Size;

                for (int childIndex = 1; childIndex < currentParent.NumChildren; ++childIndex)
                    currentParent.EncapsulateNode(currentParent._children[childIndex]);

                currentParent = currentParent.Parent;
            }
        }

        public abstract bool Raycast(Ray ray, out float t);
        public abstract bool IntersectsBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation);
        public abstract bool IntersectsSphere(Vector3 sphereCenter, float sphereRadius);

        protected abstract void EncapsulateNode(BVHNode<TData> node);
    }
}
#endif