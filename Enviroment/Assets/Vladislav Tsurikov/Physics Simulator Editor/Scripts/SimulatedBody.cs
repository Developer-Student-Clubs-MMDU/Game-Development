#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor
{
    public class SimulatedBody 
    {
        private GameObject _gameObject = null;
        private Rigidbody addedRigidbodie;
        private Collider addedCollider;
        private List<Collider> _nonConvexColliders;

        public Rigidbody Rigidbody
        {
            get
            {
                return addedRigidbodie;
            }
        }

        public GameObject GameObject
        {
            get
            {
                return _gameObject;
            }
        }

        public SimulatedBody(GameObject gameObject)
        {
            _gameObject = gameObject;

            AddPhysicsSupport();
        }

        public bool HasObjectStopped()
        {
            if(_gameObject == null)
            {
                return true;
            }

            if(addedRigidbodie == null || addedRigidbodie.IsSleeping())
            {
                return true;
            }

            return false;
        }

        public void AddPhysicsSupport()
        {
            _nonConvexColliders = new List<Collider>();

            Collider[] colliders = _gameObject.transform.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.hideFlags = HideFlags.HideInHierarchy;

                if(collider is MeshCollider && !((MeshCollider) collider).convex)
                {
                    _nonConvexColliders.Add(collider);

                    ((MeshCollider)collider).convex = true;
                }
            }

            AddСomponentsIfNecessary();
        }

        public void DisablePhysicsSupport()
        {
            if(_gameObject == null || _nonConvexColliders == null) return;

            RemoveAddedComponents();

            Collider[] colliders = _gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider is MeshCollider && _nonConvexColliders.Contains(collider))
                {
                    ((MeshCollider)collider).convex = false;
                }
            }

            _nonConvexColliders = null;
        }

        public bool HasRigidbody()
        {
            if(_gameObject == null || _nonConvexColliders == null) return false;

            if (_gameObject.GetComponent<Rigidbody>())
            {
                return true;
            }

            return false;
        }

        void AddСomponentsIfNecessary()
        {
            if (!_gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rigidbody = _gameObject.gameObject.AddComponent<Rigidbody>();

                rigidbody.useGravity = true;
                rigidbody.mass = 1;

                addedRigidbodie = rigidbody;
            }

            if (!_gameObject.GetComponent<Collider>())
            {
                MeshCollider collider = _gameObject.gameObject.AddComponent<MeshCollider>();

                // hide colliders in the hierarchy, they cost performance
                collider.hideFlags = HideFlags.HideInHierarchy;

                collider.convex = true;

                addedCollider = collider;
            }
        }

        void RemoveAddedComponents()
        {
            if(addedRigidbodie != null)
            {
                MonoBehaviour.DestroyImmediate(addedRigidbodie);
            }

            if(addedCollider != null)
            {
                MonoBehaviour.DestroyImmediate(addedCollider);
            }
        }
    }
}
#endif