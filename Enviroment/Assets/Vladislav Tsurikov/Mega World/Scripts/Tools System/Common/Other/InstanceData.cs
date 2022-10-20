using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public class InstanceData
    {
        public Vector3 Position;
        public Vector3 Scale; 
        public Quaternion Rotation;

        public InstanceData()
        {
            
        }

        public InstanceData(GameObject gameObject)
        {
            this.Position = gameObject.transform.position;
            this.Scale = gameObject.transform.localScale; 
            this.Rotation = gameObject.transform.rotation;
        }
        
        public InstanceData(Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
            this.Position = position;
            this.Scale = scaleFactor; 
            this.Rotation = rotation;
        }
    }
}

