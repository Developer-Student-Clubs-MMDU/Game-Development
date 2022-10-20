using UnityEngine;
using System;

namespace VladislavTsurikov.MegaWorldSystem
{
    [Serializable]
    public class CollisionCheck
    {
        public float multiplyBoundsSize = 1;
        public bool collisionCheckType = false;
        public LayerMask checkCollisionLayers;

        public CollisionCheck()
        {

        }

        public CollisionCheck(CollisionCheck other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(CollisionCheck other)
        {            
            collisionCheckType = other.collisionCheckType;
            multiplyBoundsSize = other.multiplyBoundsSize;
            checkCollisionLayers = other.checkCollisionLayers;
        }

        public bool IsBoundHittingWithCollisionsLayers(Vector3 position, float rotation, Vector3 extents)
        {
			RaycastHit hitInfo;
			if (Physics.BoxCast(new Vector3(position.x, position.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.Offset, position.z), extents, 
				Vector3.down, out hitInfo, Quaternion.Euler(0f, rotation, 0f), MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.Offset + 1000f, checkCollisionLayers))
            {
                return true;
            }

            return false;
        }
    }
}
