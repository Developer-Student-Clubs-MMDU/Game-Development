using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class PlacedObjectUtility
    {
        public static PlacedObject PlaceObject(GameObject prefab, Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
            GameObject go;

#if UNITY_EDITOR
            go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            go = MonoBehaviour.Instantiate(prefab);
#endif

            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

            PlacedObject objectInfo = new PlacedObject();
            objectInfo.GameObject = go;
            VladislavTsurikov.GameObjectUtility.GetObjectWorldBounds(go, out objectInfo.Bounds);

            return objectInfo;
        }

        public static void FindTypeParentObject(Group group)
        {
            string groupName = group.Name;

            UnityEngine.Transform container = null;
            
            GameObject[] sceneRoots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject root in sceneRoots)
			{
				if(root.name == groupName) 
                {
					container = root.transform;
                    break;
				}
			} 

            if (container == null)
            {
                GameObject childObject = new GameObject(groupName);
                container = childObject.transform;
            }

            group.ContainerForGameObjects = container.gameObject;
        }

        public static void ParentGameObject(Group group, PlacedObject objectInfo)
        {            
            if(group.ContainerForGameObjects == null)
            {
                FindTypeParentObject(group);
            }

            GameObjectUtility.ParentGameObjectIfNecessary(objectInfo.GameObject, group.ContainerForGameObjects);
        }
    }
}